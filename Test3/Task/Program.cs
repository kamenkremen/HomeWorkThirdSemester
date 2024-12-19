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
using System.Net;
using System.Net.Sockets;
using Chat;

string help_message = "If you want to start as client, specify the port and the ip. If you want to start as server, specify only the port.";

if (args[0] == "help" || args.Length == 0)
{
    Console.WriteLine(help_message);
    return;
}

int port;
if (!int.TryParse(args[0], out port))
{
    Console.WriteLine("Invalid port");
    return;
}

if (args.Length == 1)
{
    var server = new Server(port);
    await server.Start();
}
else
{
    IPAddress? ip;
    if (!IPAddress.TryParse(args[1], out ip))
    {
        Console.WriteLine("invalid address");
        return;
    }

    var endPoint = new IPEndPoint(ip, port);
    try
    {
        var client = new Client(endPoint);
        await client.Start();
    }
    catch (SocketException)
    {
        Console.WriteLine("Connection error");
    }
}
