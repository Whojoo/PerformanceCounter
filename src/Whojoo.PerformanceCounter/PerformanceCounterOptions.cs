using Microsoft.Extensions.Logging;

namespace Whojoo.PerformanceCounter;

public sealed class PerformanceCounterOptions
{
    /// <summary>
    /// Set the LogLevel for reporting.
    ///
    /// Default: Information
    /// </summary>
    public LogLevel LogLevel { get; set; } = LogLevel.Information;

    /// <summary>
    /// Enable or disable the performance counter, useful when used in combination with a feature switch.
    ///
    /// Default: true
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Set the name used for the performance counter's reporting.
    ///
    /// Default: PerformanceTest
    /// </summary>
    public string PerformanceCounterName { get; set; } = "PerformanceTest";
}