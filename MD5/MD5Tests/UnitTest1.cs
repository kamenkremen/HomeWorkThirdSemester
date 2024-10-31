namespace MD5Tests;

using System.Security.Cryptography;
using System.Text;
using Test1;

public class Tests
{
    [Test]
    public async void TestOneDirectory()
    {
        Assert.That(Encoding.UTF8.GetString(MD5Sequential.ComputeSum("../testFolder/test1")), Is.EqualTo(Encoding.UTF8.GetString(MD5Async.ComputeSum("../testFolder/test1").Result)));
    }

    [Test]
    public async void TestManyDirectories()
    {
        Assert.That(Encoding.UTF8.GetString(MD5Sequential.ComputeSum("../testFolder/testBenchmark")), Is.EqualTo(Encoding.UTF8.GetString(MD5Async.ComputeSum("../testFolder/testBenchmark").Result)));
    }

    [Test]
    public async void TestOneFile()
    {
        Assert.That(Encoding.UTF8.GetString(MD5Sequential.ComputeSum("../testFolder/test1.txt")), Is.EqualTo(Encoding.UTF8.GetString(MD5Async.ComputeSum("../testFolder/test1.txt").Result)));
    }
}