// Copyright 2024 Ivan Zakarlyuka.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
namespace MyThreadPool;

using System.Collections.Concurrent;

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
        this.threads = new Thread[numberOfThreads];

        for (var i = 0; i < this.NumberOfThreads; ++i)
        {
            this.threads[i] = new Thread(() => this.ThreadFunction(this.tokenSource.Token));
            this.threads[i].Start();
        }
    }

    /// <summary>
    /// Gets number of threads in the threadpool.
    /// </summary>
    public int NumberOfThreads { get => this.threads.Length; }

    /// <summary>
    /// Adds task, that is going to calculate function, to the threadpool.
    /// </summary>
    /// <typeparam name="TResult">Return type of the function.</typeparam>
    /// <param name="function">Function, that is going to be calculated.</param>
    /// <returns>Task, that represents function calculation.</returns>
    /// <exception cref="TaskCanceledException">Throws if threadpool had been shutted down.</exception>
    public IMyTask<TResult> Submit<TResult>(Func<TResult> function)
    {
        lock (this.taskQueue)
        {
            if (this.tokenSource.IsCancellationRequested)
            {
                throw new TaskCanceledException();
            }

            lock (this.lockObject)
            {
                var newTask = new MyTask<TResult>(function, this);
                this.taskQueue.Enqueue(newTask.Execute);
                Monitor.Pulse(this.lockObject);
                return newTask;
            }
        }
    }

    /// <summary>
    /// Signals threadpool to stop the work.
    /// </summary>
    public void Shutdown()
    {
        lock (this.taskQueue)
        {
            this.tokenSource.Cancel();

            foreach (var thread in this.threads)
            {
                thread.Join();
            }
        }
    }

    private void ThreadFunction(CancellationToken token)
    {
        Action? currentAction = null;
        while (!this.taskQueue.IsEmpty || !token.IsCancellationRequested)
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
                if (this.IsCompleted)
                {
                    if (this.exception != null)
                    {
                        throw new AggregateException(this.exception);
                    }

                    return this.result;
                }

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
            lock (this.threadPool.taskQueue)
            {
                if (this.threadPool.tokenSource.IsCancellationRequested)
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
