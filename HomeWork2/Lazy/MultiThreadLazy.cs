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

namespace Lazy;

/// <summary>
/// Class, that implements lazy calculation in multiple threads.
/// </summary>
/// <typeparam name="T">Return type of function, that is going to be calculated.</typeparam>
/// <param name="supplier">Function, that is going to be calculated.</param>
#pragma warning disable SA1009 // Closing parenthesis should be spaced correctly
public class MultiThreadLazy<T>(Func<T?> supplier) : ILazy<T>
#pragma warning restore SA1009 // Closing parenthesis should be spaced correctly
{
    private Func<T?> supplier = supplier;
    private Exception? thrownException = null;
    private T? result;
    private bool calculated = false;
    private object lockObject = new ();

    /// <inheritdoc/>
    public T? Get()
    {
        if (this.thrownException != null)
        {
            throw this.thrownException;
        }

        lock (this.lockObject)
        {
            if (!this.calculated)
            {
                try
                {
                    this.result = this.supplier();
                }
                catch (Exception exception)
                {
                    this.thrownException = exception;
                    throw;
                }
                finally
                {
                    this.calculated = true;
                    #pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                    this.supplier = null;
                    #pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                }
            }
        }

        return this.result;
    }
}
