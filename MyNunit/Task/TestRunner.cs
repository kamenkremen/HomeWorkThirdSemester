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
namespace MyNunit;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;

/// <summary>
/// Class that runs tests.
/// </summary>
public static class TestRunner
{
    /// <summary>
    /// Runs tests from path.
    /// </summary>
    /// <param name="path">Path to directory with tests.</param>
    /// <exception cref="ArgumentException">Throws if there is no such path.</exception>
    public static void RunFromPath(string path)
    {
        if (!Directory.Exists(path))
        {
            throw new ArgumentException("invalid path");
        }

        var assemblies = new List<Assembly>();

        foreach (var file in Directory.GetFiles(path, "*.dll"))
        {
            Console.WriteLine(file);
            var assembly = Assembly.LoadFrom(file);
            assemblies.Add(assembly);
        }

        Parallel.ForEach(assemblies, RunFromAssebly);
    }

    private static void RunFromAssebly(Assembly assembly)
    {
        var results = new ConcurrentBag<TestResult>();
        Parallel.ForEach(assembly.DefinedTypes, type =>
        {
            var tests = type.GetMethods().Where(method => method.GetCustomAttributes<Test>().Any()).ToList();
            var beforeClassMethods = type.GetMethods().Where(method => method.GetCustomAttributes<BeforeClass>().Any()).ToList();
            var afterClassMethods = type.GetMethods().Where(method => method.GetCustomAttributes<AfterClass>().Any()).ToList();
            var beforeMethods = type.GetMethods().Where(method => method.GetCustomAttributes<Before>().Any()).ToList();
            var afterMethods = type.GetMethods().Where(method => method.GetCustomAttributes<After>().Any()).ToList();
            foreach (var method in beforeClassMethods)
            {
                if (!method.IsStatic)
                {
                    throw new InvalidDataException($"BeforeClass method should be static, {method.Name} is not static.");
                }

                method.Invoke(null, null);
            }

            Parallel.ForEach(tests, test =>
            {
                var result = RunTest(type, test, beforeMethods, afterMethods);
                if (result is not null)
                {
                    results.Add((TestResult)result);
                }
            });

            foreach (var method in afterClassMethods)
            {
                if (!method.IsStatic)
                {
                    throw new InvalidDataException($"AfterClass method should be static, {method.Name} is not static.");
                }

                method.Invoke(null, null);
            }
        });

        PrintResults(results);
    }

    private static TestResult? RunTest(Type type, MethodInfo testMethodInfo, List<MethodInfo>? beforeMethods, List<MethodInfo>? afterMethods)
    {
        if (testMethodInfo.GetCustomAttributes<Before>().Any() ||
            testMethodInfo.GetCustomAttributes<AfterClass>().Any() ||
            testMethodInfo.GetCustomAttributes<After>().Any() ||
            testMethodInfo.GetCustomAttributes<BeforeClass>().Any())
        {
            return null;
        }

        var result = new TestResult(testMethodInfo.Name);

        var test = testMethodInfo.GetCustomAttribute<Test>();
        if (test?.Ignore is not null)
        {
            result.WhyIgnored = test.Ignore;
            return result;
        }

        if (testMethodInfo.GetParameters().Length > 0 || testMethodInfo.ReturnType != typeof(void))
        {
            result.WhyIgnored = "Invalid test signature";
            return result;
        }

        var instance = Activator.CreateInstance(type);
        if (beforeMethods is not null)
        {
            foreach (var beforeMethod in beforeMethods)
            {
                beforeMethod.Invoke(instance, null);
            }
        }

        var stopwatch = new Stopwatch();
        try
        {
            stopwatch.Start();
            testMethodInfo.Invoke(instance, null);
            stopwatch.Stop();
            result.EllapsedTime = stopwatch.ElapsedMilliseconds;
            result.Ok = true;
        }
        catch (Exception exception)
        {
            if (test?.Expected is not null && test?.Expected == exception.InnerException?.GetType())
            {
                result.Ok = true;
            }
            else
            {
                result.Ok = false;
                result.ExceptionMessage = exception.InnerException?.Message ?? "Exception with no message";
            }
        }
        finally
        {
            stopwatch.Stop();
        }

        if (afterMethods is not null)
        {
            foreach (var afterMethod in afterMethods)
            {
                afterMethod.Invoke(instance, null);
            }
        }

        return result;
    }

    private static void PrintResults(ConcurrentBag<TestResult> results)
    {
        if (results.IsEmpty)
        {
            return;
        }

        var toPrint = string.Empty;
        var passed = 0;
        var ignored = 0;
        foreach (var result in results)
        {
            var currentResult = string.Empty;
            if (result.Ok)
            {
                currentResult += $"{result.TestName} is passed. Ellapsed time - {result.EllapsedTime}ms\n";
                ++passed;
            }

            if (result.WhyIgnored is not null)
            {
                currentResult += $"{result.TestName} ignored. Reason - {result.WhyIgnored}\n";
                ++ignored;
            }

            if (result.ExceptionMessage is not null)
            {
                currentResult += $"{result.TestName} is failed. Exception message - {result.ExceptionMessage}\n";
            }

            toPrint += currentResult;
        }

        Console.WriteLine($"{passed} passed, {ignored} ignored, {results.Count - passed - ignored} failed.\n");
        Console.Write(toPrint);
    }
}
