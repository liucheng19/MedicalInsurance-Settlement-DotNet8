using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Common.Core.DbContexts;

public class MedicalInsuranceDbContextFactory : IDesignTimeDbContextFactory<MedicalInsuranceDbContext>
{
    public MedicalInsuranceDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MedicalInsuranceDbContext>();
        optionsBuilder.UseMySql("Server=localhost;Port=3306;Database=medical_insurance;Uid=root;Pwd=12345678;CharSet=utf8mb4;",
            new MySqlServerVersion(new Version(8, 0, 30)));

        return new MedicalInsuranceDbContext(optionsBuilder.Options);
    }
}