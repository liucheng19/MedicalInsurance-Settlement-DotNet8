using System;

namespace Common.Core.Models;

public class SettlementResponse
{
    public string RecordNo { get; set; } = null!;

    public string PatientId { get; set; } = null!;

    public string PatientName { get; set; } = null!;

    public decimal TotalAmount { get; set; }

    public decimal MedicalAmount { get; set; }

    public decimal SelfPayAmount { get; set; }

    public string SettlementStatus { get; set; } = null!;

    public string CityCode { get; set; } = null!;

    public DateTime SettlementTime { get; set; } = DateTime.Now;
}