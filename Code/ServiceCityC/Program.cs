using System;
using Common.Core.DbContexts;
using Common.Core.Entities;
using Common.Core.Extensions;
using Common.Core.Repositories;
using Common.Core.Services;
using Common.Core.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nacos.AspNetCore.V2;
using Serilog;
using ServiceCityC.Filters;
using ServiceCityC.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddDbContext<MedicalInsuranceDbContext>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 30)),
        b => b.MigrationsAssembly("Common.Core")
    );
});

builder.Services.AddNacosAspNet(builder.Configuration, "nacos");

builder.Services.AddValidatorsFromAssemblyContaining<SettlementRequestValidator>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
});

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IThirdPartyMockService, ThirdPartyMockService>();
builder.Services.AddScoped(sp => new SettlementService(
    sp.GetRequiredService<IRepository<Common.Core.Entities.SettlementRecord>>(),
    sp.GetRequiredService<IThirdPartyMockService>(),
    builder.Configuration["CityCode"] ?? "CITYC"));
builder.Services.AddScoped(sp => new HeartbeatService(
    sp.GetRequiredService<IRepository<Common.Core.Entities.Heartbeat>>(),
    builder.Configuration["ServiceName"] ?? "ServiceCityC",
    GetLocalIpAddress(),
    5003));

builder.Services.AddHostedService<HeartbeatBackgroundService>();

builder.Services.AddHealthChecks();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSerilogRequestLogging();

app.UseRouting();

app.MapControllers();

app.MapHealthChecks("/health");

app.Run();

string GetLocalIpAddress()
{
    var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
    foreach (var ip in host.AddressList)
    {
        if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
        {
            return ip.ToString();
        }
    }
    return "127.0.0.1";
}