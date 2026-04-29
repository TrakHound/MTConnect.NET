using System;
using System.IO;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Xunit;

namespace IntegrationTests.Workflows
{
    // Spins a Mosquitto broker once per xUnit test class via IClassFixture.
    // The eclipse-mosquitto image is pinned at 2.0.22 so the wire-protocol
    // surface and default config remain reproducible across CI runs and dev
    // machines. The broker listens on a host-side ephemeral port mapped from
    // the container's 1883/tcp; the public Host + Port are the loopback
    // address the test code hands to MTConnectMqttRelay.
    //
    // Source authority for the workflow under test:
    //   - https://docs.mtconnect.org/ Part 6.0 "MTConnect Standard - MQTT
    //     Protocol" — pins the topic structure + payload semantics that
    //     MTConnectMqttRelay implements.
    //   - https://mqtt.org/mqtt-specification/ — the wire protocol that the
    //     Testcontainers Mosquitto + the in-process MQTTnet client speak.
    public sealed class MqttBrokerFixture : IAsyncLifetime
    {
        public const string ImageTag = "eclipse-mosquitto:2.0.22";
        private const int InternalPort = 1883;

        private IContainer? _container;
        private string? _configDir;

        public string Host => _container?.Hostname ?? "127.0.0.1";

        public int Port => _container?.GetMappedPublicPort(InternalPort)
            ?? throw new InvalidOperationException("Container has not been started.");

        public async Task InitializeAsync()
        {
            // Mosquitto 2.x refuses anonymous remote connections by default.
            // Mount a per-fixture config that opens 1883/tcp to anonymous
            // clients so tests do not need to ship credentials into the
            // container or carry a global default the dev's local mosquitto
            // setup might shadow.
            _configDir = Path.Combine(
                Path.GetTempPath(),
                $"mqttrelay-fixture-{Guid.NewGuid():N}");
            Directory.CreateDirectory(_configDir);
            var configFile = Path.Combine(_configDir, "mosquitto.conf");
            File.WriteAllText(
                configFile,
                "listener 1883 0.0.0.0\nallow_anonymous true\n");

            _container = new ContainerBuilder()
                .WithImage(ImageTag)
                .WithPortBinding(InternalPort, assignRandomHostPort: true)
                .WithBindMount(configFile, "/mosquitto/config/mosquitto.conf")
                .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(InternalPort))
                .Build();

            await _container.StartAsync().ConfigureAwait(false);
        }

        public async Task DisposeAsync()
        {
            if (_container != null)
            {
                await _container.DisposeAsync().ConfigureAwait(false);
                _container = null;
            }

            if (_configDir != null && Directory.Exists(_configDir))
            {
                try { Directory.Delete(_configDir, recursive: true); }
                catch (IOException) { }
                catch (UnauthorizedAccessException) { }
                _configDir = null;
            }
        }
    }
}
