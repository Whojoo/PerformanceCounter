using Microsoft.Extensions.Logging;

namespace Whojoo.PerformanceCounter;

public static class PerformanceCounterFactory
{
    /// <summary>
    /// Create and start a default performance counter. It wil report its results as LogLevel Information.
    /// </summary>
    /// <param name="logger">Logger used for reporting</param>
    /// <param name="performanceCounterName">Name used as the header in the report</param>
    /// <returns>A new and running performance counter</returns>
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
    public static IPerformanceCounter StartWithLogLevel(LogLevel logLevel, ILogger logger, string performanceCounterName)
    {
        return new Implementation.PerformanceCounter(logger, performanceCounterName, logLevel);
    }
}