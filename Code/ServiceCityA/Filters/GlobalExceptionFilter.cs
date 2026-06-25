using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Common.Core.Entities;
using Common.Core.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;

namespace ServiceCityA.Filters;

public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly IRepository<ExceptionLog> _exceptionRepository;

    public GlobalExceptionFilter(IRepository<ExceptionLog> exceptionRepository)
    {
        _exceptionRepository = exceptionRepository;
    }

    public void OnException(ExceptionContext context)
    {
        var exception = context.Exception;
        
        Log.Error(exception, "未处理异常：{Message}", exception.Message);

        var exceptionLog = new ExceptionLog
        {
            ServiceName = "ServiceCityA",
            ExceptionType = exception.GetType().FullName ?? "Unknown",
            ExceptionMessage = exception.Message,
            StackTrace = exception.StackTrace,
            RequestPath = context.HttpContext.Request.Path,
            RequestMethod = context.HttpContext.Request.Method
        };

        try
        {
            using var reader = new StreamReader(context.HttpContext.Request.Body);
            exceptionLog.RequestBody = reader.ReadToEnd();
        }
        catch
        {
            exceptionLog.RequestBody = null;
        }

        try
        {
            _exceptionRepository.AddAsync(exceptionLog).Wait();
            _exceptionRepository.SaveChangesAsync().Wait();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "保存异常日志失败");
        }

        context.Result = new JsonResult(new
        {
            Success = false,
            Code = 500,
            Message = "系统内部错误，请稍后重试"
        })
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };

        context.ExceptionHandled = true;
    }
}