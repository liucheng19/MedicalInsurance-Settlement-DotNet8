using System;
using Serilog;
using Serilog.Events;

namespace Common.Core.Extensions;

public static class SerilogExtensions
{
    public static void ConfigureSerilog(string serviceName, string connectionString)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("ServiceName", serviceName)
            .WriteTo.Console()
            .WriteTo.File(
                path: $"Logs/{serviceName}/log-.txt",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 30,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {ServiceName} {Message:lj}{NewLine}{Exception}"
            )
            .CreateLogger();
    }
}