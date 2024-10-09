using Microsoft.Extensions.Logging;

namespace Whojoo.PerformanceCounter;

public static class PerformanceCounterFactory
{
    public static IPerformanceCounter StartDefault(ILogger logger, string performanceCounterName)
    {
        return new Implementation.PerformanceCounter(logger, performanceCounterName);
    }

    public static IPerformanceCounter StartWithLogLevel(LogLevel logLevel, ILogger logger, string performanceCounterName)
    {
        return new Implementation.PerformanceCounter(logger, performanceCounterName, logLevel);
    }
}