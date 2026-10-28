// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MTConnect.Adapters;
using MTConnect.Assets;
using MTConnect.Assets.Files;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Devices.DataItems;
using MTConnect.Shdr;
using NUnit.Framework;

namespace MTConnect.Tests.Shdr
{
    /// <summary>
    /// SHDR over TCP. Client: lines split across reads, multiline assets split across reads, UTF-8 text, an unknown
    /// first key, and a silent adapter being disconnected. Adapter: values set before an Agent connects, the
    /// connect-time dump going only to the new Agent, queued values and assets, PING / PONG, and configuration.
    /// </summary>
    [TestFixture]
    [Category("ShdrNetwork")]
    [NonParallelizable]
    public class ShdrNetworkTests
    {
        private static readonly TimeSpan Wait = TimeSpan.FromSeconds(8);


        private static int GetFreePort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        private static bool WaitFor(Func<bool> condition)
        {
            var deadline = DateTime.UtcNow + Wait;
            while (DateTime.UtcNow < deadline)
            {
                if (condition()) return true;
                Thread.Sleep(25);
            }
            return condition();
        }


        #region "Client"

        private sealed class RecordingClient : ShdrClient
        {
            public readonly ConcurrentQueue<ShdrDataItem> DataItems = new ConcurrentQueue<ShdrDataItem>();
            public readonly ConcurrentQueue<ShdrMessage> Messages = new ConcurrentQueue<ShdrMessage>();
            public readonly ConcurrentQueue<IAsset> Assets = new ConcurrentQueue<IAsset>();
            public int Disconnects;

            public RecordingClient(int port) : base("127.0.0.1", port, 5000, 200)
            {
                Disconnected += (s, e) => Interlocked.Increment(ref Disconnects);
            }

            protected override IDataItem OnGetDataItem(string dataItemKey)
            {
                if (dataItemKey == "msg") return new MessageDataItem { Id = "msg" };
                if (dataItemKey == "unknown") return null;
                return new ProgramDataItem { Id = dataItemKey };
            }

            protected override void OnDataItemReceived(ShdrDataItem dataItem) => DataItems.Enqueue(dataItem);
            protected override void OnMessageReceived(ShdrMessage message) => Messages.Enqueue(message);
            protected override void OnAssetReceived(IAsset asset) => Assets.Enqueue(asset);
        }

        // A fake adapter: accepts one Agent connection, answers its first PING, then runs the script
        private static Task RunFakeAdapter(TcpListener listener, Func<NetworkStream, Task> script, int pongHeartbeat = 10000)
        {
            return Task.Run(async () =>
            {
                using (var client = await listener.AcceptTcpClientAsync())
                using (var stream = client.GetStream())
                {
                    var buffer = new byte[256];
                    await stream.ReadAsync(buffer, 0, buffer.Length); // "* PING"
                    await Write(stream, $"* PONG {pongHeartbeat}\n");
                    await script(stream);
                }
            });
        }

        private static async Task Write(NetworkStream stream, string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            await stream.WriteAsync(bytes, 0, bytes.Length);
            await stream.FlushAsync();
        }

