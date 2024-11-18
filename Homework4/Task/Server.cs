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
namespace SimpleFTP;
using System.Net;
using System.Net.Sockets;

/// <summary>
/// Server for simple FTP connection.
/// </summary>
public class Server(int port)
{
    private readonly int port = port;
    private readonly TcpListener listener = new (IPAddress.Any, port);
    private readonly CancellationTokenSource tokenSource = new ();

    /// <summary>
    /// Stops the server.
    /// </summary>
    public void Stop()
    {
        this.tokenSource.Cancel();
        this.listener.Stop();
    }

    /// <summary>
    /// Initiates work of the server.
    /// </summary>
    public async void Start()
    {
        var tasks = new List<Task>();
        this.listener.Start();
        while (!this.tokenSource.IsCancellationRequested)
        {
            TcpClient client;

            // это выглядит очень неправильно, но у меня не получается пофиксить по другому(при остановке сервера бросается OperationCanceledException)
            try
            {
                client = await this.listener.AcceptTcpClientAsync(this.tokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                break;
            }

            tasks.Add(Task.Run(
                async () =>
                {
                    using var stream = client.GetStream();
                    using var reader = new StreamReader(stream);
                    using var writer = new StreamWriter(stream);
                    string? request;
                    while ((request = await reader.ReadLineAsync()) != null)
                    {
                        var operation = request[0];
                        if (operation == '1')
                        {
                            await this.HandleListRequest(request[2..], writer);
                        }

                        if (operation == '2')
                        {
                            await this.HandleGetRequest(request[2..], writer);
                        }
                    }

                    client.Close();
                    }));
        }

        await Task.WhenAll(tasks);
    }

    private async Task HandleListRequest(string path, StreamWriter writer)
    {
        if (!Directory.Exists(path))
        {
            await writer.WriteLineAsync("-1");
            await writer.FlushAsync();
            return;
        }

        var files = Directory.GetFileSystemEntries(path);
        Array.Sort(files);
        await writer.WriteAsync($"{files.Length}");
        foreach (var file in files)
        {
            await writer.WriteAsync($" {file} {(Directory.Exists(file) ? "true" : "false")}");
        }

        await writer.WriteLineAsync();
        await writer.FlushAsync();
    }

    private async Task HandleGetRequest(string path, StreamWriter writer)
    {
        if (!File.Exists(path))
        {
            await writer.WriteLineAsync("-1");
            await writer.FlushAsync();
            return;
        }

        var content = await File.ReadAllBytesAsync(path);
        await writer.WriteLineAsync($"{content.Length} {System.Text.Encoding.UTF8.GetString(content)}");
        await writer.FlushAsync();
    }
}
