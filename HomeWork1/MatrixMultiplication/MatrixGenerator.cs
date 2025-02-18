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
/// Class that generates matrices.
/// </summary>
public static class MatrixGenerator
{
    private static readonly Random Random = new ();

    /// <summary>
    /// Generates new matrix filled with random numbers.
    /// </summary>
    /// <param name="rowsCount">Count of rows in the generated matrix.</param>
    /// <param name="columnsCount">Count of columns in the generated matrix.</param>
    /// <returns>Generated matrix.</returns>
    public static Matrix GenerateRandomMatrix(int rowsCount, int columnsCount)
    {
        var newMatrix = new int[rowsCount, columnsCount];
        for (var currentRow = 0; currentRow < rowsCount; ++currentRow)
        {
            for (var currentColumn = 0; currentColumn < columnsCount; ++currentColumn)
            {
                newMatrix[currentRow, currentColumn] = Random.Next();
            }
        }

        return new Matrix(newMatrix);
    }
}
