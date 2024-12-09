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
using MyNunit;
public class TestClass
{
    int a = 0;
    [BeforeClass]
    public static void SetUp() 
    {
        Console.WriteLine("SetUp called");
    }

    [Before]
    public void Before()
    {
        this.a += 10;
    }

    [Test]
    public void Test1()
    {
        if (a != 10) {
            throw new ArgumentException($"a = {a}, expected 10");
        }
    }

    [Test(Ignore = "just for laughs")]
    public void TestIgnored()
    {
        throw new ArgumentException("no laughs, test not ignored");
    }

    [Test(Expected = typeof(ArgumentException))]
    public void TestThrowingException()
    {
        throw new ArgumentException("test exception");
    }

    [Test]
    public void TestThrowingExceptionNotExpected()
    {
        throw new ArgumentException("test exception");
    }

    [After]
    public void After()
    {
        this.a = 0;
    }

    [AfterClass]
    public static void TearDown()
    {
        Console.WriteLine("TearDown called");
    }
}