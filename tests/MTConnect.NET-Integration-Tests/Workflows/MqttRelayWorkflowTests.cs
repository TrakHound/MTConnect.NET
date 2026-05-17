using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MTConnect;
using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Observations;
using Xunit;

namespace MTConnect.Tests.Integration.Workflows
{
    // Workflow W06 — MQTT relay agent module: agent publishes a Current
    // document to a Mosquitto broker; a downstream consumer subscribes
    // and receives the same payload.
    //
    // Source authority:
    //   - https://docs.mtconnect.org/ Part 6.0 "MTConnect Standard - MQTT
    //     Protocol" — defines the topic structure under
    //     <prefix>/Current/{deviceUuid} and the document payload format.
    //   - https://github.com/mqtt/mqtt.org — MQTT v3.1.1 / v5 wire format
    //     used by both the relay (publisher) and the test subscriber.
    //
    // The fixture spins eclipse-mosquitto:2.0.22 on a host-mapped port,
    // boots an in-process MTConnectAgentBroker seeded with one Sample
    // DataItem, attaches the production MqttRelay agent module pointed
    // at the broker (Document topic structure, json-cppAgent format),
    // and runs a raw MQTTnet subscriber on the topic prefix.
    //
    // The Document topic structure exposes the same Current envelope the
    // HTTP /current endpoint returns; observations injected through the
    // agent show up in that envelope as soon as the relay's CurrentTimer
    // fires. The test waits for that envelope to land on the subscriber
    // and inspects the payload to confirm it carries the seeded
    // observation.
    [Trait("Category", "RequiresDocker")]
    public sealed class MqttRelayWorkflowTests : IClassFixture<MqttBrokerFixture>, IDisposable
    {
        private const string DeviceUuid = "MqttRelayWorkflow-DEVICE";
        private const string DeviceName = "MqttRelayWorkflow";
        private const string DataItemId = "x_pos";
        private const string TopicPrefix = "MTConnect";
        private const string InjectedSentinel = "12345.6789";

        private readonly MqttBrokerFixture _broker;
        private readonly IMTConnectAgentBroker _agent;
        private readonly object _module;
        private readonly MethodInfo _startMethod;
        private readonly MethodInfo _stopMethod;

        public MqttRelayWorkflowTests(MqttBrokerFixture broker)
        {
            _broker = broker;

            var agentConfig = new AgentConfiguration
            {
                DefaultVersion = MTConnectVersions.Version25,
            };
            _agent = new MTConnectAgentBroker(agentConfig);
            _agent.Start();

            var device = BuildDevice();
            _agent.AddDevice(device);

            var moduleConfig = new MqttRelayModuleConfiguration
            {
                Server = _broker.Host,
                Port = _broker.Port,
                ClientId = $"mtconnect-relay-{Guid.NewGuid():N}",
                Qos = 1,
                ReconnectInterval = 500,
                Timeout = 5000,
                TopicPrefix = TopicPrefix,
                TopicStructure = MqttTopicStructure.Document,
                DocumentFormat = "json-cppAgent",
                CurrentInterval = 250,
                SampleInterval = 250,
            };

            // The MqttRelay module type lives in the
            // MTConnect.NET-AgentModule-MqttRelay assembly under the
            // root MTConnect namespace. Reflection-load it so the test
            // exercises the same module the agent host instantiates at
            // runtime via configuration discovery.
            var moduleType = Type.GetType(
                "MTConnect.Module, MTConnect.NET-AgentModule-MqttRelay",
                throwOnError: true)!;

            _module = Activator.CreateInstance(moduleType, _agent, moduleConfig)!;
            _startMethod = moduleType.GetMethod("StartAfterLoad", new[] { typeof(bool) })!;
            _stopMethod = moduleType.GetMethod("Stop")!;

            _startMethod.Invoke(_module, new object[] { true });
        }

        public void Dispose()
        {
            try { _stopMethod.Invoke(_module, null); }
            catch { }
            _agent.Stop();
        }

