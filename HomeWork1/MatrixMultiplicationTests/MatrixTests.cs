namespace MatrixMultiplicationTests;

using MatrixMultiplication;
using Microsoft.VisualBasic;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void MatrixCreationFrom2DArrayTest()
    {
        int[,] elements = {{1, 2}, {3, 4}, {5, 6}};
        var matrix = new Matrix(elements);
        for (int i = 0; i < 3; ++i)
        {
            for (int j = 0; j < 2; ++j)
            {
                Assert.That(matrix[i, j], Is.EqualTo(elements[i, j]));
            }
        }
    }

    [Test]
    public void MatrixCreationFromFileTest()
    {
        int[,] elements = {{1, 2}, {3, 4}, {5, 6}};
        var matrix = new Matrix("../../../TestFiles/TestMatrix1.txt");
        for (int i = 0; i < 3; ++i)
        {
            for (int j = 0; j < 2; ++j)
            {
                Assert.That(matrix[i, j], Is.EqualTo(elements[i, j]));
            }
        }
    }

    [Test]
    public void MatrixCreationFromFileTestShouldFailWithArgumentException()
    {
        int[,] elements = {{1, 2}, {3, 4}, {5, 6}};
        Assert.Throws<ArgumentException>(() => new Matrix("../../../TestFiles/TestMatrix2.txt"));
    }

    [Test]
    public void MatrixCreationFromFileTestShouldFailWithFileNotFoundException()
    {
        int[,] elements = {{1, 2}, {3, 4}, {5, 6}};
        Assert.Throws<FileNotFoundException>(() => new Matrix("../../../TestFiles/TestMatrix3.txt"));
    }


    [Test]
    public void MatrixSequentialMultiplicationTest()
    {
        int[,] elements1 = {{1, 2}, {3, 4}, {5, 6}};
        int[,] elements2 = {{7, 8, 9}, {10, 11, 12}};
        int[,] expectedResult = {{27, 30, 33}, {61, 68, 75}, {95, 106, 117}};
        var expectedMatrix = new Matrix(expectedResult);
        var matrix1 = new Matrix(elements1);
        var matrix2 = new Matrix(elements2);
        var resultMatrix = MatrixMultiplyer.Multiply(matrix1, matrix2);
        Assert.That(resultMatrix.IsEqualTo(expectedMatrix), Is.True);
    }

    [Test]
    public void MatrixParallelMultiplicationTest()
    {
        int[,] elements1 = {{1, 2}, {3, 4}, {5, 6}};
        int[,] elements2 = {{7, 8, 9}, {10, 11, 12}};
        int[,] expectedResult = {{27, 30, 33}, {61, 68, 75}, {95, 106, 117}};
        var expectedMatrix = new Matrix(expectedResult);
        var matrix1 = new Matrix(elements1);
        var matrix2 = new Matrix(elements2);
        var resultMatrix = MatrixMultiplyer.ParallelMultiply(matrix1, matrix2);
        Assert.That(resultMatrix.IsEqualTo(expectedMatrix), Is.True);
    }
    
    [Test]
    public void MatrixSequentialMultiplicationTestShouldFail()
    {
        int[,] elements1 = {{1, 2}, {3, 4}, {5, 6}};
        int[,] elements2 = {{7, 8}, {9, 10}, {11, 12}};
        var matrix1 = new Matrix(elements1);
        var matrix2 = new Matrix(elements2);
        Assert.Throws<ArgumentException>(() => MatrixMultiplyer.Multiply(matrix1, matrix2));
    }
}
