using System;

namespace Common.Core.Models;

public class Result<T>
{
    public bool Success { get; set; }

    public int Code { get; set; }

    public string Message { get; set; } = null!;

    public T? Data { get; set; }

    public static Result<T> Ok(T data)
    {
        return new Result<T> { Success = true, Code = 200, Message = "操作成功", Data = data };
    }

    public static Result<T> Ok(string message, T data)
    {
        return new Result<T> { Success = true, Code = 200, Message = message, Data = data };
    }

    public static Result<T> Fail(string message)
    {
        return new Result<T> { Success = false, Code = 500, Message = message };
    }

    public static Result<T> Fail(int code, string message)
    {
        return new Result<T> { Success = false, Code = code, Message = message };
    }
}

public class Result
{
    public bool Success { get; set; }

    public int Code { get; set; }

    public string Message { get; set; } = null!;

    public static Result Ok()
    {
        return new Result { Success = true, Code = 200, Message = "操作成功" };
    }

    public static Result Ok(string message)
    {
        return new Result { Success = true, Code = 200, Message = message };
    }

    public static Result Fail(string message)
    {
        return new Result { Success = false, Code = 500, Message = message };
    }

    public static Result Fail(int code, string message)
    {
        return new Result { Success = false, Code = code, Message = message };
    }
}