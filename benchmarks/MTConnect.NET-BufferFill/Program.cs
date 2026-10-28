// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MTConnect.BufferFill
{
    /// <summary>
    /// Fills a running agent's observation buffer so its memory use can be viewed with a full buffer. Acts as the SHDR
    /// adapter the agent connects to (the agent's shdr-adapter module, port 7878 by default) and sends a changing value
    /// for every SAMPLE DataItem of the device file until the requested number of observations has been sent, then
    /// keeps the connection open (answering heartbeats) until stopped.
    ///
    /// Usage (start the agent first, or after: the agent reconnects):
    ///   dotnet run -c Release --project benchmarks/MTConnect.NET-BufferFill -- [options]
    ///     --device &lt;path&gt;   Device file to read SAMPLE DataItem IDs from (default agent/MTConnect.NET-Agent/devices/okuma.xml)
    ///     --port &lt;n&gt;       SHDR port the agent connects to (default 7878)
    ///     --count &lt;n&gt;      Observations to send (default 150000, the default buffer size)
    ///     --rate &lt;n&gt;       Observations per second (default 20000; 0 = as fast as possible)
    ///     --agent &lt;url&gt;    Agent HTTP address used to report the buffer state (default http://localhost:5000)
    ///     --exit           Exit once the observations are sent instead of holding the connection
    /// </summary>
    public static class Program
    {
        public static int Main(string[] args)
        {
            var options = ParseArgs(args);
            var devicePath = options.TryGetValue("device", out var d) ? d : FindDefaultDevice();
            var port = options.TryGetValue("port", out var p) ? int.Parse(p) : 7878;
            var count = options.TryGetValue("count", out var c) ? long.Parse(c) : 150000;
            var rate = options.TryGetValue("rate", out var r) ? int.Parse(r) : 20000;
            var agentUrl = options.TryGetValue("agent", out var a) ? a.TrimEnd('/') : "http://localhost:5000";
            var exitWhenDone = options.ContainsKey("exit");

            if (devicePath == null || !File.Exists(devicePath))
            {
                Console.Error.WriteLine("Device file not found. Pass --device <path>.");
                return 1;
            }

            var dataItemIds = XDocument.Load(devicePath).Descendants()
                .Where(e => e.Name.LocalName == "DataItem" && (string)e.Attribute("category") == "SAMPLE")
                .Select(e => (string)e.Attribute("id"))
                .Where(id => !string.IsNullOrEmpty(id))
                .ToArray();

            if (dataItemIds.Length == 0)
            {
                Console.Error.WriteLine($"No SAMPLE DataItems in {devicePath}.");
                return 1;
            }

            Console.WriteLine($"Device {Path.GetFileName(devicePath)}: {dataItemIds.Length} SAMPLE DataItems");
            Console.WriteLine($"Listening for the agent on port {port}; sending {count:N0} observations at {(rate > 0 ? rate.ToString("N0") + "/s" : "full speed")}");

            var listener = new TcpListener(IPAddress.Any, port);
            listener.Start();

            using var stop = new CancellationTokenSource();
            Console.CancelKeyPress += (s, e) => { e.Cancel = true; stop.Cancel(); };

            long sent = 0;
            var reported = false;

            while (!stop.IsCancellationRequested)
            {
                TcpClient client;
                try { client = listener.AcceptTcpClientAsync(stop.Token).AsTask().GetAwaiter().GetResult(); }
                catch (OperationCanceledException) { break; }

                Console.WriteLine($"Agent connected from {client.Client.RemoteEndPoint}");
                using (client)
                {
                    try
                    {
                        var stream = client.GetStream();
                        var writeLock = new object();
                        _ = Task.Run(() => AnswerHeartbeats(stream, writeLock, stop.Token));

                        sent = Send(stream, writeLock, dataItemIds, sent, count, rate, stop.Token);

                        if (sent >= count && !reported)
                        {
                            reported = true;
                            Thread.Sleep(1000); // Let the agent process the last lines
                            ReportBuffer(agentUrl);

                            if (exitWhenDone) return 0;
                            Console.WriteLine("Buffer filled. Holding the connection open (answering heartbeats); press Ctrl+C to stop.");
                        }

                        // Hold the connection until the agent disconnects or the tool is stopped
                        while (!stop.IsCancellationRequested && client.Connected && !(client.Client.Poll(0, SelectMode.SelectRead) && client.Client.Available == 0))
                        {
                            Thread.Sleep(500);
                        }
                    }
                    catch (IOException) { }
                    catch (SocketException) { }
                }

                if (!stop.IsCancellationRequested) Console.WriteLine($"Agent disconnected ({sent:N0} of {count:N0} sent); waiting for it to reconnect");
            }

            listener.Stop();
            return 0;
        }

        // Sends lines of "timestamp|id|value|id|value..." (one value per DataItem per line) until 'count' have been sent
        private static long Send(NetworkStream stream, object writeLock, string[] dataItemIds, long sent, long count, int rate, CancellationToken stop)
        {
            var sw = Stopwatch.StartNew();
            var startSent = sent;
            var lastReport = 0L;
            var line = new StringBuilder();

            while (sent < count && !stop.IsCancellationRequested)
            {
                // Pace to the requested rate
                if (rate > 0)
                {
                    var due = startSent + (long)(sw.Elapsed.TotalSeconds * rate);
                    if (sent >= due)
                    {
                        Thread.Sleep(1);
                        continue;
                    }
                }

                line.Clear();
                line.Append(DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture));
                var perLine = (int)Math.Min(dataItemIds.Length, count - sent);
                for (var i = 0; i < perLine; i++)
                {
                    // Every value differs from the previous one for the same DataItem, so each is stored
                    var value = ((sent + i) / dataItemIds.Length) % 100000 * 0.001 + i;
                    line.Append('|').Append(dataItemIds[i]).Append('|').Append(value.ToString("F3", CultureInfo.InvariantCulture));
                }
                line.Append('\n');

                var bytes = Encoding.ASCII.GetBytes(line.ToString());
                lock (writeLock) stream.Write(bytes, 0, bytes.Length);
                sent += perLine;

                if (sent - lastReport >= 25000 || sent >= count)
                {
                    lastReport = sent;
                    Console.WriteLine($"  {sent:N0} sent ({sw.Elapsed.TotalSeconds:F1} s)");
                }
            }

            return sent;
        }

        // The agent sends "* PING"; answer "* PONG <heartbeat ms>" so it keeps the connection open
        private static void AnswerHeartbeats(NetworkStream stream, object writeLock, CancellationToken stop)
        {
            try
            {
                using var reader = new StreamReader(stream, Encoding.ASCII, false, 1024, leaveOpen: true);
                var pong = Encoding.ASCII.GetBytes("* PONG 10000\n");
                string line;
                while (!stop.IsCancellationRequested && (line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith("* PING", StringComparison.Ordinal))
                    {
                        lock (writeLock) stream.Write(pong, 0, pong.Length);
                    }
                }
            }
            catch { }
        }

        private static void ReportBuffer(string agentUrl)
        {
            try
            {
                using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
                var xml = http.GetStringAsync(agentUrl + "/current").GetAwaiter().GetResult();
                string Attribute(string name) => Regex.Match(xml, name + "=\"(\\d+)\"").Groups[1].Value;

                var bufferSize = Attribute("bufferSize");
                var first = Attribute("firstSequence");
                var last = Attribute("lastSequence");
                Console.WriteLine($"Agent buffer: bufferSize={bufferSize} firstSequence={first} lastSequence={last}" +
                    (first != "1" ? " (full: the oldest observations are being overwritten)" : ""));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not read the buffer state from {agentUrl}/current: {ex.Message}");
            }
        }

        private static string FindDefaultDevice()
        {
            // Walk up from the working directory to the repository's agent device file
            var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
            while (dir != null)
            {
                var candidate = Path.Combine(dir.FullName, "agent", "MTConnect.NET-Agent", "devices", "okuma.xml");
                if (File.Exists(candidate)) return candidate;
                dir = dir.Parent;
            }
            return null;
        }

        private static Dictionary<string, string> ParseArgs(string[] args)
        {
            var options = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            for (var i = 0; i < args.Length; i++)
            {
                if (!args[i].StartsWith("--")) continue;
                var key = args[i].Substring(2);
                if (i + 1 < args.Length && !args[i + 1].StartsWith("--")) options[key] = args[++i];
                else options[key] = "true";
            }
            return options;
        }
    }
}
