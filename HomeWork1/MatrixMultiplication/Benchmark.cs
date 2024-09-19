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

namespace MatrixMultiplication;
using BenchmarkDotNet.Attributes;

/// <summary>
/// Class that makes benchmarks of matrix multiplications.
/// </summary>
[XmlExporter]
[CsvExporter]
[HtmlExporter]
public class Benchmarks
{
    /// <summary>
    /// Size of matrix for benchmark.
    /// </summary>
    [Params(10, 20, 100, 200, 400, 800, 1600)]
    public int MatrixSize;

    /// <summary>
    /// First matrix in multiplication.
    /// </summary>
    private Matrix firstMatrix;

    /// <summary>
    /// Second matrix in multiplication.
    /// </summary>
    private Matrix secondMatrix;

    /// <summary>
    /// Setup for benchmark.
    /// </summary>
    [GlobalSetup]
    public void Setup()
    {
        this.firstMatrix = Matrix.GenerateRandomMatrix(this.MatrixSize, this.MatrixSize);
        this.secondMatrix = Matrix.GenerateRandomMatrix(this.MatrixSize, this.MatrixSize);
    }

    /// <summary>
    /// Multiplicates matrices in parallel.
    /// </summary>
    /// <returns>Result of the multiplication.</returns>
    [Benchmark]
    public Matrix Parallel() => MatrixMultiplyer.ParallelMultiply(this.firstMatrix, this.secondMatrix);

    /// <summary>
    /// Multiplicates matrices sequentially.
    /// </summary>
    /// <returns>Result of the multiplication.</returns>
    [Benchmark]
    public Matrix Sequentially() => MatrixMultiplyer.Multiply(this.firstMatrix, this.secondMatrix);
}
