namespace MyThreadPool;

/// <summary>
/// Interface that implements task for threadpool.
/// </summary>
/// <typeparam name="TResult">Type of task result.</typeparam>
public interface IMyTask<TResult>
{
    /// <summary>
    /// Gets a value indicating whether task is completed.
    /// </summary>
    public bool IsCompleted { get; }

    /// <summary>
    /// Gets result of the task or null if task is still calculating.
    /// </summary>
    public TResult? Result { get; }

    /// <summary>
    /// Sets function to continue task with.
    /// </summary>
    /// <typeparam name="TNewResult">Type of new function result.</typeparam>
    /// <param name="nextFunction">Function, to continue task with.</param>
    /// <returns>Task with applied function.</returns>
    public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult?, TNewResult> nextFunction);
}