        [Fact]
        public async Task Agent_publishes_observation_consumer_receives_same_payload()
        {
            using var subscriber = await ConnectSubscriberAsync().ConfigureAwait(false);

            var matched = new TaskCompletionSource<MqttApplicationMessage>(
                TaskCreationOptions.RunContinuationsAsynchronously);
            subscriber.ApplicationMessageReceivedAsync += args =>
            {
                var msg = args.ApplicationMessage;
                if (!msg.Topic.EndsWith($"/{DeviceUuid}", StringComparison.Ordinal))
                {
                    return Task.CompletedTask;
                }
                if (!msg.Topic.Contains("/Current/", StringComparison.Ordinal))
                {
                    return Task.CompletedTask;
                }
                var bytes = msg.PayloadSegment.Array != null
                    ? msg.PayloadSegment.ToArray()
                    : Array.Empty<byte>();
                if (bytes.Length == 0)
                {
                    return Task.CompletedTask;
                }
                var body = Encoding.UTF8.GetString(bytes);
                if (!body.Contains(InjectedSentinel, StringComparison.Ordinal))
                {
                    return Task.CompletedTask;
                }
                matched.TrySetResult(msg);
                return Task.CompletedTask;
            };

            var topicFilter = $"{TopicPrefix}/#";
            await subscriber.SubscribeAsync(
                new MqttClientSubscribeOptionsBuilder()
                    .WithTopicFilter(topicFilter, MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                    .Build()).ConfigureAwait(false);

            await Task.Delay(500).ConfigureAwait(false);

            InjectObservation(value: InjectedSentinel);

            var completed = await Task.WhenAny(
                matched.Task,
                Task.Delay(TimeSpan.FromSeconds(20))).ConfigureAwait(false);
            Assert.True(
                completed == matched.Task,
                $"Subscriber did not receive a /Current/{DeviceUuid} payload containing '{InjectedSentinel}' within 20s.");

            var msg = await matched.Task.ConfigureAwait(false);
            var payload = Encoding.UTF8.GetString(msg.PayloadSegment.ToArray());

            Assert.Contains($"/Current/{DeviceUuid}", msg.Topic);
            Assert.Contains(DataItemId, payload);
            Assert.Contains(InjectedSentinel, payload);
        }

        [Fact]
        public async Task Consumer_disconnects_mid_publish_agent_does_not_lose_observations()
        {
            // Connect a subscriber, then drop it before the observation
            // is injected. The relay does not buffer for an absent
            // subscriber by default, so the contract under test is the
            // narrower "the agent keeps the observation in its own
            // buffer." Reading GetDeviceStreamsResponseDocument observes
            // the same data the HTTP /current endpoint would return.
            var subscriber = await ConnectSubscriberAsync().ConfigureAwait(false);
            await subscriber.DisconnectAsync().ConfigureAwait(false);
            subscriber.Dispose();

            InjectObservation(value: InjectedSentinel);

            await Task.Delay(250).ConfigureAwait(false);

            var current = _agent.GetDeviceStreamsResponseDocument(DeviceUuid);
            Assert.NotNull(current);

            var observations = current.Streams
                .SelectMany(s => s.ComponentStreams ?? Array.Empty<MTConnect.Streams.Output.IComponentStreamOutput>())
                .SelectMany(c => c.Observations ?? Array.Empty<MTConnect.Observations.Output.IObservationOutput>())
                .ToList();

            var match = observations.FirstOrDefault(o => o.DataItemId == DataItemId);
            Assert.NotNull(match);
            Assert.Equal(InjectedSentinel, match!.GetValue(ValueKeys.Result));
        }

        private async Task<IMqttClient> ConnectSubscriberAsync()
        {
            var factory = new MqttFactory();
            var client = factory.CreateMqttClient();
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer(_broker.Host, _broker.Port)
                .WithClientId($"mtconnect-subscriber-{Guid.NewGuid():N}")
                .WithCleanSession(true)
                .Build();

            await client.ConnectAsync(options, CancellationToken.None).ConfigureAwait(false);
            return client;
        }

        private void InjectObservation(string value)
        {
            var added = _agent.AddObservation(
                DeviceUuid,
                DataItemId,
                ValueKeys.Result,
                value,
                DateTime.UtcNow,
                forceUpdate: true);
            Assert.True(
                added,
                $"Agent rejected the seeded observation for {DataItemId}={value}; " +
                "fixture device + DataItem are mis-shaped.");
        }

        private static Device BuildDevice()
        {
            var device = new Device
            {
                Id = "d1",
                Uuid = DeviceUuid,
                Name = DeviceName,
            };

            var availability = new MTConnect.Devices.DataItems.AvailabilityDataItem
            {
                Id = "avail",
                Category = DataItemCategory.EVENT,
                Type = MTConnect.Devices.DataItems.AvailabilityDataItem.TypeId,
            };
            device.AddDataItem(availability);

            var sample = new MTConnect.Devices.DataItems.PositionDataItem
            {
                Id = DataItemId,
                Category = DataItemCategory.SAMPLE,
                Type = MTConnect.Devices.DataItems.PositionDataItem.TypeId,
                Units = "MILLIMETER",
            };
            device.AddDataItem(sample);

            return device;
        }
    }
}
