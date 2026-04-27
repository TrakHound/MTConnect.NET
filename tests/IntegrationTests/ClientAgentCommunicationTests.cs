using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using MTConnect;
using MTConnect.Adapters;
using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Clients;
using MTConnect.Devices;
using MTConnect.Errors;
using MTConnect.Servers.Http;
using MTConnect.Observations;
using MTConnect.Shdr;
using MTConnect.Streams;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;
using MTConnect.Assets.CuttingTools;

namespace IntegrationTests
{
    public class MTAgentFixture
    {
        #region Fields

        public int CurrentAgentPort = 5000;
        public int CurrentAdapterPort = 7878;

        #endregion
    }

    public class ClientAgentCommunicationTests : IClassFixture<MTAgentFixture>, IDisposable
    {
        #region Fields

        //private const int c_maxWaitTimeout = 100000; // Debug
        private const int c_maxWaitTimeout = 10000;

        private readonly ShdrAdapter _adapter;

        private readonly IMTConnectAgentBroker _agent;
        private readonly MTConnectHttpServer _server;

        private readonly MTAgentFixture _fixture;
        private readonly ILogger _logger;

        private readonly string _machineId;
        private readonly string _machineName;

        #endregion

        public ClientAgentCommunicationTests(
            MTAgentFixture fixture,
            ITestOutputHelper testOutputHelper)
        {
            _fixture = fixture;
            _logger = testOutputHelper.BuildLogger(LogLevel.Trace);

            _machineId = Guid.NewGuid().ToString();
            _machineName = "M12346";
            //_machineName = $"Machine{_fixture.CurrentAgentPort}";

            var devicesFile = "devices.xml";
            GenerateDevicesXml(
                _machineId,
                _machineName,
                devicesFile,
                _logger);

            _adapter = new ShdrIntervalAdapter(_machineName, _fixture.CurrentAdapterPort, 2000, 100);
            _adapter.Start();

            AddCuttingTools();

            // Pin the broker to a version for which libraries/MTConnect.NET-XML
            // has full Namespaces.cs + Schemas.cs mappings. The parameterless
            // ctor uses MTConnectVersions.Max as the default, which can advance
            // ahead of the XML library's namespace/schema coverage and surface
            // as HTTP 500 from the wire-format formatter (Namespaces.GetDevices
            // returns null for unmapped versions).
            var agentConfiguration = new AgentConfiguration
            {
                DefaultVersion = MTConnectVersions.Version25,
            };
            _agent = new MTConnectAgentBroker(agentConfiguration);
            _agent.Start();

            var adapters = new List<ShdrAdapterClientConfiguration>()
            {
                new()
                {
                    DeviceKey = _machineName,
                    Hostname = "localhost",
                    Port = _fixture.CurrentAdapterPort
                }
            };

            // Add Adapter Clients
            var devices = DeviceConfiguration.FromFile(devicesFile, DocumentFormat.XML).ToList();
            if (!devices.IsNullOrEmpty())
            {
                // Add Device(s) to Agent
                foreach (var device in devices)
                {
                    _agent.AddDevice(device);
                }

                foreach (var adapterConfiguration in adapters)
                {
                    var device = devices.FirstOrDefault(o => o.Name == adapterConfiguration.DeviceKey);
                    if (device != null)
                    {
                        var adapterClient = new ShdrAdapterClient(adapterConfiguration, _agent, device);

                        adapterClient.Start();
                    }
                }
            }

            var configuration = new HttpServerConfiguration
            {
                Port = _fixture.CurrentAgentPort
            };
            _server = new MTConnectHttpServer(configuration, _agent);

            // Capture any startup exception (e.g. EADDRINUSE) so the
            // WaitForListener timeout produces a useful diagnostic instead
            // of silently waiting out the deadline.
            Exception? serverStartException = null;
            _server.ServerException += (_, ex) =>
            {
                serverStartException ??= ex;
            };

            _server.Start();

            // MTConnectHttpServer.Start() is fire-and-forget: it spawns a
            // background Task.Run that performs the TCP bind + listen loop.
            // The first test request can race ahead of that bind and surface
            // as "Connection refused". Block here until the listener accepts
            // a TCP connection, with a generous timeout for slow CI hosts
            // and threadpool-starved parallel test runs.
            WaitForListener(
                "127.0.0.1",
                _fixture.CurrentAgentPort,
                TimeSpan.FromSeconds(30),
                () => serverStartException);
        }

