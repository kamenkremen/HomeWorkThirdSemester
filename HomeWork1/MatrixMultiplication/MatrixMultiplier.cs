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

/// <summary>
/// Class that multiplies matrices.
/// </summary>
public static class MatrixMultiplier
{
    /// <summary>
    /// Multiplies two matrices and returns result.
    /// </summary>
    /// <param name="first">First matrix.</param>
    /// <param name="second">Second matrix.</param>
    /// <returns>Result of multiplication.</returns>
    /// <exception cref="ArgumentException">Throws if dimension of matrices are not suitable for multiplication.</exception>
    public static Matrix Multiply(Matrix first, Matrix second)
    {
        if (first.NumberOfColumns != second.NumberOfRows)
        {
            throw new ArgumentException();
        }

        var newMatrix = new int[first.NumberOfRows, second.NumberOfColumns];
        for (var currentRow = 0; currentRow < newMatrix.GetLength(0); ++currentRow)
        {
            for (var currentColumn = 0; currentColumn < newMatrix.GetLength(1); ++currentColumn)
            {
                for (var i = 0; i < first.NumberOfColumns; ++i)
                {
                    newMatrix[currentRow, currentColumn] += first[currentRow, i] * second[i, currentColumn];
                }
            }
        }

        return new Matrix(newMatrix);
    }

    /// <summary>
    /// Multiplies two matrices in parallel and returns result.
    /// </summary>
    /// <param name="first">First matrix.</param>
    /// <param name="second">Second matrix.</param>
    /// <returns>Result of multiplication.</returns>
    /// <exception cref="ArgumentException">Throws if dimension of matrices are not suitable for multiplication.</exception>
    public static Matrix ParallelMultiply(Matrix first, Matrix second)
    {
        if (first.NumberOfColumns != second.NumberOfRows)
        {
            throw new ArgumentException();
        }

        var processorCount = Environment.ProcessorCount;
        var rowsForSingleThread = (first.NumberOfRows / processorCount) +
            ((first.NumberOfRows % processorCount) == 0 ? 0 : 1);
        var threads = new Thread[processorCount];
        var newMatrix = new int[first.NumberOfRows, second.NumberOfColumns];
        for (var i = 0; i < processorCount; ++i)
        {
            var startRow = i * rowsForSingleThread;

            var endRow = (i + 1) * rowsForSingleThread < first.NumberOfRows ? (i + 1) * rowsForSingleThread : first.NumberOfRows;
            threads[i] = new Thread(() =>
            {
                for (var currentRow = startRow; currentRow < endRow; ++currentRow)
                {
                    for (var currentColumn = 0; currentColumn < newMatrix.GetLength(1); ++currentColumn)
                    {
                        for (var j = 0; j < first.NumberOfColumns; ++j)
                        {
                            newMatrix[currentRow, currentColumn] += first[currentRow, j] * second[j, currentColumn];
                        }
                    }
                }
            });

            threads[i].Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }

        return new Matrix(newMatrix);
    }
}
