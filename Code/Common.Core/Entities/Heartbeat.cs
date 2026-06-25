using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Core.Entities;

[Table("heartbeat")]
public class Heartbeat
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("service_name")]
    [Required]
    [StringLength(50)]
    public string ServiceName { get; set; } = null!;

    [Column("service_ip")]
    [Required]
    [StringLength(50)]
    public string ServiceIp { get; set; } = null!;

    [Column("service_port")]
    [Required]
    public int ServicePort { get; set; }

    [Column("status")]
    [Required]
    [StringLength(20)]
    public string Status { get; set; } = null!;

    [Column("last_heartbeat_time")]
    [Required]
    public DateTime LastHeartbeatTime { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}