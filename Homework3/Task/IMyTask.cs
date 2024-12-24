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

/// <summary>
/// Interface for threadpool task.
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
