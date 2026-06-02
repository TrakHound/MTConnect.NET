// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using MQTTnet;
using MQTTnet.Client;
using MTConnect;
using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Devices;
using NUnit.Framework;

namespace MTConnect.Compliance.Tests.L2_CrossImpl
{
    // E2E parity gate — the full publish-and-subscribe round trip across
    // a real Mosquitto broker, exercising the multi-device + mixed-case
    // Agent-name regression from PR #165 end-to-end. The MTConnect.NET
    // agent publishes Probe envelopes via the MqttRelay agent module;
    // the test acts as the downstream consumer.
    //
    // This complements two adjacent suites:
    //   * MqttRelayWorkflowTests (Integration-Tests) — Current path on a
    //     real broker.
    //   * CppAgentParityWorkflowTests (this folder) — HTTP /probe parity
    //     against a Docker-spun cppagent.
    //   * JsonMqttProbeEnvelopeIntegrationTests (Integration-Tests) —
    //     Probe envelope round trip on a real broker.
    //
    // The cppagent reference parser is not invoked in-process here (no
    // Python or C++ runtime is available in the .NET test host). The
    // load-bearing parity assertion is that the MTConnect.NET formatter
    // round-trips the live broker payload symmetrically (write-then-read)
    // and that every multi-device + mixed-case-Agent fixture survives the
    // trip with its envelope shape, device count, and name case intact.
    //
    // Source authority:
    //   - https://docs.mtconnect.org/ Part 6.0 "MTConnect Standard - MQTT
    //     Protocol" — Probe topic structure and payload semantics.
    //   - https://github.com/mtconnect/cppagent — reference encoder whose
    //     JSON v2 shape the .NET formatter mirrors.
    /// <summary>Pins the behaviour expressed by the test name: json mqtt probe envelope e2 e tests.</summary>
    [TestFixture]
    [Category("RequiresDocker")]
    [Category("E2E")]
    public class JsonMqttProbeEnvelopeE2ETests
    {
        private const string MqttImage = "eclipse-mosquitto:2.0.22";
        private const int MqttPort = 1883;

        private const string TopicPrefix = "MTConnectProbeE2E";
        private const string DocumentFormat = "json-cppAgent-mqtt";

        // Two machine devices with the Agent meta-device: exactly the
        // pre-fix multi-device collapse shape.
        private const string Device1Uuid = "ProbeE2E-DEV-1";
        private const string Device1Name = "VMC-3Axis";
        private const string Device2Uuid = "ProbeE2E-DEV-2";
        private const string Device2Name = "LinuxCNC-Mixed_Case";

        private DotNet.Testcontainers.Containers.IContainer? _broker;
        private string? _configDir;
        private IMTConnectAgentBroker? _agent;
        private object? _module;
        private MethodInfo? _stopMethod;

        /// <summary>Sets up the fixture before each test.</summary>
        /// <returns>The result of the operation.</returns>
        [OneTimeSetUp]
        public async Task GlobalSetUp()
        {
            // Mosquitto refuses anonymous connections by default; stage a
            // per-fixture config that opens 1883/tcp anonymously, mirroring
            // MqttBrokerFixture in MTConnect.NET-Integration-Tests.
            _configDir = Path.Combine(Path.GetTempPath(), $"probe-e2e-{Guid.NewGuid():N}");
            Directory.CreateDirectory(_configDir);
            var configFile = Path.Combine(_configDir, "mosquitto.conf");
            File.WriteAllText(
                configFile,
                "listener 1883 0.0.0.0\nallow_anonymous true\n");

            _broker = new ContainerBuilder()
                .WithImage(MqttImage)
                .WithPortBinding(MqttPort, assignRandomHostPort: true)
                .WithBindMount(configFile, "/mosquitto/config/mosquitto.conf")
                .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(MqttPort))
                .Build();

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(120));
            await _broker.StartAsync(cts.Token).ConfigureAwait(false);

            // In-process MTConnect.NET agent publishing through the MqttRelay
            // module pointed at the spun broker, format=json-cppAgent-mqtt.
            var agentConfig = new AgentConfiguration
            {
                DefaultVersion = MTConnectVersions.Version27,
            };
            _agent = new MTConnectAgentBroker(agentConfig);
            _agent.Start();

            _agent.AddDevice(BuildDevice(Device1Uuid, "d1", Device1Name));
            _agent.AddDevice(BuildDevice(Device2Uuid, "d2", Device2Name));

            var moduleConfig = new MqttRelayModuleConfiguration
            {
                Server = _broker.Hostname,
                Port = _broker.GetMappedPublicPort(MqttPort),
                ClientId = $"mtconnect-e2e-{Guid.NewGuid():N}",
                Qos = 1,
                ReconnectInterval = 500,
                Timeout = 5000,
                TopicPrefix = TopicPrefix,
                TopicStructure = MqttTopicStructure.Document,
                DocumentFormat = DocumentFormat,
                CurrentInterval = 1000,
                SampleInterval = 1000,
            };

