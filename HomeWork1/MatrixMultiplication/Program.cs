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

#pragma warning disable SA1200 // Using directives should be placed correctly
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using MatrixMultiplication;
#pragma warning restore SA1200 // Using directives should be placed correctly

if (args.Length == 0)
{
    Console.WriteLine("Invalid amount of arguments, type 'dotnet run help'.");
    return 0;
}

if (args.Length == 1 && args[0] == "help")
{
    Console.WriteLine("Type dotnet run 'path to first matrix' 'path to second matrix' to multiply matrices.");
    Console.WriteLine("Type 'dotnet -c Release benchmark' to start benchmarks.");
}

if (args.Length == 1 && args[0] == "benchmark")
{
    Console.WriteLine("Running benchmark...");
    var config = DefaultConfig.Instance;
    var summary = BenchmarkRunner.Run<Benchmarks>(config, args);
    Console.WriteLine("Results of benchmark are in BenchmarkDotNet.Artifacts");
}

if (args.Length >= 2)
{
    try
    {
        var firstMatrix = new Matrix(args[0]);
        var secondMatrix = new Matrix(args[1]);
        var resultSequentally = MatrixMultiplier.Multiply(firstMatrix, secondMatrix);
        var resultParallel = MatrixMultiplier.ParallelMultiply(firstMatrix, secondMatrix);
        resultSequentally.WriteToFile("resultSequentally.txt");
        resultParallel.WriteToFile("resultParallel.txt");
    }
    catch (ArgumentException)
    {
        Console.WriteLine("Error with matrices format.");
    }
    catch (FileNotFoundException)
    {
        Console.WriteLine("File not found.");
    }
}

return 0;