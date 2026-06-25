using System;

namespace Common.Core.Models;

public class SettlementRequest
{
    public string PatientId { get; set; } = null!;

    public string PatientName { get; set; } = null!;

    public string PatientCardNo { get; set; } = null!;

    public decimal TotalAmount { get; set; }

    public string DiagnosisCode { get; set; } = null!;

    public string DiagnosisName { get; set; } = null!;

    public DateTime SettlementDate { get; set; } = DateTime.Now;

    public string HospitalCode { get; set; } = null!;

    public string HospitalName { get; set; } = null!;
}