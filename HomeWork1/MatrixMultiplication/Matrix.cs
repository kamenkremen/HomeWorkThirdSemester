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
/// Class that represents matrix.
/// </summary>
public class Matrix
{
    /// <summary>
    /// Gets 2D array which contains matrix elements.
    /// </summary>
    private readonly int[,] matrixElements;

    /// <summary>
    /// Initializes a new instance of the <see cref="Matrix"/> class from a 2D array.
    /// </summary>
    /// <param name="matrixElements">2D array with matrix elements.</param>
    public Matrix(int[,] matrixElements) => this.matrixElements = matrixElements;

    /// <summary>
    /// Initializes a new instance of the <see cref="Matrix"/> class from the file.
    /// </summary>
    /// <param name="path">Path to the file with the matrix.</param>
    public Matrix(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException();
        }

        var lines = File.ReadAllLines(path);
        var numberOfColumns = lines[0].Split().Length;
        var numberOfRows = lines.Length;
        var matrixElements = new int[numberOfRows, numberOfColumns];
        for (var currentRow = 0; currentRow < numberOfRows; ++currentRow)
        {
            var elements = lines[currentRow].Split();
            if (elements.Length != numberOfColumns)
            {
                throw new ArgumentException();
            }

            for (var currentColumn = 0; currentColumn < numberOfColumns; ++currentColumn)
            {
                if (!int.TryParse(elements[currentColumn], out int currentElement))
                {
                    throw new ArgumentException();
                }

                matrixElements[currentRow, currentColumn] = currentElement;
            }
        }

        this.matrixElements = matrixElements;
    }

    /// <summary>
    /// Gets number of rows in the matrix.
    /// </summary>
    public int NumberOfRows => this.matrixElements.GetLength(0);

    /// <summary>
    /// Gets number of columns in the matrix.
    /// </summary>
    public int NumberOfColumns => this.matrixElements.GetLength(1);

    /// <summary>
    /// Gets or sets elements in matrix.
    /// </summary>
    /// <param name="row">Index of row in matrix.</param>
    /// <param name="column">Index of column in matrix.</param>
    /// <returns>Element of matrix by row and column.</returns>
    public int this[int row, int column]
    {
        get => this.matrixElements[row, column];
        set => this.matrixElements[row, column] = value;
    }

    /// <summary>
    /// Generates new matrix filled with random numbers.
    /// </summary>
    /// <param name="rowsCount">Count of rows in the generated matrix.</param>
    /// <param name="columnsCount">Count of columns in the generated matrix.</param>
    /// <returns>Generated matrix.</returns>
    public static Matrix GenerateRandomMatrix(int rowsCount, int columnsCount)
    {
        var newMatrix = new int[rowsCount, columnsCount];
        var rand = new Random();
        for (var currentRow = 0; currentRow < rowsCount; ++currentRow)
        {
            for (var currentColumn = 0; currentColumn < columnsCount; ++currentColumn)
            {
                newMatrix[currentRow, currentColumn] = rand.Next();
            }
        }

        return new Matrix(newMatrix);
    }

    /// <summary>
    /// Compares other matrix to this matrix.
    /// </summary>
    /// <param name="other">Other matrix.</param>
    /// <returns>True, if other matrix is equal to this matrix, and False otherwise.</returns>
    public bool IsEqualTo(Matrix other) => Enumerable.Range(0, 2).All(dimension =>
        this.matrixElements.GetLength(dimension) == other.matrixElements.GetLength(dimension)) &&
        this.matrixElements.Cast<int>().SequenceEqual(other.matrixElements.Cast<int>());

    /// <summary>
    /// Writes matrix to the file.
    /// </summary>
    /// <param name="path">Path to the file.</param>
    public void WriteToFile(string path)
    {
        using var fileWriter = new StreamWriter(path);
        for (var i = 0; i < this.NumberOfRows; ++i)
        {
            for (var j = 0; j < this.NumberOfColumns; ++j)
            {
                fileWriter.Write($"{this.matrixElements[i, j]} ");
            }

            fileWriter.Write('\n');
        }
    }
}
