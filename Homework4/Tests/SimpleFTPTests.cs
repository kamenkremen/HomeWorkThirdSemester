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

namespace SimpleFTPTests;

using System.Net;
using SimpleFTP;

public class Tests
{

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private Server server;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private Client client;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private IPEndPoint endPoint = new (IPAddress.Loopback, 8888);
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        this.server = new (endPoint.Port);
        this.client = new (endPoint);
        _ = Task.Run(() => this.server.Start());
    }

    [OneTimeTearDown]
    public void OneTimeTearDown() {
        this.server.Stop();
    }

    [Test]
    public async Task ListCorrectPathTest()
    {
        var a = await this.client.List("../../../TestFiles/test1");
        var expected = "2";
        var files = Directory.GetFileSystemEntries("../../../TestFiles/test1");
        Array.Sort(files);
        foreach (var file in files)
        {
            expected += ' ';
            expected += file;
            expected += " ";
            expected += (Directory.Exists(file) ? "true" : "false");
        }

        Assert.That(a, Is.EqualTo(expected));
    }

    [Test]
    public async Task GetCorrectPathTest()
    {
        var a = await this.client.Get("../../../TestFiles/test1/test1.txt");
        var excpected = "4 hehe";
        Assert.That(a, Is.EqualTo(excpected));
    }

    [Test]
    public async Task ListIncorrectPath()
    {
        var a = await this.client.List("test2");
        var excpected = "-1";
        Assert.That(a, Is.EqualTo(excpected));
    }

    [Test]
    public async Task GetIncorrectPath()
    {
        var a = await this.client.List("test2.txt");
        var excpected = "-1";
        Assert.That(a, Is.EqualTo(excpected));
    }

    [Test]
    public void ManyClients()
    {
        const string listPath = "Tests/TestFiles/test1";
        const string getPath = "Tests/TestFiles/test1/test1.txt";
        const int clients = 4;

        var listResults = new string?[clients];
        var getResults = new string?[clients];
        var tasks = new Task[clients];
        for (int i = 0; i < 4; i++) {
            var locali = i;
            tasks[i] = Task.Run(
                async () => 
                {
                var newClient = new Client(endPoint);
                listResults[locali] = await newClient.List(listPath);
                getResults[locali] = await newClient.Get(getPath);
                });
        }
        Task.WaitAll(tasks);

        foreach (var getResult in getResults) {
            Assert.That(getResult, Is.EqualTo(getResults[0]));
        }

        foreach (var listResult in listResults) {
            Assert.That(listResult, Is.EqualTo(listResults[0]));
        }
    }
}
