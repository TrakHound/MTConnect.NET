using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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

namespace MTConnect.Tests.Integration
{
    public class MTAgentFixture
    {
        // Hands out a distinct OS-assigned free loopback TCP port on each call.
        // Binding a probe socket to port 0 lets the kernel pick a port that is
        // provably free at that instant; the probe is closed immediately so the
        // real server can bind it. Every test instance therefore runs on its
        // own port, so no two tests (in this run or a concurrent/back-to-back
        // run) ever contend for a fixed number — the start-up "Address already
        // in use" race that a hard-coded port causes is removed at the root.
        public static int GetFreePort()
        {
            using var probe = new System.Net.Sockets.Socket(
                System.Net.Sockets.AddressFamily.InterNetwork,
                System.Net.Sockets.SocketType.Stream,
                System.Net.Sockets.ProtocolType.Tcp);

            probe.Bind(new IPEndPoint(IPAddress.Loopback, 0));
            return ((IPEndPoint)probe.LocalEndPoint!).Port;
        }
    }

    public class ClientAgentCommunicationTests : IClassFixture<MTAgentFixture>, IDisposable
    {
        #region Fields

        //private const int c_assertionWaitTimeout = 100000; // Debug
        // Three staggered deadlines, strictly ordered so a marginal-
        // latency case fails on a clean assertion instead of racing
        // teardown:
        //
        //   assertion WaitOne (24000)  <  test cts.CancelAfter (27000)
        //                              <  derived Sample-stream read
        //                                 budget (Heartbeat * 3 = 30000)
        //
        // The stream-read budget is the outermost bound: the client's
        // responseTimer is Heartbeat * 3, reset on every chunk, so an
        // idle Sample stream tears down at ~30000 ms. The test-wide
        // cancellation token fires a few seconds inside that so a stuck
        // request is cancelled before the stream collapses. The
        // assertion wait is a few seconds inside the token so a sample
        // that is merely late (arriving between 24000 and 30000 ms)
        // fails the assertion deterministically rather than racing the
        // token/stream teardown. The pass/fail predicate is unchanged;
        // only the ordering of the surrounding deadlines is widened.
        private const int c_assertionWaitTimeout = 24000;
        private const int c_testCancelTimeout = 27000;

        // Generous, CI-safe bound for waiting until the embedded HTTP server
        // is actually accepting and serving requests. The server's socket bind
        // and the Agent's first-Probe readiness are not instantaneous under a
        // loaded CI runner; polling up to this long (well above any observed
        // bind latency) removes the start-up race without ever masking a
        // genuine hang.
        private const int c_serverReadyTimeout = 30000;
        private const int c_serverReadyPollInterval = 50;

        // MTConnect Agent heartbeat (in milliseconds) requested by the Sample
        // stream client. The client derives its stream read timeout from this
        // value as Heartbeat * 3 (see MTConnectHttpClient: the stream's
        // responseTimer is set to Heartbeat * 3 and is reset on every chunk,
        // data or heartbeat, received). The default Heartbeat of 1000 ms
        // therefore caps an idle Sample stream read at a marginal 3000 ms; on
        // a loaded CI runner the Agent's heartbeat-emitter thread can be
        // scheduler-starved past that, tearing the stream down before the
        // test's sample is delivered. Requesting a generous heartbeat lifts
        // the derived read budget to Heartbeat * 3 = 30000 ms — the same
        // CI-safe order as the readiness bounds above and well beyond any
        // realistic scheduling latency — while the Agent still emits a
        // keep-alive chunk every 10000 ms, far inside that window.
        private const int c_clientHeartbeat = 10000;

        private readonly ShdrAdapter _adapter;

        private readonly IMTConnectAgentBroker _agent;
        private readonly MTConnectHttpServer _server;

        private readonly MTAgentFixture _fixture;
        private readonly ILogger _logger;

        // OS-assigned, per-test free ports (see MTAgentFixture.GetFreePort).
        private readonly int _agentPort;
        private readonly int _adapterPort;

        private readonly string _machineId;
        private readonly string _machineName;

        #endregion