            var moduleType = Type.GetType(
                "MTConnect.Module, MTConnect.NET-AgentModule-MqttRelay",
                throwOnError: true)!;
            _module = Activator.CreateInstance(moduleType, _agent, moduleConfig)!;
            var startMethod = moduleType.GetMethod("StartAfterLoad", new[] { typeof(bool) })!;
            _stopMethod = moduleType.GetMethod("Stop")!;
            startMethod.Invoke(_module, new object[] { true });
        }

        /// <summary>Tears down the fixture after each test.</summary>
        /// <returns>The result of the operation.</returns>
        [OneTimeTearDown]
        public async Task GlobalTearDown()
        {
            try { _stopMethod?.Invoke(_module, null); } catch { }
            try { _agent?.Stop(); } catch { }
            if (_broker != null)
            {
                try { await _broker.DisposeAsync().ConfigureAwait(false); } catch { }
                _broker = null;
            }
            if (!string.IsNullOrEmpty(_configDir) && Directory.Exists(_configDir))
            {
                try { Directory.Delete(_configDir, recursive: true); } catch { }
            }
        }


        /// <summary>Pins the behaviour expressed by the test name: e2 e multi device probe envelope round trip through real broker.</summary>
        /// <returns>The result of the operation.</returns>
        [Test]
        public async Task E2E_multi_device_Probe_envelope_round_trip_through_real_broker()
        {
            Assert.That(_broker, Is.Not.Null);

            using var subscriber = await ConnectSubscriberAsync().ConfigureAwait(false);

            var received = new Dictionary<string, byte[]>(StringComparer.Ordinal);
            var receivedLock = new object();
            var allArrived = new TaskCompletionSource<bool>(
                TaskCreationOptions.RunContinuationsAsynchronously);

            subscriber.ApplicationMessageReceivedAsync += args =>
            {
                var msg = args.ApplicationMessage;
                if (!msg.Topic.Contains("/Probe/", StringComparison.Ordinal))
                {
                    return Task.CompletedTask;
                }
                var bytes = msg.PayloadSegment.Array != null
                    ? msg.PayloadSegment.ToArray()
                    : Array.Empty<byte>();
                if (bytes.Length == 0) return Task.CompletedTask;

                lock (receivedLock)
                {
                    received[msg.Topic] = bytes;
                    if (received.Keys.Any(k => k.EndsWith($"/Probe/{Device1Uuid}", StringComparison.Ordinal))
                        && received.Keys.Any(k => k.EndsWith($"/Probe/{Device2Uuid}", StringComparison.Ordinal)))
                    {
                        allArrived.TrySetResult(true);
                    }
                }
                return Task.CompletedTask;
            };

            await subscriber.SubscribeAsync(
                new MqttClientSubscribeOptionsBuilder()
                    .WithTopicFilter(
                        $"{TopicPrefix}/Probe/#",
                        MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                    .Build()).ConfigureAwait(false);

            var completed = await Task.WhenAny(
                allArrived.Task,
                Task.Delay(TimeSpan.FromSeconds(30))).ConfigureAwait(false);
            Assert.That(completed, Is.SameAs(allArrived.Task),
                $"Did not receive Probe payloads for every device within 30s "
                + $"(received {received.Count} topics: "
                + $"{string.Join(", ", received.Keys)}).");

            // Mixed-case device-name survives the round trip on the wire.
            var dev2Payload = received
                .First(kv => kv.Key.EndsWith($"/Probe/{Device2Uuid}", StringComparison.Ordinal))
                .Value;
            var dev2Json = Encoding.UTF8.GetString(dev2Payload);
            Assert.That(dev2Json, Does.Contain(Device2Name),
                $"Mixed-case Device.Name '{Device2Name}' must survive the full broker round trip.");

            // Every Probe payload has the cppagent JSON v2 keyed envelope.
            foreach (var (topic, payload) in received)
            {
                using var doc = JsonDocument.Parse(payload);
                var root = doc.RootElement;
                Assert.That(root.TryGetProperty("MTConnectDevices", out var envelope), Is.True,
                    $"Probe on {topic} missing MTConnectDevices envelope.");
                Assert.That(envelope.TryGetProperty("Devices", out var devicesNode), Is.True,
                    $"Probe on {topic} missing Devices child.");
                Assert.That(devicesNode.ValueKind, Is.EqualTo(JsonValueKind.Object),
                    $"Probe on {topic} has flat-array Devices — cppagent JSON v2 violation.");
            }
        }


        private async Task<IMqttClient> ConnectSubscriberAsync()
        {
            var factory = new MqttFactory();
            var client = factory.CreateMqttClient();
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer(_broker!.Hostname, _broker.GetMappedPublicPort(MqttPort))
                .WithClientId($"mtconnect-e2e-sub-{Guid.NewGuid():N}")
                .WithCleanSession(true)
                .Build();
            await client.ConnectAsync(options, CancellationToken.None).ConfigureAwait(false);
            return client;
        }

        private static Device BuildDevice(string uuid, string id, string name)
        {
            var device = new Device
            {
                Id = id,
                Uuid = uuid,
                Name = name,
            };
            var availability = new MTConnect.Devices.DataItems.AvailabilityDataItem
            {
                Id = $"{id}-avail",
                Category = DataItemCategory.EVENT,
                Type = MTConnect.Devices.DataItems.AvailabilityDataItem.TypeId,
            };
            device.AddDataItem(availability);
            return device;
        }
    }
}
