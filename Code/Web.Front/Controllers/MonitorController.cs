using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Common.Core.Entities;
using Common.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Web.Front.Controllers;

public class MonitorController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<MonitorController> _logger;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public MonitorController(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<MonitorController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var services = new Dictionary<string, string>
        {
            { "ServiceCityA", _configuration["ServiceUrls:ServiceCityA"] ?? "" },
            { "ServiceCityB", _configuration["ServiceUrls:ServiceCityB"] ?? "" },
            { "ServiceCityC", _configuration["ServiceUrls:ServiceCityC"] ?? "" }
        };

        // 并行查询所有服务，最长等待 5 秒
        var tasks = services.Select(kv => CheckServiceAsync(kv.Key, kv.Value)).ToList();
        var serviceStatusList = (await Task.WhenAll(tasks)).ToList();

        return View(serviceStatusList);
    }

    private async Task<ServiceStatus> CheckServiceAsync(string serviceName, string baseUrl)
    {
        var status = new ServiceStatus { ServiceName = serviceName };

        if (string.IsNullOrEmpty(baseUrl))
        {
            status.IsHealthy = false;
            status.HeartbeatStatus = "UNKNOWN";
            return status;
        }

        var client = _httpClientFactory.CreateClient("MonitorClient");

        // 1. 检查健康状态（带超时）
        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
            var healthTask = client.GetAsync($"{baseUrl}/health", cts.Token);
            var completed = await Task.WhenAny(healthTask, Task.Delay(TimeSpan.FromSeconds(2), cts.Token));

            if (completed == healthTask)
            {
                var healthResponse = await healthTask;
                status.IsHealthy = healthResponse.IsSuccessStatusCode;
            }
            else
            {
                status.IsHealthy = false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "健康检查失败: {Service}", serviceName);
            status.IsHealthy = false;
        }

        // 2. 获取心跳数据（仅在服务健康时）
        if (status.IsHealthy)
        {
            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
                var heartbeatTask = client.GetAsync($"{baseUrl}/api/heartbeat/status", cts.Token);
                var completed = await Task.WhenAny(heartbeatTask, Task.Delay(TimeSpan.FromSeconds(2), cts.Token));

                if (completed == heartbeatTask)
                {
                    var heartbeatResponse = await heartbeatTask;
                    if (heartbeatResponse.IsSuccessStatusCode)
                    {
                        var content = await heartbeatResponse.Content.ReadAsStringAsync();
                        var result = JsonSerializer.Deserialize<Result<IEnumerable<Heartbeat>>>(content, _jsonOptions);

                        var heartbeat = result?.Data?
                            .Where(h => h != null && h.ServiceName == serviceName)
                            .OrderByDescending(h => h.LastHeartbeatTime)
                            .FirstOrDefault();

                        if (heartbeat != null)
                        {
                            status.HeartbeatStatus = heartbeat.Status;
                            status.LastHeartbeatTime = heartbeat.LastHeartbeatTime;
                            status.ServiceIp = heartbeat.ServiceIp;
                            status.ServicePort = heartbeat.ServicePort;
                        }
                        else
                        {
                            status.HeartbeatStatus = "ACTIVE";
                        }
                    }
                    else
                    {
                        status.HeartbeatStatus = "ACTIVE";
                    }
                }
                else
                {
                    status.HeartbeatStatus = "ACTIVE";
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "心跳状态获取失败: {Service}", serviceName);
                status.HeartbeatStatus = "ACTIVE";
            }
        }
        else
        {
            status.HeartbeatStatus = "OFFLINE";
        }

        return status;
    }
}

public class ServiceStatus
{
    public string ServiceName { get; set; } = null!;
    public bool IsHealthy { get; set; }
    public string HeartbeatStatus { get; set; } = "UNKNOWN";
    public DateTime? LastHeartbeatTime { get; set; }
    public string? ServiceIp { get; set; }
    public int? ServicePort { get; set; }
}
