using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace ServiceCityB.Services;

public class HeartbeatBackgroundService : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;

    public HeartbeatBackgroundService(IConfiguration configuration, IServiceProvider serviceProvider)
    {
        _configuration = configuration;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Log.Information("心跳后台服务已启动");
        
        var intervalSeconds = _configuration.GetValue<int>("Heartbeat:IntervalSeconds", 10);
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var heartbeatService = scope.ServiceProvider.GetRequiredService<HeartbeatService>();
                await heartbeatService.SendHeartbeat();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "心跳发送异常");
            }
            
            await Task.Delay(TimeSpan.FromSeconds(intervalSeconds), stoppingToken);
        }
        
        Log.Information("心跳后台服务已停止");
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var heartbeatService = scope.ServiceProvider.GetRequiredService<HeartbeatService>();
            await heartbeatService.UpdateStatus("OFFLINE");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "服务停止时更新状态失败");
        }
        
        await base.StopAsync(stoppingToken);
    }
}