using Microsoft.EntityFrameworkCore;
using Common.Core.Entities;

namespace Common.Core.DbContexts;

public class MedicalInsuranceDbContext : DbContext
{
    public MedicalInsuranceDbContext(DbContextOptions<MedicalInsuranceDbContext> options) : base(options)
    {
    }

    public DbSet<Heartbeat> Heartbeats { get; set; } = null!;

    public DbSet<SettlementRecord> SettlementRecords { get; set; } = null!;

    public DbSet<ExceptionLog> ExceptionLogs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Heartbeat>(entity =>
        {
            entity.HasIndex(e => e.ServiceName);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.LastHeartbeatTime);
        });

        modelBuilder.Entity<SettlementRecord>(entity =>
        {
            entity.HasIndex(e => e.RecordNo).IsUnique();
            entity.HasIndex(e => e.CityCode);
            entity.HasIndex(e => e.PatientId);
            entity.HasIndex(e => e.SettlementStatus);
            entity.HasIndex(e => e.CreatedAt);
        });

        modelBuilder.Entity<ExceptionLog>(entity =>
        {
            entity.HasIndex(e => e.ServiceName);
            entity.HasIndex(e => e.ExceptionType);
            entity.HasIndex(e => e.CreatedAt);
        });
    }
}