        private static void WaitForListener(
            string host,
            int port,
            TimeSpan timeout,
            Func<Exception?>? serverStartException = null)
        {
            var deadline = DateTime.UtcNow + timeout;
            while (DateTime.UtcNow < deadline)
            {
                var startupException = serverStartException?.Invoke();
                if (startupException != null)
                {
                    throw new InvalidOperationException(
                        $"HTTP server failed to start on {host}:{port}: {startupException.Message}",
                        startupException);
                }

                try
                {
                    using var client = new System.Net.Sockets.TcpClient();
                    client.Connect(host, port);
                    if (client.Connected)
                    {
                        return;
                    }
                }
                catch (System.Net.Sockets.SocketException)
                {
                    // not listening yet; keep polling
                }

                Thread.Sleep(100);
            }

            throw new TimeoutException(
                $"HTTP listener did not bind to {host}:{port} within {timeout.TotalSeconds}s.");
        }

        public void Dispose()
        {
            // Stop are not awaitable, so we cannot guarantee that it finishes before next test start
            _agent.Stop();
            _server.Stop();
            _adapter.Stop();

            // Therefore we use a new port for every test.
            _fixture.CurrentAgentPort++;
            _fixture.CurrentAdapterPort++;

        }

        #region Private Tests

        private void AddCuttingTools()
        {
            var tool = new MTConnect.Assets.CuttingTools.CuttingToolAsset
            {
                //tool.Description = new Devices.Description
                //{
                //    Manufacturer = "Sandvik",
                //    Model = "B5632",
                //    SerialNumber = "12345678946"
                //};
                AssetId = "5.12",
                ToolId = "12",
                CuttingToolLifeCycle = new MTConnect.Assets.CuttingTools.CuttingToolLifeCycle
                {
                    Location = new MTConnect.Assets.CuttingTools.Location { Type = MTConnect.Assets.CuttingTools.LocationType.SPINDLE },
                    ProgramToolNumber = "12",
                    ProgramToolGroup = "5"
                }
            };

            var cuttingToolLifeCycle = new CuttingToolLifeCycle();

            var measurements = new List<IMeasurement>();
            measurements.Add(new MTConnect.Assets.CuttingTools.Measurements.FunctionalLengthMeasurement(7.6543));
            measurements.Add(new MTConnect.Assets.CuttingTools.Measurements.CuttingDiameterMaxMeasurement(0.375));
            cuttingToolLifeCycle.Measurements = measurements.OfType<IToolingMeasurement>();

            var cuttingItems = new List<ICuttingItem>();
            cuttingItems.Add(new CuttingItem
            {
                ItemId = "12.1",
                //Locus = MTConnect.Assets.CuttingTools..FLUTE.ToString()
            });
            cuttingToolLifeCycle.CuttingItems = cuttingItems;

            var cutterStatus = new List<CutterStatusType>();
            cutterStatus.Add(CutterStatusType.AVAILABLE);
            cutterStatus.Add(CutterStatusType.NEW);
            cutterStatus.Add(CutterStatusType.MEASURED);
            cuttingToolLifeCycle.CutterStatus = cutterStatus;

            tool.CuttingToolLifeCycle = cuttingToolLifeCycle;
            tool.Timestamp = DateTime.Now;

            _adapter.SendAsset(tool);
        }

        private Task<MTConnectHttpClient?> Connect(
            string url, 
            string deviceName,
            ILogger logger,
            EventHandler<IStreamsResponseDocument> onCurrent,
            EventHandler<IStreamsResponseDocument> onSample)
        {
            var tcs = new TaskCompletionSource<MTConnectHttpClient?>();

            var client = new MTConnectHttpClient(url, deviceName)
            {
                Interval = 500
            };
            client.CurrentReceived += onCurrent;
            client.SampleReceived += onSample;
            client.ConnectionError += (
                sender,
                exception) =>
            {
                _logger.LogDebug(exception, "Connection error happened.");
                tcs.TrySetResult(null);
            };
            client.InternalError += (
                sender,
                exception) =>
            {
                _logger.LogDebug(exception, "Internal error happened.");
                tcs.TrySetResult(null);
            };
            client.MTConnectError += (
                sender,
                exception) =>
            {
                foreach (var ex in exception.Errors)
                {
                    _logger.LogDebug(
                        "MTConnect error {0} happened: {1}.",
                        ex.ErrorCode,
                        ex.Value);
                }

                tcs.TrySetResult(null);
            };
            client.ClientStarted += (
                sender,
                args) =>
            {
                _logger.LogTrace("Connection established");
                tcs.TrySetResult(client);
            };
            client.Start();

            return tcs.Task;
        }

