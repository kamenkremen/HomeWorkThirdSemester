using System.Collections.Concurrent;

namespace MyThreadPool;
/// <summary>
/// Class that implements threadpool.
/// </summary>
public class MyThreadPool
{
    private readonly CancellationTokenSource tokenSource = new ();
    private readonly ConcurrentQueue<Action> taskQueue = new ();
    private readonly object lockObject = new ();
    private readonly Thread[] threads;

    /// <summary>
    /// Initializes a new instance of the <see cref="MyThreadPool"/> class.
    /// </summary>
    /// <param name="numberOfThreads">Number of working threads in the threadpool.</param>
    public MyThreadPool(int numberOfThreads)
    {
        this.NumberOfThreads = numberOfThreads;
        this.threads = new Thread[this.NumberOfThreads];

        for (var i = 0; i < this.NumberOfThreads; ++i)
        {
            this.threads[i] = new Thread(() => this.ThreadFunction(this.tokenSource.Token));
            this.threads[i].Start();
        }
    }

    /// <summary>
    /// Gets number of threads in the threadpool.
    /// </summary>
    public int NumberOfThreads { get; }

    /// <summary>
    /// Adds task, that is going to calculate function, to the threadpool.
    /// </summary>
    /// <typeparam name="TResult">Return type of the function.</typeparam>
    /// <param name="function">Function, that is going to be calculated.</param>
    /// <returns>Taks, that represents function calculation.</returns>
    /// <exception cref="TaskCanceledException">Throws if threadpool had been shutted down.</exception>
    public IMyTask<TResult> Submit<TResult>(Func<TResult> function)
    {
        if (this.tokenSource.Token.IsCancellationRequested)
        {
            throw new TaskCanceledException();
        }

        lock (this.lockObject)
        {
            var newTask = new MyTask<TResult>(function, this);
            this.taskQueue.Enqueue(newTask.Execute);
            Monitor.PulseAll(this.lockObject);
            return newTask;
        }
    }

    /// <summary>
    /// Signals threadpool to stop the work.
    /// </summary>
    public void Shutdown()
    {
        this.tokenSource.Cancel();

        foreach (var thread in this.threads)
        {
            thread.Join();
        }
    }

    private void ThreadFunction(CancellationToken token)
    {
        Action? currentAction = null;
        while (!this.taskQueue.IsEmpty)
        {
            lock (this.lockObject)
            {
                while (!this.taskQueue.IsEmpty && !this.taskQueue.TryDequeue(out currentAction))
                {
                    Monitor.Wait(this.lockObject);
                }
            }

            currentAction?.Invoke();
        }
    }

    private class MyTask<TResult>(Func<TResult> function, MyThreadPool threadPool) : IMyTask<TResult>
    {
        private readonly MyThreadPool threadPool = threadPool;
        private readonly ConcurrentQueue<Action> tasksToContinueWith = new ();
        private readonly Func<TResult> function = function;
        private readonly object lockObject = new ();
        private TResult? result;
        private Exception? exception = null;

        public bool IsCompleted { get; private set; }

        public TResult? Result
        {
            get
            {
                lock (this.lockObject)
                {
                    while (!this.IsCompleted)
                    {
                        Monitor.Wait(this.lockObject);
                    }

                    if (this.exception != null)
                    {
                        throw new AggregateException(this.exception);
                    }

                    return this.result;
                }
            }
        }

        public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult?, TNewResult> nextFunction)
        {
            if (this.threadPool.tokenSource.Token.IsCancellationRequested)
            {
                throw new TaskCanceledException();
            }

            if (this.IsCompleted)
            {
                return this.threadPool.Submit(() => nextFunction(this.Result));
            }

            MyTask<TNewResult> nextTask = new (() => nextFunction(this.Result), this.threadPool);
            this.tasksToContinueWith.Enqueue(nextTask.Execute);

            return nextTask;
        }

        public void Execute()
        {
            try
            {
                this.result = this.function();
            }
            catch (Exception ex)
            {
                this.exception = ex;
            }
            finally
            {
                lock (this.lockObject)
                {
                    this.IsCompleted = true;
                    this.ExecuteContinuations();
                    Monitor.Pulse(this.lockObject);
                }
            }
        }

        private void ExecuteContinuations()
        {
            foreach (var task in this.tasksToContinueWith)
            {
                this.threadPool.taskQueue.Enqueue(task);
            }
        }
    }
}
