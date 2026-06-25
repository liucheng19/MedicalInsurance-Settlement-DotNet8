using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using Nacos.V2;
using Serilog;
using Yarp.ReverseProxy.Configuration;

namespace Gateway.Yarp;

public class NacosYarpConfigProvider : IProxyConfigProvider
{
    private readonly INacosNamingService _nacosNamingService;
    private readonly string _namespaceId;
    private readonly string _groupName;
    private readonly string[] _serviceNames;
    private NacosYarpConfig _config;
    private readonly ConcurrentDictionary<string, List<ServiceInstanceInfo>> _serviceInstances = new();

    public NacosYarpConfigProvider(INacosNamingService nacosNamingService, string namespaceId, string groupName, string[] serviceNames)
    {
        _nacosNamingService = nacosNamingService;
        _namespaceId = namespaceId;
        _groupName = groupName;
        _serviceNames = serviceNames;
        _config = new NacosYarpConfig(Array.Empty<RouteConfig>(), Array.Empty<ClusterConfig>());
    }

    public IProxyConfig GetConfig() => _config;

    public async Task InitializeAsync()
    {
        Log.Information("开始从Nacos初始化YARP服务配置...");

        foreach (var serviceName in _serviceNames)
        {
            try
            {
                var instances = await _nacosNamingService.SelectInstances(
                    serviceName, _groupName, new List<string> { "DEFAULT" }, true);

                var instanceList = instances.Select(i => new ServiceInstanceInfo
                {
                    Ip = i.Ip,
                    Port = i.Port,
                    Weight = i.Weight,
                    Healthy = i.Healthy
                }).ToList();

                _serviceInstances[serviceName] = instanceList;

                Log.Information("从Nacos加载服务 {ServiceName}，实例数量: {Count}", serviceName, instanceList.Count);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "加载Nacos服务 {ServiceName} 失败", serviceName);
                _serviceInstances[serviceName] = new List<ServiceInstanceInfo>();
            }
        }

        UpdateConfig();
        _ = WatchNacosAsync();
    }

    private async Task WatchNacosAsync()
    {
        while (true)
        {
            try
            {
                var hasChanges = false;

                foreach (var serviceName in _serviceNames)
                {
                    try
                    {
                        var instances = await _nacosNamingService.SelectInstances(
                            serviceName, _groupName, new List<string> { "DEFAULT" }, true);

                        var newList = instances.Select(i => new ServiceInstanceInfo
                        {
                            Ip = i.Ip,
                            Port = i.Port,
                            Weight = i.Weight,
                            Healthy = i.Healthy
                        }).ToList();

                        var currentList = _serviceInstances.GetValueOrDefault(serviceName, new List<ServiceInstanceInfo>());

                        if (!InstancesEqual(currentList, newList))
                        {
                            _serviceInstances[serviceName] = newList;
                            hasChanges = true;
                            Log.Information("服务 {ServiceName} 实例变化: {Count} 个实例", serviceName, newList.Count);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Warning(ex, "刷新服务 {ServiceName} 实例失败", serviceName);
                    }
                }

                if (hasChanges)
                {
                    UpdateConfig();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Nacos服务监听异常");
            }

            await Task.Delay(TimeSpan.FromSeconds(10));
        }
    }

    private static bool InstancesEqual(List<ServiceInstanceInfo> a, List<ServiceInstanceInfo> b)
    {
        if (a.Count != b.Count) return false;

        var setA = new HashSet<string>(a.Select(i => $"{i.Ip}:{i.Port}"));
        var setB = new HashSet<string>(b.Select(i => $"{i.Ip}:{i.Port}"));

        return setA.SetEquals(setB);
    }

    private void UpdateConfig()
    {
        var routes = new List<RouteConfig>();
        var clusters = new List<ClusterConfig>();

        foreach (var serviceName in _serviceNames)
        {
            var cityCode = serviceName.Replace("ServiceCity", "city").ToLowerInvariant();
            var routeId = $"{serviceName}Route";
            var clusterId = serviceName;

            routes.Add(new RouteConfig
            {
                RouteId = routeId,
                ClusterId = clusterId,
                Match = new RouteMatch
                {
                    Path = $"/api/{cityCode}/{{**catch-all}}"
                },
                Transforms = new[]
                {
                    new Dictionary<string, string>
                    {
                        { "PathRemovePrefix", $"/api/{cityCode}" }
                    }
                }
            });

            var instances = _serviceInstances.GetValueOrDefault(serviceName, new List<ServiceInstanceInfo>());
            var destinations = new Dictionary<string, DestinationConfig>();

            for (int i = 0; i < instances.Count; i++)
            {
                var instance = instances[i];
                destinations[$"{serviceName}_{i + 1}"] = new DestinationConfig
                {
                    Address = $"http://{instance.Ip}:{instance.Port}"
                };
            }

            clusters.Add(new ClusterConfig
            {
                ClusterId = clusterId,
                Destinations = destinations,
                HealthCheck = new HealthCheckConfig
                {
                    Active = new ActiveHealthCheckConfig
                    {
                        Enabled = true,
                        Interval = TimeSpan.FromSeconds(10),
                        Timeout = TimeSpan.FromSeconds(5),
                        Policy = "ConsecutiveFailures",
                        Path = "/health"
                    }
                }
            });
        }

        var oldConfig = _config;
        _config = new NacosYarpConfig(routes, clusters);

        Log.Information("YARP配置已更新: {RouteCount} 条路由, {ClusterCount} 个集群", routes.Count, clusters.Count);

        oldConfig.SignalChange();
    }

    private class ServiceInstanceInfo
    {
        public string Ip { get; set; } = null!;
        public int Port { get; set; }
        public double Weight { get; set; }
        public bool Healthy { get; set; }
    }

    private class NacosYarpConfig : IProxyConfig
    {
        private readonly CancellationTokenSource _cts = new();

        public NacosYarpConfig(IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters)
        {
            Routes = routes;
            Clusters = clusters;
            ChangeToken = new CancellationChangeToken(_cts.Token);
        }

        public IReadOnlyList<RouteConfig> Routes { get; }
        public IReadOnlyList<ClusterConfig> Clusters { get; }
        public IChangeToken ChangeToken { get; }

        public void SignalChange()
        {
            _cts.Cancel();
        }
    }
}