        internal static void GenerateDevicesXml(
            string machineId,
            string machineName,
            string fileName,
            ILogger logger)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "IntegrationTests.devices-tpl.xml";

            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream is null)
            {
                logger.LogError("Cannot find resource {0}", resourceName);
                return;
            }

            using var reader = new StreamReader(stream);
            var xml = reader.ReadToEnd();
            var xDocument = XDocument.Parse(xml);

            XNamespace ns = "urn:mtconnect.org:MTConnectDevices:1.7";
            var xDevice = xDocument.Descendants(ns + "Device").First();
            var uuidAttr = xDevice.Attribute("uuid");
            if (uuidAttr is null)
            {
                logger.LogError("UUID Device attribute cannot be found in\n{0}", xml);
                return;
            }

            uuidAttr.Value = machineId;

            var nameAttr = xDevice.Attribute("name");
            if (nameAttr is null)
            {
                logger.LogError("UUID Device attribute cannot be found in\n{0}", xml);
                return;
            }

            nameAttr.Value = machineName;

            using var config = File.Create("devices.xml");
            xDocument.Save(config);
        }

        #endregion

        [Fact]
        public async void GetCurrentFieldShouldReturnUpdatedValue()
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(c_maxWaitTimeout);

            var currentClient = new MTConnectHttpCurrentClient(
                $"127.0.0.1:{_fixture.CurrentAgentPort}", 
                _machineName, 
                $"//*[@id='program']");

            var document = await currentClient.GetAsync(cts.Token);
            if (document is null || document.Streams.IsNullOrEmpty())
            {
                throw new XunitException("Document is null or empty.");
            }

            var current = document.Streams
                .First()
                .Observations
                .Select(o => o.GetValue("Result"))
                .FirstOrDefault();

            Assert.Equal("UNAVAILABLE", current);

            var observationEvt = new AutoResetEvent(false);
            _agent.ObservationAdded += (
                sender,
                observation) =>
            {
                _logger.LogTrace($"to {observation.DataItemId} {observation.GetValue("Result")}");
                observationEvt.Set();
            };

            var item = new ShdrDataItem("program", "SuperProg42");
            _adapter.AddDataItem(item);

            Assert.True(observationEvt.WaitOne(c_maxWaitTimeout));

            document = await currentClient.GetAsync(cts.Token);
            if (document is null || document.Streams.IsNullOrEmpty())
            {
                throw new XunitException("Document is null or empty.");
            }

            current = document.Streams
                .First()
                .Observations
                .Select(o => o.GetValue("Result"))
                .FirstOrDefault();

            Assert.Equal("SuperProg42", current);
        }

        [Fact]
        public async void WaitForSampleShouldSucceedAfterFirstItemIsSent()
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(c_maxWaitTimeout);

            void OnCurrent(object? sender, IStreamsResponseDocument document) { }

            var observationEvt = new AutoResetEvent(false);
            void OnSample(
                object? sender,
                IStreamsResponseDocument document)
            {
                if (document.Streams.IsNullOrEmpty())
                {
                    return;
                }

                foreach (var observation in document.GetObservations())
                {
                    if (observation.DataItemId == "servotemp1" && observation.GetValue("Result") == "120")
                    {
                        observationEvt.Set();
                    }
                }
            }

            var client = await Connect(
                $"127.0.0.1:{_fixture.CurrentAgentPort}",
                _machineName,
                _logger,
                OnCurrent,
                OnSample);
            if (client is null)
            {
                throw new XunitException("Client is null.");
            }

            // Delay 1 second to let client connect
            // Otherwise the added dataitem will be contained in the Current response instead of the Sample response
            await Task.Delay(1000);

            _adapter.AddDataItem(new ShdrDataItem("servotemp1", 120));
            //_adapter.AddDataItem(new ShdrDataItem("servotemp1", new TemperatureValue(120)));
            
            Assert.True(observationEvt.WaitOne(c_maxWaitTimeout));
        }
    }
}
