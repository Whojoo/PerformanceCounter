using System.Diagnostics;
using System.Text;

using Microsoft.Extensions.Logging;

namespace Whojoo.PerformanceCounter.Implementation;

/// <inheritdoc />
internal sealed class PerformanceCounter(
    ILogger logger,
    string performanceCounterName,
    LogLevel logLevel = LogLevel.Information)
    : IPerformanceCounter
{
    private const string HeaderMessage = "Starting performance counter {PerformanceCounterName}\n";

    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
    private readonly ILogger _logger = logger;

    private readonly string _performanceCounterName = performanceCounterName;
    private readonly LogLevel _logLevel = logLevel;

    private readonly StringBuilder _logBuilder = new StringBuilder().Append(HeaderMessage);
    private readonly List<object[]> _arguments = [[performanceCounterName]];

    /// <inheritdoc />
    public void RecordStep(string stepName, Action step)
    {
        var startElapsed = _stopwatch.Elapsed;
        step();
        var endElapsed = _stopwatch.Elapsed;

        var elapsed = endElapsed - startElapsed;

        _logBuilder.Append($"- Step {{{stepName}}}: {{{stepName}ElapsedMilliseconds}} ms\n");

        _arguments.Add([stepName, elapsed.TotalMilliseconds]);
    }

    /// <inheritdoc />
    public T RecordStep<T>(string stepName, Func<T> step)
    {
        var startElapsed = _stopwatch.Elapsed;
        var result = step();
        var endElapsed = _stopwatch.Elapsed;

        var elapsed = endElapsed - startElapsed;

        _logBuilder.Append($"- Step {{{stepName}}}: {{{stepName}ElapsedMilliseconds}} ms\n");

        _arguments.Add([stepName, elapsed.TotalMilliseconds]);

        return result;
    }

    /// <inheritdoc />
    public async Task RecordStepAsync(string stepName, Func<Task> step)
    {
        var startElapsed = _stopwatch.Elapsed;
        await step();
        var endElapsed = _stopwatch.Elapsed;

        var elapsed = endElapsed - startElapsed;

        _logBuilder.Append($"- Step {{{stepName}}}: {{{stepName}ElapsedMilliseconds}} ms\n");

        _arguments.Add([stepName, elapsed.TotalMilliseconds]);
    }

    /// <inheritdoc />
    public async Task<T> RecordStepAsync<T>(string stepName, Func<Task<T>> step)
    {
        var startElapsed = _stopwatch.Elapsed;
        var result = await step();
        var endElapsed = _stopwatch.Elapsed;

        var elapsed = endElapsed - startElapsed;

        _logBuilder.Append($"- Step {{{stepName}}}: {{{stepName}ElapsedMilliseconds}} ms\n");

        _arguments.Add([stepName, elapsed.TotalMilliseconds]);

        return result;
    }

    /// <inheritdoc />
    public void StopAndReport()
    {
        _stopwatch.Stop();
        Report();
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
    public void Restart(string? newPerformanceCounterName = null)
    {
        _stopwatch.Restart();
        _logBuilder.Clear().Append(HeaderMessage);
        _arguments.Clear();
        _arguments.Add([newPerformanceCounterName ?? _performanceCounterName]);
    }
}