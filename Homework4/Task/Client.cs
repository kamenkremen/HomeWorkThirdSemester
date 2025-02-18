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
using System.Text;

/// <summary>
/// Client for simple FTP connection.
/// </summary>
public class Client(IPEndPoint endPoint)
{
    private readonly IPEndPoint endPoint = endPoint;

    /// <summary>
    /// Lists the files on the specified path on the server.
    /// </summary>
    /// <param name="path">Path to the directory with files.</param>
    /// <returns>Reponse from the server.</returns>
    public async Task<string?> List(string path)
    {
        return await this.SendRequest($"1 {path}");
    }

    /// <summary>
    /// Gets a file from specified path on the server.
    /// </summary>
    /// <param name="path">Path to the file.</param>
    /// <returns>File content.</returns>
    public async Task<string?> Get(string path)
    {
        return await this.SendRequest($"2 {path}");
    }

    private async Task<string?> SendRequest(string request)
    {
        var client = new TcpClient();
        await client.ConnectAsync(this.endPoint);
        using var stream = client.GetStream();
        using var writer = new StreamWriter(stream);
        writer.AutoFlush = true;
        using var reader = new StreamReader(stream);
        await writer.WriteLineAsync(request);
        var result = string.Empty;
        if (request == "2")
        {
            var lengthString = new StringBuilder();
            char[] buf = new char[1];
            while (buf[0] != ' ')
            {
                buf = new char[1];
                reader.Read(buf);

                lengthString.Append(buf[0]);
            }

            var length = int.Parse(lengthString.ToString());
            buf = new char[length + 1];
            _ = reader.ReadAsync(buf);
            result = $"{length} {result}";
        }
        else
        {
            result = await reader.ReadLineAsync();
        }

        return result;
    }
}
