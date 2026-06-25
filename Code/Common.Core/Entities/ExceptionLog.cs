using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Core.Entities;

[Table("exception_log")]
public class ExceptionLog
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("service_name")]
    [Required]
    [StringLength(50)]
    public string ServiceName { get; set; } = null!;

    [Column("exception_type")]
    [Required]
    [StringLength(200)]
    public string ExceptionType { get; set; } = null!;

    [Column("exception_message", TypeName = "text")]
    [Required]
    public string ExceptionMessage { get; set; } = null!;

    [Column("stack_trace", TypeName = "text")]
    public string? StackTrace { get; set; }

    [Column("request_path")]
    [StringLength(500)]
    public string? RequestPath { get; set; }

    [Column("request_method")]
    [StringLength(20)]
    public string? RequestMethod { get; set; }

    [Column("request_body", TypeName = "text")]
    public string? RequestBody { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}