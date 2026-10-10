// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Agents;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace MTConnect.AgentModule.TormachAdapter.Tests
{
    /// <summary>
    /// Runs the module against a fake PathPilot status server, and against the real status
    /// server script with a fake linuxcnc module, and checks the observations that reach the Agent.
    /// </summary>
    [TestFixture]
    public class TormachModuleTests
    {
        private const string DeviceName = "PCNC1100";

        /// <summary>Observations are read from the status server and added to the generated Device.</summary>
        [Test]
        public async Task Module_reports_observations()
        {
            using (var server = new FakeStatusServer())
            {
                var agent = new MTConnectAgentBroker();
                var module = CreateModule(agent, server.Port);

                try
                {
                    await WaitFor(() => GetValue(agent, "a_act") == "90.5");

                    Assert.That(GetValue(agent, "avail"), Is.EqualTo("AVAILABLE"));
                    Assert.That(GetValue(agent, "estop"), Is.EqualTo("ARMED"));
                    Assert.That(GetValue(agent, "mode"), Is.EqualTo("AUTOMATIC"));
                    Assert.That(GetValue(agent, "execution"), Is.EqualTo("ACTIVE"));
                    Assert.That(GetValue(agent, "program"), Is.EqualTo("bracket.nc"));
                    Assert.That(GetValue(agent, "line"), Is.EqualTo("120"));
                    Assert.That(GetValue(agent, "tool"), Is.EqualTo("5"));
                    Assert.That(GetValue(agent, "workOffset"), Is.EqualTo("54"));
                    Assert.That(GetValue(agent, "optionalStop"), Is.EqualTo("ON"));
                    Assert.That(GetValue(agent, "feedrate"), Is.EqualTo("6.35"), "0.25 in/s = 6.35 mm/s");
                    Assert.That(GetValue(agent, "feedOverride"), Is.EqualTo("100"));
                    Assert.That(GetValue(agent, "rapidOverride"), Is.EqualTo("50"));
                    Assert.That(GetValue(agent, "spindleOverride"), Is.EqualTo("110"));
                    Assert.That(GetValue(agent, "speed"), Is.EqualTo("3000"), "counterclockwise speed is reported as positive");
                    Assert.That(GetValue(agent, "x_act"), Is.EqualTo("38.1"), "inches are converted to millimeters");
                    Assert.That(GetValue(agent, "y_act"), Is.EqualTo("-57.15"));
                }
                finally
                {
                    module.Stop();
                }
            }
        }

        /// <summary>The Device is built with the machine name and axes configured in PathPilot.</summary>
        [Test]
        public async Task Module_updates_device_from_machine_info()
        {
            using (var server = new FakeStatusServer())
            {
                var agent = new MTConnectAgentBroker();
                var module = CreateModule(agent, server.Port, new Dictionary<string, object> { { "workPositions", true } });

                try
                {
                    await WaitFor(() => agent.GetDevice(DeviceName)?.Description?.Model == "tormach_mill_1100");

                    var device = agent.GetDevice(DeviceName);
                    var axes = device.GetComponents().Where(o => o.Type == "Linear" || o.Type == "Rotary").Select(o => $"{o.Type}:{o.Name}");
                    Assert.That(axes, Is.EquivalentTo(new[] { "Linear:X", "Linear:Y", "Linear:Z", "Rotary:A", "Rotary:S" }));
                    Assert.That(device.GetDataItems().Single(o => o.Name == "x_work").CoordinateSystemIdRef, Is.EqualTo("PCNC1100_work"));

                    await WaitFor(() => HasValue(agent, "z_work"));
                    Assert.That(GetValue(agent, "x_work"), Is.EqualTo("12.7"));
                    Assert.That(GetValue(agent, "z_work"), Is.EqualTo("-10.16"));
                }
                finally
                {
                    module.Stop();
                }
            }
        }

        /// <summary>Emergency stop is reported as TRIGGERED and stops execution.</summary>
        [Test]
        public async Task Module_reports_emergency_stop()
        {
            using (var server = new FakeStatusServer())
            {
                server.Set("estop", "true");
                server.Set("task_state", "\"ESTOP\"");

                var agent = new MTConnectAgentBroker();
                var module = CreateModule(agent, server.Port);

                try
                {
                    await WaitFor(() => GetValue(agent, "estop") == "TRIGGERED");
                    Assert.That(GetValue(agent, "execution"), Is.EqualTo("STOPPED"));
                }
                finally
                {
                    module.Stop();
                }
            }
        }

        /// <summary>When PathPilot is not running the Device is UNAVAILABLE; it recovers when PathPilot starts.</summary>
        [Test]
        public async Task Module_reports_unavailable_when_pathpilot_stops()
        {
            using (var server = new FakeStatusServer())
            {
                var agent = new MTConnectAgentBroker();
                var module = CreateModule(agent, server.Port);

                try
                {
                    await WaitFor(() => GetValue(agent, "avail") == "AVAILABLE");

                    server.Set("available", "false");
                    await WaitFor(() => GetValue(agent, "avail") == "UNAVAILABLE" && GetValue(agent, "x_act") == "UNAVAILABLE");

                    server.Set("available", "true");
                    await WaitFor(() => GetValue(agent, "avail") == "AVAILABLE");
                    Assert.That(server.ConnectionCount, Is.EqualTo(1), "the connection is kept open");
                }
                finally
                {
                    module.Stop();
                }
            }
        }

        /// <summary>A status server that stops sending data is disconnected and reconnected.</summary>
        [Test]
        public async Task Module_reconnects_when_no_data_is_received()
        {
            using (var server = new FakeStatusServer())
            {
                var agent = new MTConnectAgentBroker();
                var module = CreateModule(agent, server.Port, new Dictionary<string, object> { { "dataTimeout", 500 } });

                try
                {
                    await WaitFor(() => GetValue(agent, "avail") == "AVAILABLE");

                    server.Silent = true;
                    await WaitFor(() => GetValue(agent, "avail") == "UNAVAILABLE");

                    server.Silent = false;
                    await WaitFor(() => GetValue(agent, "avail") == "AVAILABLE");
                    Assert.That(server.ConnectionCount, Is.GreaterThanOrEqualTo(2));
                }
                finally
                {
                    module.Stop();
                }
            }
        }

        /// <summary>Losing the status server sets the Device UNAVAILABLE; it recovers when the server returns.</summary>
        [Test]
        public async Task Module_sets_unavailable_and_reconnects()
        {
            var server = new FakeStatusServer();
            var port = server.Port;

            var agent = new MTConnectAgentBroker();
            var module = CreateModule(agent, port);

            try
            {
                await WaitFor(() => GetValue(agent, "avail") == "AVAILABLE");

                server.Dispose();

                // Every value is set UNAVAILABLE (they are added one at a time, so wait for all of them)
                await WaitFor(() => GetValue(agent, "avail") == "UNAVAILABLE"
                    && GetValue(agent, "execution") == "UNAVAILABLE"
                    && GetValue(agent, "a_act") == "UNAVAILABLE");

                using (new FakeStatusServer(port))
                {
                    await WaitFor(() => GetValue(agent, "avail") == "AVAILABLE", TimeSpan.FromSeconds(15));
                }
            }
            finally
            {
                module.Stop();
                server.Dispose();
            }
        }

        /// <summary>
        /// The module reads the real PathPilot status server script (run with Python and a fake linuxcnc module).
        /// </summary>
        [Test]
        public async Task Module_reads_pathpilot_status_server_script()
        {
            var script = Path.Combine(AppContext.BaseDirectory, "pathpilot", "pathpilot_status_server.py");
            var fakeModule = Path.Combine(AppContext.BaseDirectory, "fake_linuxcnc");
            Assert.That(File.Exists(script), Is.True, "the status server script is copied with the module");

            var port = GetFreePort();
            var process = StartPython(script, port, fakeModule);
            if (process == null) Assert.Ignore("Python is not installed");

            var agent = new MTConnectAgentBroker();
            var module = CreateModule(agent, port);

            try
            {
                await WaitFor(() => HasValue(agent, "z_act") && agent.GetDevice(DeviceName)?.Description?.Model == "tormach_mill_1100", TimeSpan.FromSeconds(15));

                Assert.That(GetValue(agent, "avail"), Is.EqualTo("AVAILABLE"));
                Assert.That(GetValue(agent, "mode"), Is.EqualTo("AUTOMATIC"));
                Assert.That(GetValue(agent, "execution"), Is.EqualTo("ACTIVE"));
                Assert.That(GetValue(agent, "program"), Is.EqualTo("bracket.nc"));
                Assert.That(GetValue(agent, "workOffset"), Is.EqualTo("55"));
                Assert.That(GetValue(agent, "speed"), Is.EqualTo("3000"));
                Assert.That(GetValue(agent, "spindleOverride"), Is.EqualTo("110"));
                Assert.That(GetValue(agent, "x_act"), Is.EqualTo("38.1"));
                Assert.That(GetValue(agent, "a_act"), Is.EqualTo("90.5"));
                Assert.That(GetValue(agent, "feedrate"), Is.EqualTo("6.35"));
            }
            finally
            {
                module.Stop();
                try { process.Kill(); } catch { }
                process.Dispose();
            }
        }


        private static Process StartPython(string script, int port, string fakeModule)
        {
            foreach (var python in new[] { "python3", "python" })
            {
                try
                {
                    var info = new ProcessStartInfo(python, $"-u \"{script}\" --bind 127.0.0.1 --port {port} --interval 100")
                    {
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    };
                    info.Environment["PYTHONPATH"] = fakeModule;

                    var process = Process.Start(info);
                    var line = process.StandardOutput.ReadLine(); // "PathPilot status server ... listening on ..."
                    if (line != null && line.Contains("listening")) return process;

                    try { process.Kill(); } catch { }
                }
                catch (Win32Exception)
                {
                    // Not installed under this name
                }
            }

            return null;
        }

        private static int GetFreePort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        private static Modules.Module CreateModule(IMTConnectAgentBroker agent, int port, Dictionary<string, object> settings = null)
        {
            var configuration = new Dictionary<string, object>
            {
                { "deviceKey", DeviceName },
                { "server", "127.0.0.1" },
                { "port", port },
                { "readInterval", 50 }
            };

            if (settings != null)
            {
                foreach (var setting in settings) configuration[setting.Key] = setting.Value;
            }

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

        private static bool HasValue(IMTConnectAgentBroker agent, string name)
        {
            var value = GetValue(agent, name);
            return value != null && value != "UNAVAILABLE";
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
    }
}
