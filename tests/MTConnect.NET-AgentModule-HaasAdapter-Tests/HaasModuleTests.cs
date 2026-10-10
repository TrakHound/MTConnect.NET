// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Agents;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
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
        [TestCase(false)]
        [TestCase(true)]
        public async Task Module_reports_observations(bool framed)
        {
            using (var control = new FakeHaasControl { Framed = framed })
            {
                var agent = new MTConnectAgentBroker();
                var module = CreateModule(agent, control.Port, new Dictionary<string, object> { { "units", "inch" } });

                try
                {
                    await WaitFor(() => GetValue(agent, "spindleLoad") == "23.4" && GetValue(agent, "optionalStop") == "OFF");

                    // Fast group
                    Assert.That(GetValue(agent, "avail"), Is.EqualTo("AVAILABLE"));
                    Assert.That(GetValue(agent, "mode"), Is.EqualTo("AUTOMATIC"));
                    Assert.That(GetValue(agent, "execution"), Is.EqualTo("READY"));
                    Assert.That(GetValue(agent, "program"), Is.EqualTo("O01234"));
                    Assert.That(GetValue(agent, "partCount"), Is.EqualTo("42"));
                    Assert.That(GetValue(agent, "tool"), Is.EqualTo("4"));
                    Assert.That(GetValue(agent, "x_act"), Is.EqualTo("38.1"), "inches are converted to millimeters");
                    Assert.That(GetValue(agent, "speed"), Is.EqualTo("8000"));
                    Assert.That(GetValue(agent, "system", "Level"), Is.EqualTo("NORMAL"));

                    // Slow group
                    Assert.That(GetValue(agent, "firmware"), Is.EqualTo("100.24.000.1200"));
                    Assert.That(GetValue(agent, "powerOnTime"), Is.EqualTo("22616233"));
                    Assert.That(GetValue(agent, "cycleStartTime"), Is.EqualTo("353909"));
                    Assert.That(GetValue(agent, "lastCycleTime"), Is.EqualTo("13"));
                    Assert.That(GetValue(agent, "previousCycleTime"), Is.EqualTo("65"));
                    Assert.That(GetValue(agent, "partCount2"), Is.EqualTo("17"));
                    Assert.That(GetValue(agent, "workOffset"), Is.EqualTo("54"));
                    Assert.That(GetValue(agent, "singleBlock"), Is.EqualTo("ON"));
                }
                finally
                {
                    module.Stop();
                }
            }
        }

        /// <summary>The Device Description is updated with the model and serial number read from the control.</summary>
        [Test]
        public async Task Module_updates_device_description()
        {
            using (var control = new FakeHaasControl())
            {
                var agent = new MTConnectAgentBroker();
                var module = CreateModule(agent, control.Port);

                try
                {
                    await WaitFor(() => agent.GetDevice(DeviceName)?.Description?.Model == "VF-2");

                    Assert.That(agent.GetDevice(DeviceName).Description.SerialNumber, Is.EqualTo("1234567"));
                    Assert.That(agent.GetDevice(DeviceName).Description.Manufacturer, Is.EqualTo("Haas"));

                    // Observations continue after the Device is updated
                    await Task.Delay(300);
                    Assert.That(GetValue(agent, "avail"), Is.EqualTo("AVAILABLE"));
                    Assert.That(GetValue(agent, "execution"), Is.EqualTo("READY"));
                }
                finally
                {
                    module.Stop();
                }
            }
        }

        /// <summary>The control may not answer the first ?Q100 after connecting; it is resent without reconnecting.</summary>
        [Test]
        public async Task Module_retries_first_command_without_reconnecting()
        {
            using (var control = new FakeHaasControl { IgnoreFirstQ100 = true })
            {
                var agent = new MTConnectAgentBroker();
                var module = CreateModule(agent, control.Port, new Dictionary<string, object> { { "responseTimeout", 300 } });

                try
                {
                    await WaitFor(() => GetValue(agent, "avail") == "AVAILABLE");
                    await Task.Delay(500);

                    Assert.That(GetValue(agent, "avail"), Is.EqualTo("AVAILABLE"));
                    Assert.That(control.ConnectionCount, Is.EqualTo(1));
                }
                finally
                {
                    module.Stop();
                }
            }
        }

        /// <summary>Replies to commands sent by other clients connected to the control are ignored.</summary>
        [Test]
        public async Task Module_ignores_replies_to_other_clients()
        {
            using (var control = new FakeHaasControl { SendOtherClientReplies = true })
            {
                var agent = new MTConnectAgentBroker();
                var module = CreateModule(agent, control.Port);

                try
                {
                    await WaitFor(() => GetValue(agent, "partCount2") == "17");

                    Assert.That(GetValue(agent, "mode"), Is.EqualTo("AUTOMATIC"));
                    Assert.That(GetValue(agent, "partCount"), Is.EqualTo("42"));
                    Assert.That(GetValue(agent, "tool"), Is.EqualTo("4"));
                    Assert.That(control.ConnectionCount, Is.EqualTo(1));
                }
                finally
                {
                    module.Stop();
                }
            }
        }

        /// <summary>A command the control does not recognize is only sent once.</summary>
        [Test]
        public async Task Module_stops_reading_unsupported_commands()
        {
            using (var control = new FakeHaasControl())
            {
                control.UnsupportedCommands.Add("?Q303");

                var agent = new MTConnectAgentBroker();
                var module = CreateModule(agent, control.Port, new Dictionary<string, object> { { "slowReadInterval", 100 } });

                try
                {
                    await WaitFor(() => control.CountOf("?Q304") >= 3);

                    Assert.That(control.CountOf("?Q303"), Is.EqualTo(1));
                    Assert.That(GetValue(agent, "lastCycleTime"), Is.EqualTo("UNAVAILABLE"));
                    Assert.That(GetValue(agent, "previousCycleTime"), Is.EqualTo("65"));
                    Assert.That(GetValue(agent, "avail"), Is.EqualTo("AVAILABLE"));
                }
                finally
                {
                    module.Stop();
                }
            }
        }

        /// <summary>Slowly changing values are read less often than the others.</summary>
        [Test]
        public async Task Module_reads_slow_values_less_often()
        {
            using (var control = new FakeHaasControl())
            {
                var agent = new MTConnectAgentBroker();
                var module = CreateModule(agent, control.Port, new Dictionary<string, object> { { "slowReadInterval", 60000 } });

                try
                {
                    await WaitFor(() => control.CountOf("?Q500") >= 5);

                    Assert.That(control.CountOf("?Q300"), Is.EqualTo(1));
                    Assert.That(control.CountOf("?Q101"), Is.EqualTo(1));
                }
                finally
                {
                    module.Stop();
                }
            }
        }

        /// <summary>The NGC reports zero return mode as "MODE, ZERO".</summary>
        [Test]
        public async Task Module_maps_zero_return_mode()
        {
            using (var control = new FakeHaasControl { Mode = "ZERO" })
            {
                var agent = new MTConnectAgentBroker();
                var module = CreateModule(agent, control.Port);

                try
                {
                    await WaitFor(() => GetValue(agent, "mode") == "MANUAL");
                }
                finally
                {
                    module.Stop();
                }
            }
        }

        /// <summary>Rotary axes, work positions and coolant level are read when configured.</summary>
        [Test]
        public async Task Module_reports_optional_data()
        {
            using (var control = new FakeHaasControl())
            {
                var agent = new MTConnectAgentBroker();
                var module = CreateModule(agent, control.Port, new Dictionary<string, object>
                {
                    { "units", "inch" },
                    { "axes", new List<string> { "X", "Y", "Z", "A" } },
                    { "workPositions", true },
                    { "coolantLevel", true },
                    { "spindleLoad", false }
                });

                try
                {
                    await WaitFor(() => GetValue(agent, "a_work") == "45" && GetValue(agent, "coolantLevel") == "75");

                    Assert.That(GetValue(agent, "a_act"), Is.EqualTo("90.5"), "rotary axes are not converted");
                    Assert.That(GetValue(agent, "x_work"), Is.EqualTo("12.7"));
                    Assert.That(GetValue(agent, "spindleLoad"), Is.Null, "spindle load is not part of the Device");
                    Assert.That(control.CountOf("?Q600 1098"), Is.EqualTo(0));

                    var dataItems = agent.GetDevice(DeviceName).GetDataItems().ToList();
                    Assert.That(dataItems.Single(o => o.Name == "a_act").Type, Is.EqualTo("ANGLE"));
                    Assert.That(dataItems.Single(o => o.Name == "x_work").CoordinateSystem, Is.EqualTo(Devices.DataItemCoordinateSystem.WORK));
                    Assert.That(dataItems.Single(o => o.Name == "x_work").CoordinateSystemIdRef, Is.EqualTo("VF2_work"));
                    Assert.That(agent.GetDevice(DeviceName).Configuration.CoordinateSystems.Single().Type, Is.EqualTo(Devices.Configurations.CoordinateSystemType.OBJECT));
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
                Assert.That(GetValue(agent, "powerOnTime"), Is.EqualTo("UNAVAILABLE"));
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


        private static Modules.Module CreateModule(IMTConnectAgentBroker agent, int port, Dictionary<string, object> settings = null)
        {
            var configuration = new Dictionary<string, object>
            {
                { "deviceKey", DeviceName },
                { "server", "127.0.0.1" },
                { "port", port },
                { "readInterval", 100 },
                { "responseTimeout", 1000 }
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
