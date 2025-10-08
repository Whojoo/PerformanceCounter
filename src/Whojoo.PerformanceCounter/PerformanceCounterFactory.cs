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

        if (!options.IsEnabled || options.LogLevel is LogLevel.None || !logger.IsEnabled(options.LogLevel))
        {
            return new NoopPerformanceCounter();
        }

        return new Implementation.PerformanceCounter(logger, options.PerformanceCounterName, options.LogLevel);
    }
}