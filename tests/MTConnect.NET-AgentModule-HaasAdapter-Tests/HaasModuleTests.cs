// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Agents;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.AgentModule.HaasAdapter.Tests
{
    /// <summary>
    /// Runs the module against a fake Haas control (a TCP server answering
    /// Q-commands) and checks the observations that reach the Agent.
    /// </summary>
    [TestFixture]
    public class HaasModuleTests
    {
        private const string DeviceName = "VF2";

        /// <summary>Observations are read from the control and added to the generated Device.</summary>
        [TestCase(true)]
        [TestCase(false)]
        public async Task Module_reports_observations(bool framed)
        {
            using (var control = new FakeHaasControl { Framed = framed })
            {
                var agent = new MTConnectAgentBroker();
                var module = CreateModule(agent, control.Port, "inch");

                try
                {
                    await WaitFor(() => GetValue(agent, "avail") == "AVAILABLE" && GetValue(agent, "speed") != "UNAVAILABLE");

                    Assert.That(GetValue(agent, "mode"), Is.EqualTo("AUTOMATIC"));
                    Assert.That(GetValue(agent, "execution"), Is.EqualTo("READY"));
                    Assert.That(GetValue(agent, "program"), Is.EqualTo("O01234"));
                    Assert.That(GetValue(agent, "partCount"), Is.EqualTo("42"));
                    Assert.That(GetValue(agent, "x_act"), Is.EqualTo("38.1"), "inches are converted to millimeters");
                    Assert.That(GetValue(agent, "speed"), Is.EqualTo("8000"));
                    Assert.That(GetValue(agent, "system", "Level"), Is.EqualTo("NORMAL"));
                }
                finally
                {
                    module.Stop();
                }
            }
        }

        /// <summary>An alarm raises a FAULT on the system condition and stops execution.</summary>
        [Test]
        public async Task Module_reports_alarm()
        {
            using (var control = new FakeHaasControl())
            {
                control.Q500 = "PROGRAM, O01234, ALARM ON, PARTS, 42";

                var agent = new MTConnectAgentBroker();
                var module = CreateModule(agent, control.Port);

                try
                {
                    await WaitFor(() => GetValue(agent, "system", "Level") == "FAULT");

                    Assert.That(GetValue(agent, "execution"), Is.EqualTo("STOPPED"));
                }
                finally
                {
                    module.Stop();
                }
            }
        }

        /// <summary>Losing the connection sets the Device UNAVAILABLE and it recovers when the control returns.</summary>
        [Test]
        public async Task Module_sets_unavailable_and_reconnects()
        {
            var control = new FakeHaasControl();
            var port = control.Port;

            var agent = new MTConnectAgentBroker();
            var module = CreateModule(agent, port);

            try
            {
                await WaitFor(() => GetValue(agent, "avail") == "AVAILABLE");

                control.Dispose();

                await WaitFor(() => GetValue(agent, "avail") == "UNAVAILABLE");
                Assert.That(GetValue(agent, "execution"), Is.EqualTo("UNAVAILABLE"));
                Assert.That(GetValue(agent, "system", "Level"), Is.EqualTo("UNAVAILABLE"));

                using (new FakeHaasControl(port))
                {
                    await WaitFor(() => GetValue(agent, "avail") == "AVAILABLE", TimeSpan.FromSeconds(15));
                }
            }
            finally
            {
                module.Stop();
                control.Dispose();
            }
        }


        private static Modules.Module CreateModule(IMTConnectAgentBroker agent, int port, string units = "mm")
        {
            var configuration = new Dictionary<string, object>
            {
                { "deviceKey", DeviceName },
                { "server", "127.0.0.1" },
                { "port", port },
                { "readInterval", 100 },
                { "responseTimeout", 1000 },
                { "units", units }
            };

            var module = new Modules.Module(agent, configuration);
            module.StartBeforeLoad(true);
            module.StartAfterLoad(true);
            return module;
        }

        private static string GetValue(IMTConnectAgentBroker agent, string name, string valueKey = "Result")
        {
            var observation = agent.GetCurrentObservations(DeviceName)?.FirstOrDefault(o => o.Name == name);
            return observation?.GetValue(valueKey);
        }

        private static async Task WaitFor(Func<bool> condition, TimeSpan? timeout = null)
        {
            var deadline = DateTime.UtcNow + (timeout ?? TimeSpan.FromSeconds(10));
            while (!condition())
            {
                if (DateTime.UtcNow > deadline) Assert.Fail("Timed out waiting for condition");
                await Task.Delay(50);
            }
        }


        private class FakeHaasControl : IDisposable
        {
            private readonly TcpListener _listener;
            private readonly CancellationTokenSource _stop = new CancellationTokenSource();
            private readonly List<TcpClient> _clients = new List<TcpClient>();

            public int Port { get; }
            public bool Framed { get; set; } = true;
            public string Q500 { get; set; } = "PROGRAM, O01234, IDLE, PARTS, 42";

            public FakeHaasControl(int port = 0)
            {
                _listener = new TcpListener(IPAddress.Loopback, port);
                _listener.Start();
                Port = ((IPEndPoint)_listener.LocalEndpoint).Port;
                _ = Task.Run(Accept);
            }

            private async Task Accept()
            {
                while (!_stop.IsCancellationRequested)
                {
                    TcpClient client;
                    try { client = await _listener.AcceptTcpClientAsync(_stop.Token); }
                    catch { return; }

                    lock (_clients) _clients.Add(client);
                    _ = Task.Run(() => Serve(client));
                }
            }

            private async Task Serve(TcpClient client)
            {
                try
                {
                    var stream = client.GetStream();
                    var reader = new StreamReader(stream, Encoding.ASCII);
                    while (true)
                    {
                        var command = await reader.ReadLineAsync();
                        if (command == null) return;

                        var body = GetReply(command);
                        var reply = Framed ? $"\u0002{body}\u0017\r\n>" : $">{body}\r\n";
                        var bytes = Encoding.ASCII.GetBytes(reply);
                        await stream.WriteAsync(bytes, 0, bytes.Length);
                    }
                }
                catch { }
            }

            private string GetReply(string command)
            {
                switch (command)
                {
                    case "?Q100": return "SERIAL NUMBER, 1234567";
                    case "?Q104": return "MODE, (MEM)";
                    case "?Q500": return Q500;
                    case "?Q600 5021": return "MACRO, 1.5";
                    case "?Q600 5022": return "MACRO, -2.25";
                    case "?Q600 5023": return "MACRO, 0.1";
                    case "?Q600 3027": return "MACRO, 8000.";
                    default: return "INVALID";
                }
            }

            public void Dispose()
            {
                _stop.Cancel();
                _listener.Stop();
                lock (_clients)
                {
                    foreach (var client in _clients) client.Close();
                    _clients.Clear();
                }
            }
        }
    }
}
