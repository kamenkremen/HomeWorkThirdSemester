namespace Tests;

using System.Text;
using Test1;

public class Tests
{
    [Test]
    public void TestOneDirectory()
    {
        Assert.That(Encoding.UTF8.GetString(MD5Sequential.ComputeSum("../testFolder/test1")), Is.EqualTo(Encoding.UTF8.GetString(MD5Async.ComputeSum("../testFolder/test1").Result)));
    }

    [Test]
    public void TestOneFile()
    {
        Assert.That(Encoding.UTF8.GetString(MD5Sequential.ComputeSum("../testFolder/test1.txt")), Is.EqualTo(Encoding.UTF8.GetString(MD5Async.ComputeSum("../testFolder/test1.txt").Result)));
    }

    [Test]
    public void TestManyDirectories()
    {
        Assert.That(Encoding.UTF8.GetString(MD5Sequential.ComputeSum("../testFolder/testBenchmark")), Is.EqualTo(Encoding.UTF8.GetString(MD5Async.ComputeSum("../testFolder/testBenchmark").Result)));
    }
}
