// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MTConnect;
using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Formatters;
using Xunit;

namespace MTConnect.Tests.Integration.Workflows
{
    // Workflow W06b — MQTT Probe envelope round-trip on a real broker.
    //
    // The companion suite MqttRelayWorkflowTests pins the Current path.
    // This suite pins the Probe path's cppagent JSON v2 envelope shape
    // end-to-end through a real Mosquitto broker, then back through the
    // production JsonMqttResponseDocumentFormatter on the consumer side.
    //
    // The central regression this protects against is the pre-fix read
    // path that collapsed multi-device Probe envelopes to a single device
    // — see PR #165 and the bug report at
    //   extra-files.bak/issues-to-fix/mtconnect.net/14-agent-meta-device-
    //   collapsed-in-cppagent-json-v2-encoder.md
    //
    // Source authority:
    //   - https://docs.mtconnect.org/ Part 6.0 "MTConnect Standard - MQTT
    //     Protocol" — Probe topic structure and payload semantics.
    //   - https://github.com/mtconnect/cppagent — reference C++ encoder
    //     whose JSON v2 wire shape MTConnect.NET must match.
    //   - MTConnect v2.7 XSD DevicesType (MTConnectDevices_2.7.xsd lines
    //     5029-5051): Agent (minOccurs=0, maxOccurs=1) and Device
    //     (minOccurs=1, maxOccurs=unbounded) are SEPARATE named child
    //     elements within the DevicesType sequence.
    [Trait("Category", "RequiresDocker")]
    public sealed class JsonMqttProbeEnvelopeIntegrationTests
        : IClassFixture<MqttBrokerFixture>, IDisposable
    {
        private const string TopicPrefix = "MTConnectProbeEnv";
        private const string DocumentFormat = "json-cppAgent-mqtt";

        // Three machine devices + the implicit Agent meta-device cover
        // the multi-device pre-fix collapse regression and the Agent-vs-
        // Device typing split.
        private const string Device1Uuid = "ProbeEnv-DEV-1";
        private const string Device1Name = "VMC-3Axis";
        private const string Device2Uuid = "ProbeEnv-DEV-2";
        private const string Device2Name = "Lathe";
        private const string Device3Uuid = "ProbeEnv-DEV-3";
        private const string Device3Name = "LinuxCNC-Mixed_Case";

        private readonly MqttBrokerFixture _broker;
        private readonly IMTConnectAgentBroker _agent;
        private readonly object _module;
        private readonly MethodInfo _stopMethod;

        public JsonMqttProbeEnvelopeIntegrationTests(MqttBrokerFixture broker)
        {
            _broker = broker;

            var agentConfig = new AgentConfiguration
            {
                DefaultVersion = MTConnectVersions.Version27,
            };
            _agent = new MTConnectAgentBroker(agentConfig);
            _agent.Start();

            _agent.AddDevice(BuildDevice(Device1Uuid, "d1", Device1Name));
            _agent.AddDevice(BuildDevice(Device2Uuid, "d2", Device2Name));
            _agent.AddDevice(BuildDevice(Device3Uuid, "d3", Device3Name));

            var moduleConfig = new MqttRelayModuleConfiguration
            {
                Server = _broker.Host,
                Port = _broker.Port,
                ClientId = $"mtconnect-probe-envelope-{Guid.NewGuid():N}",
                Qos = 1,
                ReconnectInterval = 500,
                Timeout = 5000,
                TopicPrefix = TopicPrefix,
                TopicStructure = MqttTopicStructure.Document,
                DocumentFormat = DocumentFormat,
                CurrentInterval = 1000,
                SampleInterval = 1000,
            };

            // Same reflection-load pattern as MqttRelayWorkflowTests so
            // we exercise the same module the agent host instantiates at
            // runtime via configuration discovery, not a test double.
            var moduleType = Type.GetType(
                "MTConnect.Module, MTConnect.NET-AgentModule-MqttRelay",
                throwOnError: true)!;

            _module = Activator.CreateInstance(moduleType, _agent, moduleConfig)!;
            var startMethod = moduleType.GetMethod("StartAfterLoad", new[] { typeof(bool) })!;
            _stopMethod = moduleType.GetMethod("Stop")!;
            startMethod.Invoke(_module, new object[] { true });
        }

        public void Dispose()
        {
            try { _stopMethod.Invoke(_module, null); }
            catch { }
            _agent.Stop();
        }


        [Fact]
        public async Task Probe_envelope_multi_device_round_trip_preserves_every_device()
        {
            // Subscribe to every Probe topic published under the prefix.
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
                    // Every machine device + the Agent meta-device get
                    // their own /Probe/{uuid} topic. We don't know the
                    // agent UUID statically, so wait for at least the
                    // three machine devices plus one more (the agent).
                    if (HasMachineDevices(received) && received.Count >= 4)
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
            Assert.True(
                completed == allArrived.Task,
                $"Did not receive Probe payloads for every device within 30s "
                + $"(received {received.Count} topics: "
                + $"{string.Join(", ", received.Keys)}).");

            // Inspect every received Probe payload: each must wrap content
            // in MTConnectDevices.Devices as a JSON object with Device[]
            // (and, on the agent topic, Agent[]).
            foreach (var (topic, payload) in received)
            {
                var json = Encoding.UTF8.GetString(payload);
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                Assert.True(
                    root.TryGetProperty("MTConnectDevices", out var envelope),
                    $"Probe payload on {topic} must wrap content in MTConnectDevices.");
                Assert.True(
                    envelope.TryGetProperty("Devices", out var devicesNode),
                    $"MTConnectDevices on {topic} must include a Devices child.");
                Assert.Equal(JsonValueKind.Object, devicesNode.ValueKind);

                // Either Agent[] or Device[] (or both) must be present —
                // the cppagent JSON v2 keyed shape.
                var hasAgent = devicesNode.TryGetProperty("Agent", out _);
                var hasDevice = devicesNode.TryGetProperty("Device", out _);
                Assert.True(
                    hasAgent || hasDevice,
                    $"Devices on {topic} must contain at least one of Agent[] or Device[].");
            }
        }


        [Fact]
        public async Task Probe_envelope_round_trip_through_formatter_preserves_Agent_name_case()
        {
            // The Device3 fixture name "LinuxCNC-Mixed_Case" sets up a
            // case-sensitivity probe. Subscribe, capture its Probe payload,
            // and parse it back through the production formatter. The
            // round-tripped Device.Name must match the source byte-for-byte
            // — no lowercase / case normalisation anywhere in the path.
            using var subscriber = await ConnectSubscriberAsync().ConfigureAwait(false);

            var matched = new TaskCompletionSource<byte[]>(
                TaskCreationOptions.RunContinuationsAsynchronously);
            subscriber.ApplicationMessageReceivedAsync += args =>
            {
                var msg = args.ApplicationMessage;
                if (!msg.Topic.EndsWith($"/Probe/{Device3Uuid}", StringComparison.Ordinal))
                {
                    return Task.CompletedTask;
                }
                var bytes = msg.PayloadSegment.Array != null
                    ? msg.PayloadSegment.ToArray()
                    : Array.Empty<byte>();
                if (bytes.Length == 0) return Task.CompletedTask;
                matched.TrySetResult(bytes);
                return Task.CompletedTask;
            };

            await subscriber.SubscribeAsync(
                new MqttClientSubscribeOptionsBuilder()
                    .WithTopicFilter(
                        $"{TopicPrefix}/Probe/{Device3Uuid}",
                        MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                    .Build()).ConfigureAwait(false);

            var completed = await Task.WhenAny(
                matched.Task,
                Task.Delay(TimeSpan.FromSeconds(30))).ConfigureAwait(false);
            Assert.True(
                completed == matched.Task,
                $"Did not receive a Probe payload on /Probe/{Device3Uuid} within 30s.");

            var payload = await matched.Task.ConfigureAwait(false);

            // Surface inspection: case is preserved on the wire.
            var json = Encoding.UTF8.GetString(payload);
            Assert.Contains(Device3Name, json);

            // Parse back through the production formatter; the symmetric
            // read path (15d70abe) routes through JsonDevicesResponseDocument.
            var formatter = new JsonMqttResponseDocumentFormatter();
            using var stream = new System.IO.MemoryStream(payload);
            var readResult = formatter.CreateDevicesResponseDocument(stream);

            Assert.True(
                readResult.Success,
                "Formatter must round-trip the live Probe payload.");

            var devices = readResult.Content.Devices.ToList();
            var match = devices.FirstOrDefault(d => d.Uuid == Device3Uuid);
            Assert.NotNull(match);
            Assert.Equal(
                Device3Name,
                match!.Name);
        }


        private async Task<IMqttClient> ConnectSubscriberAsync()
        {
            var factory = new MqttFactory();
            var client = factory.CreateMqttClient();
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer(_broker.Host, _broker.Port)
                .WithClientId($"mtconnect-probe-subscriber-{Guid.NewGuid():N}")
                .WithCleanSession(true)
                .Build();
            await client.ConnectAsync(options, CancellationToken.None).ConfigureAwait(false);
            return client;
        }

        private static bool HasMachineDevices(IReadOnlyDictionary<string, byte[]> received)
        {
            return received.Keys.Any(k => k.EndsWith($"/Probe/{Device1Uuid}", StringComparison.Ordinal))
                && received.Keys.Any(k => k.EndsWith($"/Probe/{Device2Uuid}", StringComparison.Ordinal))
                && received.Keys.Any(k => k.EndsWith($"/Probe/{Device3Uuid}", StringComparison.Ordinal));
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
