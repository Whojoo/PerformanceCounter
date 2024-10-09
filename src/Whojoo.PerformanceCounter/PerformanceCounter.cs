using System.Diagnostics;
using System.Text;

using Microsoft.Extensions.Logging;

namespace Whojoo.PerformanceCounter;

/// <summary>
/// Simple performance counter to monitor performance of certain steps.
/// Use RecordStep and RecordStepAsync to record the individual steps and use Report or StopAndReport to log the results in a single formatted message.
/// </summary>
/// <param name="logger">The ILogger used to log the report to</param>
/// <param name="performanceCounterName">Name to use for the header of the report</param>
/// <param name="logLevel">Optional: Log level to use for the logger, defaults to Information</param>
public sealed class PerformanceCounter(
    ILogger logger,
    string performanceCounterName,
    LogLevel logLevel = LogLevel.Information)
{
    private const string HeaderMessage = "Starting performance counter {PerformanceCounterName}\n";

    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
    private readonly ILogger _logger = logger;

    private readonly string _performanceCounterName = performanceCounterName;
    private readonly LogLevel _logLevel = logLevel;

    private readonly StringBuilder _logBuilder = new StringBuilder().Append(HeaderMessage);
    private readonly List<object[]> _arguments = [[performanceCounterName]];

    /// <summary>
    /// Record the performance of a single step
    /// </summary>
    /// <param name="stepName">Name to use for the report</param>
    /// <param name="step">The step you want monitored</param>
    /// <returns>This instance in case you want to chain calls</returns>
    public PerformanceCounter RecordStep(string stepName, Action step)
    {
        var startElapsed = _stopwatch.Elapsed;
        step();
        var endElapsed = _stopwatch.Elapsed;

        var elapsed = endElapsed - startElapsed;

        _logBuilder.Append($"- Step {{{stepName}}}: {{{stepName}ElapsedMilliseconds}} ms\n");

        _arguments.Add([stepName, elapsed.TotalMilliseconds]);

        return this;
    }

    /// <summary>
    /// Record the performance of a single async step
    /// </summary>
    /// <param name="stepName">Name to use for the report</param>
    /// <param name="step">The step you want monitored</param>
    /// <returns>This instance in case you want to chain calls</returns>
    public async Task<PerformanceCounter> RecordStepAsync(string stepName, Func<Task> step)
    {
        var startElapsed = _stopwatch.Elapsed;
        await step();
        var endElapsed = _stopwatch.Elapsed;

        var elapsed = endElapsed - startElapsed;

        _logBuilder.Append($"- Step {{{stepName}}}: {{{stepName}ElapsedMilliseconds}} ms\n");

        _arguments.Add([stepName, elapsed.TotalMilliseconds]);

        return this;
    }

    /// <summary>
    /// Stops the timer and reports the monitored performance.
    /// </summary>
    public void StopAndReport()
    {
        _stopwatch.Stop();
        Report();
    }

    /// <summary>
    /// Reports the monitored performance without stopping the timer.
    /// If you record steps after this and Report() again, then you will re-report previous steps as well.
    /// </summary>
    public void Report()
    {
        var totalElapsed = _stopwatch.Elapsed;

        _logBuilder.Append($"- Total elapsed: {totalElapsed.TotalMilliseconds} ms");
        _arguments.Add([totalElapsed.TotalMilliseconds]);

        var messageFormat = _logBuilder.ToString();
        var arguments = _arguments.SelectMany(argumentArray => argumentArray).ToArray();

#pragma warning disable CA2254
        _logger.Log(_logLevel, messageFormat, arguments);
#pragma warning restore CA2254
    }

    /// <summary>
    /// Resets and restarts this performance counter with either the previous name or a new given name.
    /// </summary>
    /// <param name="newPerformanceCounterName">Optional: new name for the header, uses the previous one if this is kept empty</param>
    public void Restart(string? newPerformanceCounterName = null)
    {
        _stopwatch.Restart();
        _logBuilder.Clear().Append(HeaderMessage);
        _arguments.Clear();
        _arguments.Add([newPerformanceCounterName ?? _performanceCounterName]);
    }
}