        private static (TcpListener Listener, RecordingClient Client) StartClient()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var client = new RecordingClient(((IPEndPoint)listener.LocalEndpoint).Port);
            return (listener, client);
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: client line split across reads is reassembled.
        /// </summary>
        [Test]
        public void Client_LineSplitAcrossReads_IsReassembled()
        {
            var (listener, client) = StartClient();
            try
            {
                var adapter = RunFakeAdapter(listener, async stream =>
                {
                    await Write(stream, "2024-01-01T00:00:00Z|Xact|12.3");
                    await Task.Delay(300);
                    await Write(stream, "45|Yact|1\n");
                    await Task.Delay(2000);
                });

                client.Start();

                Assert.That(WaitFor(() => client.DataItems.Any(o => o.DataItemKey == "Yact")), Is.True);
                Assert.That(client.DataItems.Single(o => o.DataItemKey == "Xact").CDATA, Is.EqualTo("12.345"));
                Assert.That(client.DataItems.Any(o => o.DataItemKey == "45"), Is.False);
            }
            finally
            {
                client.Stop();
                listener.Stop();
            }
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: client multiline asset across reads is reassembled.
        /// </summary>
        [Test]
        public void Client_MultilineAssetAcrossReads_IsReassembled()
        {
            var (listener, client) = StartClient();
            try
            {
                var adapter = RunFakeAdapter(listener, async stream =>
                {
                    await Write(stream, "|@ASSET@|A1|File|--multiline--ABCD\n<File assetId=\"A1\" name=\"f\" size=\"1\"");
                    await Task.Delay(300);
                    await Write(stream, "\nversionId=\"1\">\n  <CreationTime>2024-01-01T00:00:00Z</CreationTime>\n</File>\n--multiline--ABCD\n");
                    await Task.Delay(2000);
                });

                client.Start();

                Assert.That(WaitFor(() => client.Assets.Count > 0), Is.True);
                Assert.That(client.Assets.Single().AssetId, Is.EqualTo("A1"));
                Assert.That(client.DataItems, Is.Empty, "asset XML lines must not be parsed as DataItems");
            }
            finally
            {
                client.Stop();
                listener.Stop();
            }
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: client utf8 text is decoded.
        /// </summary>
        [Test]
        public void Client_Utf8Text_IsDecoded()
        {
            var (listener, client) = StartClient();
            try
            {
                var adapter = RunFakeAdapter(listener, async stream =>
                {
                    await Write(stream, "2024-01-01T00:00:00Z|msg|C1|Température élevée\r\n");
                    await Task.Delay(2000);
                });

                client.Start();

                Assert.That(WaitFor(() => client.Messages.Count > 0), Is.True);
                Assert.That(client.Messages.Single().CDATA, Is.EqualTo("Température élevée"), "and no trailing carriage return");
            }
            finally
            {
                client.Stop();
                listener.Stop();
            }
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: client unknown first key does not drop the rest of the line.
        /// </summary>
        [Test]
        public void Client_UnknownFirstKey_DoesNotDropTheRestOfTheLine()
        {
            var (listener, client) = StartClient();
            try
            {
                var adapter = RunFakeAdapter(listener, async stream =>
                {
                    await Write(stream, "2024-01-01T00:00:00Z|unknown|1|Xact|5\n");
                    await Task.Delay(2000);
                });

                client.Start();

                Assert.That(WaitFor(() => client.DataItems.Any(o => o.DataItemKey == "Xact")), Is.True);
                Assert.That(client.DataItems.Single(o => o.DataItemKey == "Xact").CDATA, Is.EqualTo("5"));
            }
            finally
            {
                client.Stop();
                listener.Stop();
            }
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: client silent adapter is disconnected.
        /// </summary>
        [Test]
        public void Client_SilentAdapter_IsDisconnected()
        {
            var (listener, client) = StartClient();
            try
            {
                // Advertise a 500 ms heartbeat, then never send anything again (and never close the socket)
                var adapter = RunFakeAdapter(listener, stream => Task.Delay(TimeSpan.FromSeconds(30)), pongHeartbeat: 500);

                client.Start();

                Assert.That(WaitFor(() => Volatile.Read(ref client.Disconnects) > 0), Is.True);
            }
            finally
            {
                client.Stop();
                listener.Stop();
            }
        }

        #endregion

        #region "Adapter"

        // An Agent: connects to the adapter and records every line it receives
        private sealed class FakeAgent : IDisposable
        {
            private readonly TcpClient _client;
            private readonly NetworkStream _stream;
            public readonly ConcurrentQueue<string> Lines = new ConcurrentQueue<string>();

            public FakeAgent(int port)
            {
                _client = new TcpClient();
                _client.Connect(IPAddress.Loopback, port);
                _stream = _client.GetStream();

                _ = Task.Run(() =>
                {
                    try
                    {
                        using (var reader = new StreamReader(_stream, Encoding.UTF8))
                        {
                            string line;
                            while ((line = reader.ReadLine()) != null) Lines.Enqueue(line);
                        }
                    }
                    catch { }
                });
            }

            public void Send(string text)
            {
                var bytes = Encoding.ASCII.GetBytes(text);
                _stream.Write(bytes, 0, bytes.Length);
            }

            public int Count(string contains) => Lines.Count(o => o.Contains(contains));

            public void Dispose() => _client.Dispose();
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: adapter value set before agent connects is sent on connect.
        /// </summary>
        [Test]
        public void Adapter_ValueSetBeforeAgentConnects_IsSentOnConnect()
        {
            var port = GetFreePort();
            var adapter = new ShdrIntervalAdapter(port, 10000, 50);
            adapter.Start();
            try
            {
                adapter.AddDataItem("avail", "AVAILABLE");
                Thread.Sleep(300); // Several interval ticks with no Agent connected

                using (var agent = new FakeAgent(port))
                {
                    Assert.That(WaitFor(() => agent.Count("|avail|AVAILABLE") > 0), Is.True);
                }
            }
            finally
            {
                adapter.Stop();
            }
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: adapter connect dump goes only to the new agent.
        /// </summary>
        [Test]
        public void Adapter_ConnectDump_GoesOnlyToTheNewAgent()
        {
            var port = GetFreePort();
            var adapter = new ShdrIntervalAdapter(port, 10000, 50);
            adapter.Start();
            try
            {
                adapter.AddDataItem("avail", "AVAILABLE");
                Thread.Sleep(200);

                using (var first = new FakeAgent(port))
                {
                    Assert.That(WaitFor(() => first.Count("|avail|") > 0), Is.True);

                    using (var second = new FakeAgent(port))
                    {
                        Assert.That(WaitFor(() => second.Count("|avail|") > 0), Is.True);
                        Thread.Sleep(300);

                        Assert.That(first.Count("|avail|"), Is.EqualTo(1), "an existing Agent must not receive the next Agent's dump");
                    }
                }
            }
            finally
            {
                adapter.Stop();
            }
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: queue adapter sends every queued value in order and assets too.
        /// </summary>
        [Test]
        public void QueueAdapter_SendsEveryQueuedValueInOrder_AndAssetsToo()
        {
            var port = GetFreePort();
            var adapter = new ShdrIntervalQueueAdapter(port, 10000, 100);
            adapter.Start();
            try
            {
                using (var agent = new FakeAgent(port))
                {
                    Thread.Sleep(300); // Connected before anything is queued

                    var now = UnixDateTime.Now;
                    adapter.AddDataItem("count", "1", now);
                    adapter.AddDataItem("count", "2", now + 10000);
                    adapter.AddDataItem("count", "3", now + 20000);
                    adapter.AddAsset(new FileAsset { AssetId = "A1", Size = 1, VersionId = "1" });

                    Assert.That(WaitFor(() => agent.Count("|count|") >= 3 && agent.Count("@ASSET@") > 0), Is.True,
                        string.Join(" / ", agent.Lines));

                    var values = agent.Lines.Where(o => o.Contains("|count|")).Select(o => o.Substring(o.LastIndexOf('|') + 1)).ToList();
                    Assert.That(values, Is.EqualTo(new[] { "1", "2", "3" }));
                }
            }
            finally
            {
                adapter.Stop();
            }
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: adapter answers ping.
        /// </summary>
        [Test]
        public void Adapter_AnswersPing()
        {
            var port = GetFreePort();
            var adapter = new ShdrAdapter(port, 5000);
            adapter.Start();
            try
            {
                using (var agent = new FakeAgent(port))
                {
                    agent.Send("* PING\n");

                    Assert.That(WaitFor(() => agent.Count("* PONG 5000") > 0), Is.True);
                }
            }
            finally
            {
                adapter.Stop();
            }
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: adapter settings pass through to the adapter.
        /// </summary>
        [Test]
        public void Adapter_Settings_PassThroughToTheAdapter()
        {
            var adapter = new ShdrAdapter(GetFreePort());

            adapter.FilterDuplicates = false;
            adapter.OutputTimestamps = false;

            Assert.That(adapter.FilterDuplicates, Is.False);
            Assert.That(adapter.OutputTimestamps, Is.False);
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: interval adapter from null configuration starts and stops.
        /// </summary>
        [Test]
        public void IntervalAdapter_FromNullConfiguration_StartsAndStops()
        {
            var adapter = new ShdrIntervalAdapter((ShdrAdapterClientConfiguration)null);

            Assert.DoesNotThrow(() =>
            {
                adapter.Start();
                adapter.Stop();
            });
        }

        #endregion
    }
}
