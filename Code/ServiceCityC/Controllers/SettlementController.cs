using System;
using System.Threading.Tasks;
using Common.Core.Entities;
using Common.Core.Models;
using Microsoft.AspNetCore.Mvc;
using ServiceCityC.Services;
using Serilog;

namespace ServiceCityC.Controllers;

[ApiController]
[Route("api/settlement")]
public class SettlementController : ControllerBase
{
    private readonly SettlementService _settlementService;

    public SettlementController(SettlementService settlementService)
    {
        _settlementService = settlementService;
    }

    [HttpPost("submit")]
    public async Task<IActionResult> SubmitSettlement([FromBody] SettlementRequest request)
    {
        try
        {
            var response = await _settlementService.ProcessSettlement(request);
            return Ok(Result<SettlementResponse>.Ok(response));
        }
        catch (Exception ex)
        {
            Log.Error(ex, "结算处理异常");
            return Ok(Result<SettlementResponse>.Fail(ex.Message));
        }
    }

    [HttpGet("{recordNo}")]
    public async Task<IActionResult> GetSettlementRecord(string recordNo)
    {
        try
        {
            var record = await _settlementService.GetSettlementRecord(recordNo);
            if (record == null)
            {
                return Ok(Result<SettlementRecord>.Fail(404, "结算记录不存在"));
            }
            return Ok(Result<SettlementRecord>.Ok(record));
        }
        catch (Exception ex)
        {
            Log.Error(ex, "查询结算记录异常");
            return Ok(Result<SettlementRecord>.Fail(ex.Message));
        }
    }
}