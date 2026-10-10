// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Agents;
using MTConnect.Fanuc;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTConnect.AgentModule.FanucAdapter.Tests
{
    /// <summary>
    /// Runs the module against a simulated Fanuc CNC and checks the observations
    /// and Device model that reach the Agent.
    /// </summary>
    [TestFixture]
    public class FanucModuleTests
    {
        private const string DeviceName = "M1";

        /// <summary>Observations are read from the CNC and added to the generated Device.</summary>
        [Test]
        public async Task Module_reports_observations()
        {
            var client = FakeFocasClient.CreateMill();
            var agent = new MTConnectAgentBroker();
            var module = CreateModule(agent, client);

            try
            {
                await WaitFor(() => GetValue(agent, "cycleTime") == "42.25" && GetValue(agent, "a_act") != null);

                Assert.That(GetValue(agent, "avail"), Is.EqualTo("AVAILABLE"));
                Assert.That(GetValue(agent, "estop"), Is.EqualTo("ARMED"));
                Assert.That(GetValue(agent, "mode"), Is.EqualTo("AUTOMATIC"));
                Assert.That(GetValue(agent, "execution"), Is.EqualTo("ACTIVE"));
                Assert.That(GetValue(agent, "program"), Is.EqualTo("O1234"));
                Assert.That(GetValue(agent, "line"), Is.EqualTo("120"));
                Assert.That(GetValue(agent, "partCount"), Is.EqualTo("42"));
                Assert.That(GetValue(agent, "tool"), Is.EqualTo("5"));
                Assert.That(GetValue(agent, "workOffset"), Is.EqualTo("54"));
                Assert.That(GetValue(agent, "feedrate"), Is.EqualTo("10"), "600 mm/min = 10 mm/s");
                Assert.That(GetValue(agent, "speed"), Is.EqualTo("8000"));
                Assert.That(GetValue(agent, "spindleLoad"), Is.EqualTo("35.5"));
                Assert.That(GetValue(agent, "feedOverride"), Is.EqualTo("100"));
                Assert.That(GetValue(agent, "rapidOverride"), Is.EqualTo("50"));
                Assert.That(GetValue(agent, "spindleOverride"), Is.EqualTo("110"));
                Assert.That(GetValue(agent, "x_act"), Is.EqualTo("38.1"), "inches are converted to millimeters");
                Assert.That(GetValue(agent, "a_act"), Is.EqualTo("90.5"), "rotary axes are not converted");
                Assert.That(GetValue(agent, "x_load"), Is.EqualTo("12.3"));
                Assert.That(GetValue(agent, "system", "Level"), Is.EqualTo("NORMAL"));
                Assert.That(GetValue(agent, "firmware"), Is.EqualTo("G421 15.0"));
                Assert.That(GetValue(agent, "powerOnTime"), Is.EqualTo("3600"));
                Assert.That(GetValue(agent, "operatingTime"), Is.EqualTo("1800.5"));
                Assert.That(GetValue(agent, "cuttingTime"), Is.EqualTo("900"));
            }
            finally
            {
                module.Stop();
            }
        }

        /// <summary>The Device is updated with the model, axes and spindle read from the CNC.</summary>
        [Test]
        public async Task Module_updates_device_from_cnc()
        {
            var client = FakeFocasClient.CreateMill();
            var agent = new MTConnectAgentBroker();
            var module = CreateModule(agent, client, new Dictionary<string, object> { { "workPositions", true } });

            try
            {
                await WaitFor(() => agent.GetDevice(DeviceName)?.Description?.Model == "30i-M");

                var device = agent.GetDevice(DeviceName);
                var axes = device.GetComponents().Where(o => o.Type == "Linear" || o.Type == "Rotary").Select(o => $"{o.Type}:{o.Name}").ToList();
                Assert.That(axes, Is.EquivalentTo(new[] { "Linear:X", "Linear:Y", "Linear:Z", "Rotary:A", "Rotary:S1" }));

                var dataItems = device.GetDataItems().ToList();
                Assert.That(dataItems.Single(o => o.Name == "a_act").Type, Is.EqualTo("ANGLE"));
                Assert.That(dataItems.Single(o => o.Name == "x_work").CoordinateSystemIdRef, Is.EqualTo("M1_work"));

                await WaitFor(() => GetValue(agent, "x_work") == "12.7");
                Assert.That(GetValue(agent, "a_work"), Is.EqualTo("45"));
            }
            finally
            {
                module.Stop();
            }
        }

        /// <summary>Each active alarm is a separate FAULT on the system condition; cleared alarms are removed.</summary>
        [Test]
        public async Task Module_reports_alarms()
        {
            var client = FakeFocasClient.CreateMill();
            var path = client.Paths[0];
            path.Status = new FocasStatus { Mode = 1, Run = 0, Alarm = 1 };
            path.Alarms.Add(new FocasAlarm { Type = 3, Number = 10, Message = "IMPROPER G-CODE" });
            path.Alarms.Add(new FocasAlarm { Type = 6, Number = 401, Message = "V READY OFF" });

            var agent = new MTConnectAgentBroker();
            var module = CreateModule(agent, client);

            try
            {
                await WaitFor(() => GetConditions(agent).Count(o => o.GetValue("Level") == "FAULT") == 2);

                var codes = GetConditions(agent).Select(o => o.GetValue("NativeCode")).ToList();
                Assert.That(codes, Is.SupersetOf(new[] { "PS0010", "SV0401" }));
                Assert.That(GetConditions(agent).Single(o => o.GetValue("NativeCode") == "PS0010").GetValue("Message"), Is.EqualTo("IMPROPER G-CODE"));

                // Clear one alarm
                path.Alarms.RemoveAt(0);
                await WaitFor(() => GetConditions(agent).Count(o => o.GetValue("Level") == "FAULT") == 1);
                Assert.That(GetConditions(agent).Single(o => o.GetValue("Level") == "FAULT").GetValue("NativeCode"), Is.EqualTo("SV0401"));

                // Clear every alarm
                path.Alarms.Clear();
                path.Status = new FocasStatus { Mode = 1, Run = 0, Alarm = 0 };
                await WaitFor(() => GetConditions(agent).All(o => o.GetValue("Level") == "NORMAL"));
            }
            finally
            {
                module.Stop();
            }
        }

        /// <summary>Warnings (ex. battery low) are reported as WARNING.</summary>
        [Test]
        public async Task Module_reports_warnings()
        {
            var client = FakeFocasClient.CreateMill();
            client.Paths[0].Status = new FocasStatus { Mode = 1, Run = 3, Alarm = 2 };

            var agent = new MTConnectAgentBroker();
            var module = CreateModule(agent, client);

            try
            {
                await WaitFor(() => GetValue(agent, "system", "Level") == "WARNING");
                Assert.That(GetValue(agent, "system", "Message"), Is.EqualTo("BATTERY LOW"));
            }
            finally
            {
                module.Stop();
            }
        }

        /// <summary>Emergency stop is reported as TRIGGERED and stops execution.</summary>
        [Test]
        public async Task Module_reports_emergency_stop()
        {
            var client = FakeFocasClient.CreateMill();
            client.Paths[0].Status = new FocasStatus { Mode = 5, Run = 0, Emergency = 1 };

            var agent = new MTConnectAgentBroker();
            var module = CreateModule(agent, client);

            try
            {
                await WaitFor(() => GetValue(agent, "estop") == "TRIGGERED");
                Assert.That(GetValue(agent, "execution"), Is.EqualTo("STOPPED"));
                Assert.That(GetValue(agent, "mode"), Is.EqualTo("MANUAL"));
            }
            finally
            {
                module.Stop();
            }
        }

        /// <summary>Every path of a multi-path CNC is read; path 2 keys are prefixed.</summary>
        [Test]
        public async Task Module_reads_every_path()
        {
            var client = FakeFocasClient.CreateTwoPathLathe();
            var agent = new MTConnectAgentBroker();
            var module = CreateModule(agent, client);

            try
            {
                await WaitFor(() => GetValue(agent, "p2_execution") == "FEED_HOLD" && GetValue(agent, "z2_act") != null);

                Assert.That(GetValue(agent, "execution"), Is.EqualTo("ACTIVE"));
                Assert.That(GetValue(agent, "program"), Is.EqualTo("O1000"));
                Assert.That(GetValue(agent, "p2_program"), Is.EqualTo("O2000"));
                Assert.That(GetValue(agent, "speed"), Is.EqualTo("1200"));
                Assert.That(GetValue(agent, "p2_speed"), Is.EqualTo("800"));
                Assert.That(GetValue(agent, "x1_act"), Is.EqualTo("10"));
                Assert.That(GetValue(agent, "z2_act"), Is.EqualTo("-7.5"));

                var device = agent.GetDevice(DeviceName);
                Assert.That(device.Description.Model, Is.EqualTo("0i-T"));
                Assert.That(device.GetComponents().Where(o => o.Type == "Path").Select(o => o.Name), Is.EquivalentTo(new[] { "path", "path2" }));
                Assert.That(device.GetComponents().Where(o => o.Type == "Rotary").Select(o => o.Name), Is.EquivalentTo(new[] { "S1", "S2" }));
            }
            finally
            {
                module.Stop();
            }
        }

        /// <summary>Older CNCs without cnc_exeprgname / cnc_rdspeed use cnc_rdprgnum / cnc_acts; unsupported functions are called once.</summary>
        [Test]
        public async Task Module_falls_back_for_unsupported_functions()
        {
            var client = FakeFocasClient.CreateMill();
            client.Unsupported.Add("cnc_exeprgname");
            client.Unsupported.Add("cnc_rdspeed");

            var agent = new MTConnectAgentBroker();
            var module = CreateModule(agent, client);

            try
            {
                await WaitFor(() => client.CountOf("cnc_rdprgnum") >= 3);

                Assert.That(GetValue(agent, "program"), Is.EqualTo("O1234"));
                Assert.That(GetValue(agent, "speed"), Is.EqualTo("8000"));
                Assert.That(client.CountOf("cnc_exeprgname"), Is.EqualTo(1));
                Assert.That(client.CountOf("cnc_rdspeed"), Is.EqualTo(1));
            }
            finally
            {
                module.Stop();
            }
        }

        /// <summary>A lost connection sets the Device UNAVAILABLE; it recovers when the CNC returns.</summary>
        [Test]
        public async Task Module_sets_unavailable_and_reconnects()
        {
            var client = FakeFocasClient.CreateMill();
            var agent = new MTConnectAgentBroker();
            var module = CreateModule(agent, client);

            try
            {
                await WaitFor(() => GetValue(agent, "avail") == "AVAILABLE");

                client.FailWith = FocasReturnCodes.EW_SOCKET;
                client.Offline = true;

                // Every value is set UNAVAILABLE (they are added one at a time, so wait for all of them)
                await WaitFor(() => GetValue(agent, "avail") == "UNAVAILABLE"
                    && GetValue(agent, "execution") == "UNAVAILABLE"
                    && GetValue(agent, "x_act") == "UNAVAILABLE"
                    && GetValue(agent, "system", "Level") == "UNAVAILABLE");

                client.FailWith = null;
                client.Offline = false;

                await WaitFor(() => GetValue(agent, "avail") == "AVAILABLE");
                Assert.That(client.ConnectCount, Is.EqualTo(2));
            }
            finally
            {
                module.Stop();
            }
        }

        /// <summary>Timers are read less often than the other values.</summary>
        [Test]
        public async Task Module_reads_timers_less_often()
        {
            var client = FakeFocasClient.CreateMill();
            var agent = new MTConnectAgentBroker();
            var module = CreateModule(agent, client, new Dictionary<string, object> { { "slowReadInterval", 60000 } });

            try
            {
                await WaitFor(() => client.CountOf("cnc_statinfo") >= 5);
                Assert.That(client.CountOf("cnc_rdtimer"), Is.EqualTo(4), "4 timers read once");
            }
            finally
            {
                module.Stop();
            }
        }


        private static Modules.Module CreateModule(IMTConnectAgentBroker agent, IFocasClient client, Dictionary<string, object> settings = null)
        {
            var configuration = new Dictionary<string, object>
            {
                { "deviceKey", DeviceName },
                { "server", "127.0.0.1" },
                { "readInterval", 50 }
            };

            if (settings != null)
            {
                foreach (var setting in settings) configuration[setting.Key] = setting.Value;
            }

            var module = new Modules.Module(agent, configuration, client);
            module.StartBeforeLoad(true);
            module.StartAfterLoad(true);
            return module;
        }

        private static string GetValue(IMTConnectAgentBroker agent, string name, string valueKey = "Result")
        {
            var observation = agent.GetCurrentObservations(DeviceName)?.LastOrDefault(o => o.Name == name);
            return observation?.GetValue(valueKey);
        }

        private static List<Observations.Output.IObservationOutput> GetConditions(IMTConnectAgentBroker agent)
        {
            return agent.GetCurrentObservations(DeviceName)?.Where(o => o.Name == "system").ToList() ?? new List<Observations.Output.IObservationOutput>();
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
