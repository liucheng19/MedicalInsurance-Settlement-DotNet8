using System;
using System.Threading.Tasks;
using Common.Core.Models;

namespace Common.Core.Services;

public interface IThirdPartyMockService
{
    Task<string> CallMedicalInsuranceService(SettlementRequest request, string cityCode);
}

public class ThirdPartyMockService : IThirdPartyMockService
{
    public async Task<string> CallMedicalInsuranceService(SettlementRequest request, string cityCode)
    {
        await Task.Delay(new Random().Next(100, 500));

        var successRate = 0.95;
        if (new Random().NextDouble() > successRate)
        {
            throw new Exception("第三方医保接口调用失败");
        }

        var medicalRate = 0.7;
        var medicalAmount = Math.Round(request.TotalAmount * (decimal)medicalRate, 2);
        var selfPayAmount = Math.Round(request.TotalAmount - medicalAmount, 2);

        return $"{{\"success\":true,\"cityCode\":\"{cityCode}\",\"medicalAmount\":{medicalAmount},\"selfPayAmount\":{selfPayAmount},\"message\":\"结算成功\"}}";
    }
}