        public ClientAgentCommunicationTests(
            MTAgentFixture fixture,
            ITestOutputHelper testOutputHelper)
        {
            _fixture = fixture;
            _logger = testOutputHelper.BuildLogger(LogLevel.Trace);

            _agentPort = MTAgentFixture.GetFreePort();
            _adapterPort = MTAgentFixture.GetFreePort();

            _machineId = Guid.NewGuid().ToString();
            _machineName = "M12346";

            var devicesFile = "devices.xml";
            GenerateDevicesXml(
                _machineId,
                _machineName,
                devicesFile,
                _logger);

            _adapter = new ShdrIntervalAdapter(_machineName, _adapterPort, 2000, 100);
            _adapter.Start();

            AddCuttingTools();

            _agent = new MTConnectAgentBroker();
            //_agent.Version = new Version(1, 8);
            _agent.Start();

            var adapters = new List<ShdrAdapterClientConfiguration>()
            {
                new()
                {
                    DeviceKey = _machineName,
                    Hostname = "localhost",
                    Port = _adapterPort
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
                Port = _agentPort,
                // Bind to loopback only so an in-process integration run
                // cannot accidentally expose the test agent on a
                // non-loopback interface of the dev machine. Pass the
                // numeric loopback literal (via IPAddress.Loopback) so the
                // server-side bind path never depends on a reverse-PTR
                // entry for 127.0.0.1 in /etc/hosts or the system resolver.
                Server = IPAddress.Loopback.ToString()
            };
            _server = new MTConnectHttpServer(configuration, _agent);
            _server.Start();

            WaitForServerReady(_agentPort, _machineName);
        }

        // Blocks until the embedded HTTP server answers a Probe request for the
        // test device, polling on a short interval up to a generous CI-safe
        // bound. The server's Start() is fire-and-forget and its ServerStarted
        // event fires just before the socket bind completes, so neither is a
        // reliable "ready to serve" point on its own. Polling the actual Probe
        // endpoint proves end to end that the listener is bound and the Agent
        // is serving, which removes the start-up race that otherwise let a
        // client's first request fail and back off for a full reconnection
        // interval (longer than the per-test timeout).
        private void WaitForServerReady(int port, string deviceName)
        {
            var deadline = DateTime.UtcNow.AddMilliseconds(c_serverReadyTimeout);
            Exception? lastError = null;

            while (DateTime.UtcNow < deadline)
            {
                try
                {
                    var probeClient = new MTConnectHttpProbeClient(
                        $"127.0.0.1:{port}",
                        deviceName)
                    {
                        Timeout = c_serverReadyPollInterval * 4
                    };

                    var document = probeClient.Get();
                    if (document != null && !document.Devices.IsNullOrEmpty())
                    {
                        return;
                    }
                }
                catch (Exception ex)
                {
                    lastError = ex;
                }

                Thread.Sleep(c_serverReadyPollInterval);
            }

            throw new XunitException(
                $"Embedded HTTP server did not become ready within {c_serverReadyTimeout} ms"
                + (lastError != null ? $" (last error: {lastError.Message})." : "."));
        }

        public void Dispose()
        {
            _agent.Stop();
            _server.Stop();
            _adapter.Stop();

            // Server.Stop() only signals cancellation; the listener socket is
            // closed asynchronously inside the server's accept loop. Block
            // until the port stops accepting connections so the process does
            // not exit (or the next test does not start) while a half-open
            // listener is still bound.
            WaitForPortReleased(_agentPort);
        }

        // Blocks until a TCP connection to the loopback port is refused (the
        // listener is gone) or a generous CI-safe bound elapses. A refused
        // connection is the definitive "port released" signal; an accepted one
        // means the previous server is still listening.
        private static void WaitForPortReleased(int port)
        {
            var deadline = DateTime.UtcNow.AddMilliseconds(c_serverReadyTimeout);

            while (DateTime.UtcNow < deadline)
            {
                using var socket = new System.Net.Sockets.Socket(
                    System.Net.Sockets.AddressFamily.InterNetwork,
                    System.Net.Sockets.SocketType.Stream,
                    System.Net.Sockets.ProtocolType.Tcp);

                try
                {
                    var connect = socket.BeginConnect(IPAddress.Loopback, port, null, null);
                    var connected = connect.AsyncWaitHandle.WaitOne(
                        c_serverReadyPollInterval * 4);

                    if (!connected)
                    {
                        // No answer within the probe window: treat as released.
                        return;
                    }

                    socket.EndConnect(connect);

                    // Connection accepted: the listener is still up.
                    socket.Close();
                }
                catch (System.Net.Sockets.SocketException)
                {
                    // Connection refused: the port is free.
                    return;
                }

                Thread.Sleep(c_serverReadyPollInterval);
            }
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
                Interval = 500,
                // Lift the derived Sample-stream read timeout (Heartbeat * 3)
                // off its marginal 3000 ms default so a scheduler-starved CI
                // runner cannot tear the stream down before the sample under
                // test is delivered. See c_clientHeartbeat.
                Heartbeat = c_clientHeartbeat
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
            var resourceName = "MTConnect.Tests.Integration.devices-tpl.xml";

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

            using var config = File.Create(fileName);
            xDocument.Save(config);
        }

        #endregion

        [Fact]
        public async Task GetCurrentFieldShouldReturnUpdatedValue()
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(c_testCancelTimeout);

            var currentClient = new MTConnectHttpCurrentClient(
                $"127.0.0.1:{_agentPort}",
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

            Assert.True(observationEvt.WaitOne(c_assertionWaitTimeout));

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
        public async Task WaitForSampleShouldSucceedAfterFirstItemIsSent()
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(c_testCancelTimeout);

            // Completes on the first Current document the client receives.
            // The client's worker runs the Probe + Current requests before it
            // opens the long-lived Sample stream, and pins the Sample stream's
            // starting sequence to the Current response's NextSequence. A data
            // item added before the Current request is served therefore lands
            // in the Current document and is skipped past by that NextSequence,
            // so it never reaches the Sample stream. Waiting for the first
            // CurrentReceived (instead of a fixed sleep) deterministically
            // proves the Current request has been served, so the data item
            // added afterwards is guaranteed a sequence at or above the Sample
            // stream's start and is delivered on the Sample path.
            var currentReady = new TaskCompletionSource<bool>(
                TaskCreationOptions.RunContinuationsAsynchronously);

            void OnCurrent(object? sender, IStreamsResponseDocument document)
            {
                currentReady.TrySetResult(true);
            }

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
                $"127.0.0.1:{_agentPort}",
                _machineName,
                _logger,
                OnCurrent,
                OnSample);
            if (client is null)
            {
                throw new XunitException("Client is null.");
            }

            // Wait until the Current request has provably been served rather
            // than guessing with a fixed delay. Bounded by the test-wide
            // timeout so a client that never reaches the Current request fails
            // fast with a clear message instead of racing.
            using (cts.Token.Register(() => currentReady.TrySetCanceled()))
            {
                try
                {
                    await currentReady.Task;
                }
                catch (OperationCanceledException)
                {
                    throw new XunitException(
                        $"Current request was not served within {c_testCancelTimeout} ms.");
                }
            }

            _adapter.AddDataItem(new ShdrDataItem("servotemp1", 120));

            Assert.True(observationEvt.WaitOne(c_assertionWaitTimeout));
        }
    }
}
