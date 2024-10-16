using Microsoft.Extensions.Logging;

namespace Whojoo.PerformanceCounter.Implementation;

internal static class PerformanceCounterOptionsExtensions
{
    public static void ThrowIfInvalid(this PerformanceCounterOptions? performanceCounterOptions)
    {
#pragma warning disable CA2208
        if (performanceCounterOptions is null)
        {
            throw new ArgumentException("Options cannot be null", nameof(PerformanceCounterOptions));
        }

        if (string.IsNullOrWhiteSpace(performanceCounterOptions.PerformanceCounterName))
        {
            throw new ArgumentException(
                "Performance counter name cannot be null or whitespace",
                nameof(PerformanceCounterOptions.PerformanceCounterName));
        }

        if (performanceCounterOptions.LogLevel is < LogLevel.Trace or > LogLevel.None)
        {
            throw new ArgumentException(
                "LogLevel is out of available range",
                nameof(PerformanceCounterOptions.LogLevel));
        }
#pragma warning restore CA2208
    }
}