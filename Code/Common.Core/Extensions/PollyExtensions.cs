using System;
using Polly;

namespace Common.Core.Extensions;

public static class PollyExtensions
{
    public static IAsyncPolicy CreateCircuitBreakerPolicy(int exceptionsAllowedBeforeBreaking = 5, TimeSpan durationOfBreak = default)
    {
        if (durationOfBreak == default)
        {
            durationOfBreak = TimeSpan.FromSeconds(30);
        }

        return Policy
            .Handle<Exception>()
            .CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking,
                durationOfBreak,
                onBreak: (exception, timespan) =>
                {
                    Console.WriteLine($"Circuit breaker opened for {timespan.TotalSeconds} seconds due to: {exception.Message}");
                },
                onReset: () =>
                {
                    Console.WriteLine("Circuit breaker reset");
                },
                onHalfOpen: () =>
                {
                    Console.WriteLine("Circuit breaker half-open, testing...");
                });
    }

    public static IAsyncPolicy CreateRetryPolicy(int retryCount = 3)
    {
        return Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }

    public static IAsyncPolicy CreateTimeoutPolicy(TimeSpan timeout)
    {
        return Policy.TimeoutAsync(timeout);
    }
}