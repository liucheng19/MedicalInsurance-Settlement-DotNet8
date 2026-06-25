using System;
using Gateway.Yarp;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nacos.V2;
using Nacos.V2.DependencyInjection;
using Serilog;
using Yarp.ReverseProxy.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddNacosV2Naming(options =>
{
    var config = builder.Configuration.GetSection("nacos");
    options.ServerAddresses = config.GetSection("ServerAddresses").Get<List<string>>() ?? new List<string> { "http://localhost:8848" };
    options.Namespace = config["Namespace"] ?? "dev";
    options.UserName = config["UserName"] ?? "nacos";
    options.Password = config["Password"] ?? "nacos";
    options.ListenInterval = 10000;
    options.NamingUseRpc = false;
});

builder.Services.AddSingleton<IProxyConfigProvider, NacosYarpConfigProvider>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var nacosNaming = sp.GetRequiredService<INacosNamingService>();
    var namespaceId = config["nacos:Namespace"] ?? "dev";
    var groupName = config["nacos:GroupName"] ?? "DEFAULT_GROUP";
    var services = new[] { "ServiceCityA", "ServiceCityB", "ServiceCityC" };
    var provider = new NacosYarpConfigProvider(nacosNaming, namespaceId, groupName, services);
    _ = provider.InitializeAsync();
    return provider;
});

builder.Services.AddReverseProxy();

builder.Services.AddHealthChecks();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSerilogRequestLogging();

app.MapReverseProxy();

app.MapHealthChecks("/health");

Log.Information("Gateway.Yarp 网关启动完成，监听端口 5000");

app.Run();
