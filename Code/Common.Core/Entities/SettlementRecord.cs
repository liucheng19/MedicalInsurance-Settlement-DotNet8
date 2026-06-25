using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Core.Entities;

[Table("settlement_record")]
public class SettlementRecord
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("record_no")]
    [Required]
    [StringLength(50)]
    public string RecordNo { get; set; } = null!;

    [Column("city_code")]
    [Required]
    [StringLength(20)]
    public string CityCode { get; set; } = null!;

    [Column("patient_id")]
    [Required]
    [StringLength(50)]
    public string PatientId { get; set; } = null!;

    [Column("patient_name")]
    [Required]
    [StringLength(100)]
    public string PatientName { get; set; } = null!;

    [Column("total_amount")]
    [Required]
    public decimal TotalAmount { get; set; }

    [Column("medical_amount")]
    public decimal MedicalAmount { get; set; }

    [Column("self_pay_amount")]
    public decimal SelfPayAmount { get; set; }

    [Column("settlement_status")]
    [Required]
    [StringLength(20)]
    public string SettlementStatus { get; set; } = null!;

    [Column("third_party_response", TypeName = "text")]
    public string? ThirdPartyResponse { get; set; }

    [Column("error_message", TypeName = "text")]
    public string? ErrorMessage { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}