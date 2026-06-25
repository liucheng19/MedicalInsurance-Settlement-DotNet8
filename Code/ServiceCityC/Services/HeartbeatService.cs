using System;
using System.Threading.Tasks;
using Common.Core.Entities;
using Common.Core.Repositories;
using Serilog;

namespace ServiceCityC.Services;

public class HeartbeatService
{
    private readonly IRepository<Heartbeat> _heartbeatRepository;
    private readonly string _serviceName;
    private readonly string _serviceIp;
    private readonly int _servicePort;

    public HeartbeatService(IRepository<Heartbeat> heartbeatRepository, 
        string serviceName, string serviceIp, int servicePort)
    {
        _heartbeatRepository = heartbeatRepository;
        _serviceName = serviceName;
        _serviceIp = serviceIp;
        _servicePort = servicePort;
    }

    public async Task SendHeartbeat()
    {
        try
        {
            var existingHeartbeat = await _heartbeatRepository.FirstOrDefaultAsync(
                h => h.ServiceName == _serviceName);

            if (existingHeartbeat != null)
            {
                existingHeartbeat.Status = "ONLINE";
                existingHeartbeat.LastHeartbeatTime = DateTime.Now;
                existingHeartbeat.UpdatedAt = DateTime.Now;
                _heartbeatRepository.Update(existingHeartbeat);
            }
            else
            {
                var newHeartbeat = new Heartbeat
                {
                    ServiceName = _serviceName,
                    ServiceIp = _serviceIp,
                    ServicePort = _servicePort,
                    Status = "ONLINE",
                    LastHeartbeatTime = DateTime.Now
                };
                await _heartbeatRepository.AddAsync(newHeartbeat);
            }

            await _heartbeatRepository.SaveChangesAsync();
            Log.Information($"{_serviceName} 心跳发送成功");
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"{_serviceName} 心跳发送失败");
            throw;
        }
    }

    public async Task UpdateStatus(string status)
    {
        var existingHeartbeat = await _heartbeatRepository.FirstOrDefaultAsync(
            h => h.ServiceName == _serviceName);

        if (existingHeartbeat != null)
        {
            existingHeartbeat.Status = status;
            existingHeartbeat.UpdatedAt = DateTime.Now;
            _heartbeatRepository.Update(existingHeartbeat);
            await _heartbeatRepository.SaveChangesAsync();
        }
    }
}