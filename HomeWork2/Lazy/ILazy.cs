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
/// Interface that implements lazy calculation.
/// </summary>
/// <typeparam name="T">Return type of function, that is going to be calculated.</typeparam>
public interface ILazy<T>
{
    /// <summary>
    /// Gets result of function calculation.
    /// </summary>
    /// <returns>Result of function calculation.</returns>
    public T? Get();
}
