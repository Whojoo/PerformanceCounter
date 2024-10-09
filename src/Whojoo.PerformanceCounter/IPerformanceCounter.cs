namespace Whojoo.PerformanceCounter;

/// <summary>
/// Simple performance counter to monitor performance of certain steps.
/// Use RecordStep and RecordStepAsync to record the individual steps and use Report or StopAndReport to log the results in a single formatted message.
/// </summary>
public interface IPerformanceCounter
{
    /// <summary>
    /// Record the performance of a single step
    /// </summary>
    /// <param name="stepName">Name to use for the report</param>
    /// <param name="step">The step you want monitored</param>
    void RecordStep(string stepName, Action step);

    /// <summary>
    /// Record the performance of a single step
    /// </summary>
    /// <param name="stepName">Name to use for the report</param>
    /// <param name="step">The step you want monitored</param>
    /// <returns>The value produced by the step</returns>
    T RecordStep<T>(string stepName, Func<T> step);

    /// <summary>
    /// Record the performance of a single step
    /// </summary>
    /// <param name="stepName">Name to use for the report</param>
    /// <param name="step">The step you want monitored</param>
    Task RecordStepAsync(string stepName, Func<Task> step);

    /// <summary>
    /// Record the performance of a single step
    /// </summary>
    /// <param name="stepName">Name to use for the report</param>
    /// <param name="step">The step you want monitored</param>
    /// <returns>The value produced by the step</returns>
    Task<T> RecordStepAsync<T>(string stepName, Func<Task<T>> step);

    /// <summary>
    /// Stops the timer and reports the monitored performance.
    /// </summary>
    void StopAndReport();

    /// <summary>
    /// Reports the monitored performance without stopping the timer.
    /// If you record steps after this and Report() again, then you will re-report previous steps as well.
    /// </summary>
    void Report();

    /// <summary>
    /// Resets and restarts this performance counter with either the previous name or a new given name.
    /// </summary>
    /// <param name="newPerformanceCounterName">Optional: new name for the header, uses the previous one if this is kept empty</param>
    void Restart(string? newPerformanceCounterName = null);
}