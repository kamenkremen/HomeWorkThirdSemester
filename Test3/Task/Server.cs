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

namespace Chat;
using System.Net;
using System.Net.Sockets;

/// <summary>
/// Server for chat for two users.
/// </summary>
public class Server
{
    private readonly int port;
    private CancellationTokenSource cts = new ();

    /// <summary>
    /// Initializes a new instance of the <see cref="Server"/> class.
    /// </summary>
    /// <param name="port">Port that the server will listen to.</param>
    /// <exception cref="ArgumentException">Throws if port is invalid.</exception>
    public Server(int port)
    {
        if (port < 0)
        {
            throw new ArgumentException();
        }

        this.port = port;
    }

    /// <summary>
    /// Gets or sets writer for test purposes.
    /// </summary>
    public StreamWriter? TestWriter { get; set; } = null;

    /// <summary>
    /// Gets or sets reader for test purposes.
    /// </summary>
    public StreamReader? TestReader { get; set; } = null;

    /// <summary>
    /// Starts server.
    /// </summary>
    /// <returns>Running server task.</returns>
    public async Task Start()
    {
        if (this.TestReader != null && this.TestWriter != null)
        {
            await Task.WhenAny(this.Read(this.TestReader, this.cts.Token), this.Write(this.TestWriter, this.cts.Token));
            return;
        }

        var listener = new TcpListener(IPAddress.Any, this.port);
        listener.Start();

        var client = await listener.AcceptTcpClientAsync();
        using var stream = client.GetStream();
        using var writer = new StreamWriter(stream) { AutoFlush = true };
        using var reader = new StreamReader(stream);

        await Task.WhenAny(this.Read(reader, this.cts.Token), this.Write(writer, this.cts.Token));
        client.Close();
        listener.Stop();
    }

    private async Task Read(StreamReader reader, CancellationToken token)
    {
        string? message;
        while ((message = await reader.ReadLineAsync()) is not null && message != "exit")
        {
            if (token.IsCancellationRequested)
            {
                break;
            }

            Console.WriteLine($"incoming message: {message[1..]}");
        }

        this.cts.Cancel();
    }

    private async Task Write(StreamWriter writer, CancellationToken token)
    {
        string? message;
        while ((message = Console.ReadLine()) != "exit")
        {
            if (token.IsCancellationRequested)
            {
                break;
            }

            await writer.WriteLineAsync('a' + message);
        }

        await writer.WriteLineAsync();
        this.cts.Cancel();
    }
}
