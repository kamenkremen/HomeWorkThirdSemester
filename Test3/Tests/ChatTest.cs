namespace Tests;

using System.Net;
using System.Net.Sockets;
using Chat;

public class Tests
{
    private readonly IPEndPoint testEndPoint = new (IPAddress.Loopback, 8888);

    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void TestShouldThrowSocketException()
    {
        var client = new Client(testEndPoint);
        Assert.ThrowsAsync<SocketException>(client.Start);
    }

    [Test]
    public void TestInvalidPort()
    {
        Assert.Throws<ArgumentException>(() => new Server(-128));
    }

    /*[Test]
    public async Task TestCorrectWork()
    {
        var serverInput = File.OpenRead("../../../ServerInput.txt");
        var clientInput = File.OpenRead("../../../ClientInput.txt");
        var serverOutput = File.OpenWrite("../../../ServerOutput.txt");
        var clientOutput = File.OpenWrite("../../../ClientOutput.txt");
        var server = new Server(testEndPoint.Port);
        server.TestReader = new StreamReader(serverInput);
        server.TestWriter = new StreamWriter(serverOutput);// { AutoFlush = true };
        var client = new Client(testEndPoint);
        client.TestReader = new StreamReader(clientInput);
        client.TestWriter = new StreamWriter(clientOutput);// { AutoFlush = true };
        await server.Start();
        await client.Start();

    }*/
}