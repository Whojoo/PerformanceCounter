using Microsoft.Extensions.Logging;

using Whojoo.PerformanceCounter.Implementation;

namespace Whojoo.PerformanceCounter;

public static class PerformanceCounterFactory
{
    /// <summary>
    /// Create and start a performance counter. Options are optional and default options will be used if not provided.
    /// </summary>
    /// <param name="logger">Logger used for reporting</param>
    /// <param name="performanceCounterOptions">Options used to create the counter, default is used when not provided</param>
    /// <returns>A new and running performance counter</returns>
    public static IPerformanceCounter Start(ILogger logger, PerformanceCounterOptions? performanceCounterOptions = null)
    {
        var options = performanceCounterOptions ?? new PerformanceCounterOptions();

        options.ThrowIfInvalid();

        if (!options.IsEnabled || options.LogLevel is LogLevel.None)
        {
            return new NoopPerformanceCounter();
        }

        return new Implementation.PerformanceCounter(logger, options.PerformanceCounterName, options.LogLevel);
    }

    /// <summary>
    /// Create and start a default performance counter. It wil report its results as LogLevel Information.
    /// </summary>
    /// <param name="logger">Logger used for reporting</param>
    /// <param name="performanceCounterName">Name used as the header in the report</param>
    /// <returns>A new and running performance counter</returns>
    [Obsolete("This method is obsolete and will be removed in v2, use Start instead.")]
    public static IPerformanceCounter StartDefault(ILogger logger, string performanceCounterName)
    {
        return new Implementation.PerformanceCounter(logger, performanceCounterName);
    }

    /// <summary>
    /// Create a performance counter which reports its results as a specific LogLevel.
    /// </summary>
    /// <param name="logLevel">LogLevel used for performance reporting</param>
    /// <param name="logger">Logger used for reporting</param>
    /// <param name="performanceCounterName">Name used as the header in the report</param>
    /// <returns>A new and running performance counter</returns>
    [Obsolete("This method is obsolete and will be removed in v2, use Start instead.")]
    public static IPerformanceCounter StartWithLogLevel(LogLevel logLevel, ILogger logger, string performanceCounterName)
    {
        return new Implementation.PerformanceCounter(logger, performanceCounterName, logLevel);
    }
}