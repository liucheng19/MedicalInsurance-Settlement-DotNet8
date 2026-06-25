using System;
using System.Threading.Tasks;
using Common.Core.Entities;
using Common.Core.Models;
using Common.Core.Repositories;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ServiceCityA.Controllers;

[ApiController]
[Route("api/heartbeat")]
public class HeartbeatController : ControllerBase
{
    private readonly IRepository<Heartbeat> _heartbeatRepository;

    public HeartbeatController(IRepository<Heartbeat> heartbeatRepository)
    {
        _heartbeatRepository = heartbeatRepository;
    }

    [HttpGet("status")]
    public async Task<IActionResult> GetHeartbeatStatus()
    {
        try
        {
            var heartbeats = await _heartbeatRepository.GetAllAsync();
            return Ok(Result<IEnumerable<Heartbeat>>.Ok(heartbeats));
        }
        catch (Exception ex)
        {
            Log.Error(ex, "获取心跳状态异常");
            return Ok(Result<IEnumerable<Heartbeat>>.Fail(ex.Message));
        }
    }
}