using System;
using Nacos.V2.Naming.Dtos;

namespace Common.Core.Extensions;

public static class NacosExtensions
{
    public static Instance CreateInstance(string ip, int port, string serviceName)
    {
        return new Instance
        {
            Ip = ip,
            Port = port,
            Weight = 1.0,
            Healthy = true,
            Enabled = true,
            Metadata = new System.Collections.Generic.Dictionary<string, string>
            {
                { "serviceName", serviceName },
                { "port", port.ToString() }
            }
        };
    }

    public static string GetLocalIpAddress()
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
}