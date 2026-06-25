using System;

namespace Common.Core.Tools;

public static class IdGenerator
{
    public static string GenerateRecordNo(string cityCode)
    {
        return $"{cityCode}{DateTime.Now:yyyyMMddHHmmssfff}{new Random().Next(1000, 9999)}";
    }

    public static string GenerateId()
    {
        return Guid.NewGuid().ToString("N");
    }
}