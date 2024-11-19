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

namespace LazyTests;

using Lazy;
public class Tests
{
    public static ILazy<int?>[] LazyRand =
    {
        new SingleThreadLazy<int?> (() => {Random rand = new (); return rand.Next();}),
        new MultiThreadLazy<int?> (() => {Random rand = new (); return rand.Next();}),
    };
    public static ILazy<int?>[] LazyNull =
    {
        new SingleThreadLazy<int?> (() => null),
        new MultiThreadLazy<int?> (() => null),
    };

    public static ILazy<int>[] LazyException =
    {
        new SingleThreadLazy<int> (() => throw new ArgumentException()),
        new MultiThreadLazy<int> (() => throw new ArgumentException()),
    };

    [TestCaseSource(nameof(LazyRand))]
    [TestCaseSource(nameof(LazyNull))]
    public void TestLazyCalculatesOnce(ILazy<int?> lazy)
    {
        var firstResult = lazy.Get();
        Assert.That(firstResult, Is.EqualTo(lazy.Get()));
    }

    [TestCaseSource(nameof(LazyException))]
    public void TestLazyThrowsException(ILazy<int> lazy)
    {
        Assert.Throws<ArgumentException>(() => lazy.Get());
        Assert.Throws<ArgumentException>(() => lazy.Get());
    }

    [Test]
    public void TestMultiThreadLazy()
    {
        ManualResetEvent mre = new (false);
        MultiThreadLazy<int> lazy = new (() => 
            {
                Random rand = new();
                return rand.Next();
            });
        var threadCount = 8;
        int[] results = new int[threadCount];
        Thread[] threads = new Thread[threadCount];
        for (int i = 0; i < threadCount; i++)
        {
            var locali = i;
            threads[locali] = new Thread(() => 
            {
                mre.WaitOne();
                results[locali] = lazy.Get();
            });

            threads[i].Start();
        }

        mre.Set();

        foreach (var thread in threads)
        {
            thread.Join();
        }

        foreach (var result in results)
        {
            Assert.That(results[0], Is.EqualTo(result));
        }
    }
}
