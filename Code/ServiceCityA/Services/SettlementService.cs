using System;
using System.Threading.Tasks;
using Common.Core.Entities;
using Common.Core.Models;
using Common.Core.Repositories;
using Common.Core.Services;
using Common.Core.Tools;
using Polly;
using Serilog;

namespace ServiceCityA.Services;

public class SettlementService
{
    private readonly IRepository<SettlementRecord> _settlementRepository;
    private readonly IThirdPartyMockService _thirdPartyService;
    private readonly IAsyncPolicy _circuitBreakerPolicy;
    private readonly IAsyncPolicy _retryPolicy;
    private readonly IAsyncPolicy _timeoutPolicy;
    private readonly string _cityCode;

    public SettlementService(IRepository<SettlementRecord> settlementRepository,
        IThirdPartyMockService thirdPartyService,
        string cityCode)
    {
        _settlementRepository = settlementRepository;
        _thirdPartyService = thirdPartyService;
        _cityCode = cityCode;
        
        _circuitBreakerPolicy = Policy
            .Handle<Exception>()
            .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
        
        _retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        
        _timeoutPolicy = Policy.TimeoutAsync(TimeSpan.FromSeconds(10));
    }

    public async Task<SettlementResponse> ProcessSettlement(SettlementRequest request)
    {
        Log.Information($"开始处理{{{_cityCode}}}结算请求，患者ID：{request.PatientId}");
        
        var recordNo = IdGenerator.GenerateRecordNo(_cityCode);
        
        var combinedPolicy = Policy.WrapAsync(_circuitBreakerPolicy, _retryPolicy, _timeoutPolicy);
        
        string thirdPartyResponse;
        try
        {
            thirdPartyResponse = await combinedPolicy.ExecuteAsync(async () =>
                await _thirdPartyService.CallMedicalInsuranceService(request, _cityCode));
            
            Log.Information($"{{{_cityCode}}}第三方医保接口调用成功，记录号：{recordNo}");
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"{{{_cityCode}}}第三方医保接口调用失败，记录号：{recordNo}");
            
            var failedRecord = new SettlementRecord
            {
                RecordNo = recordNo,
                CityCode = _cityCode,
                PatientId = request.PatientId,
                PatientName = request.PatientName,
                TotalAmount = request.TotalAmount,
                SettlementStatus = "FAILED",
                ErrorMessage = ex.Message,
                ThirdPartyResponse = null
            };
            
            await _settlementRepository.AddAsync(failedRecord);
            await _settlementRepository.SaveChangesAsync();
            
            throw;
        }
        
        var medicalAmount = request.TotalAmount * 0.7m;
        var selfPayAmount = request.TotalAmount - medicalAmount;
        
        var record = new SettlementRecord
        {
            RecordNo = recordNo,
            CityCode = _cityCode,
            PatientId = request.PatientId,
            PatientName = request.PatientName,
            TotalAmount = request.TotalAmount,
            MedicalAmount = Math.Round(medicalAmount, 2),
            SelfPayAmount = Math.Round(selfPayAmount, 2),
            SettlementStatus = "SUCCESS",
            ThirdPartyResponse = thirdPartyResponse
        };
        
        await _settlementRepository.AddAsync(record);
        await _settlementRepository.SaveChangesAsync();
        
        Log.Information($"{{{_cityCode}}}结算记录已保存，记录号：{recordNo}");
        
        return new SettlementResponse
        {
            RecordNo = recordNo,
            PatientId = request.PatientId,
            PatientName = request.PatientName,
            TotalAmount = request.TotalAmount,
            MedicalAmount = Math.Round(medicalAmount, 2),
            SelfPayAmount = Math.Round(selfPayAmount, 2),
            SettlementStatus = "SUCCESS",
            CityCode = _cityCode,
            SettlementTime = DateTime.Now
        };
    }

    public async Task<SettlementRecord?> GetSettlementRecord(string recordNo)
    {
        return await _settlementRepository.FirstOrDefaultAsync(r => r.RecordNo == recordNo);
    }
}