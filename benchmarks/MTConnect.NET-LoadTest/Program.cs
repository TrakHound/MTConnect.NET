// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Servers.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.LoadTest
{
    /// <summary>
    /// HTTP load test: an agent served by MTConnectHttpServer runs in a child process (so its CPU, threads and memory
    /// are measured on their own) while this process opens many /sample streams and /current pollers against it.
    /// Observation values carry the time they were added, so each streamed part gives an add-to-receive latency.
    /// Uses only public APIs so the same file can be run against different versions of the libraries.
    /// Usage: dotnet run -c Release --project benchmarks/MTConnect.NET-LoadTest -- run [streams] [pollers] [observations/s] [seconds] [interval ms]
    /// </summary>
    public static class Program
    {
        private const string DeviceUuid = "load-device";
        private const int DataItems = 100;

        public static int Main(string[] args)
        {
            if (args.Length > 0 && args[0] == "server")
            {
                Server(int.Parse(args[1]), int.Parse(args[2]));
                return 0;
            }

            int Arg(int index, int defaultValue) => args.Length > index ? int.Parse(args[index]) : defaultValue;
            return Run(Arg(1, 100), Arg(2, 0), Arg(3, 1000), Arg(4, 20), Arg(5, 100)).GetAwaiter().GetResult();
        }


        #region "Server (child process)"

        private static void Server(int port, int rate)
        {
            var agent = new MTConnectAgentBroker(new AgentConfiguration { ObservationBufferSize = 131072 });
            agent.Start();

            var device = new Device { Id = "load", Name = "load", Uuid = DeviceUuid };
            for (var i = 0; i < DataItems; i++) device.AddDataItem(new DataItem { Id = "e" + i, Name = "e" + i, Category = DataItemCategory.EVENT, Type = "MESSAGE", Representation = DataItemRepresentation.VALUE });
            agent.AddDevice(device);
            for (var i = 0; i < DataItems; i++) agent.AddObservation(DeviceUuid, "e" + i, (object)("T" + DateTime.UtcNow.Ticks), DateTime.UtcNow);

            var server = new MTConnectHttpServer(new HttpServerConfiguration { Port = port, Server = IPAddress.Loopback.ToString() }, agent);
            server.Start();

            var stop = false;

            // Ingest at a steady rate; every value is new and carries the time it was added
            if (rate > 0)
            {
                new Thread(() =>
                {
                    var sw = Stopwatch.StartNew();
                    long added = 0;
                    while (!Volatile.Read(ref stop))
                    {
                        var due = (long)(sw.Elapsed.TotalSeconds * rate);
                        while (added < due)
                        {
                            var now = DateTime.UtcNow;
                            agent.AddObservation(DeviceUuid, "e" + (added % DataItems), (object)("T" + now.Ticks), now);
                            added++;
                        }
                        Thread.Sleep(5);
                    }
                }) { IsBackground = true, Name = "Ingest" }.Start();
            }

            // Sample thread counts while measuring
            var maxThreads = 0;
            var maxPoolThreads = 0;
            new Thread(() =>
            {
                using var process = Process.GetCurrentProcess();
                while (!Volatile.Read(ref stop))
                {
                    process.Refresh();
                    maxThreads = Math.Max(maxThreads, process.Threads.Count);
                    maxPoolThreads = Math.Max(maxPoolThreads, ThreadPool.ThreadCount);
                    Thread.Sleep(250);
                }
            }) { IsBackground = true }.Start();

            Console.WriteLine("ready");

            TimeSpan cpuStart = TimeSpan.Zero, pauseStart = TimeSpan.Zero;
            int gen2Start = 0;
            long allocatedStart = 0;
            var window = new Stopwatch();
            string line;
            while ((line = Console.ReadLine()) != null)
            {
                using var process = Process.GetCurrentProcess();
                if (line == "start")
                {
                    cpuStart = process.TotalProcessorTime;
                    pauseStart = GC.GetTotalPauseDuration();
                    gen2Start = GC.CollectionCount(2);
                    allocatedStart = GC.GetTotalAllocatedBytes();
                    maxThreads = 0;
                    maxPoolThreads = 0;
                    window.Restart();
                }
                else if (line == "stop")
                {
                    window.Stop();
                    var seconds = window.Elapsed.TotalSeconds;
                    var cpu = (process.TotalProcessorTime - cpuStart).TotalSeconds;
                    Console.WriteLine(
                        $"stats cpu={100 * cpu / seconds:F0}% " +
                        $"threads={maxThreads} pool={maxPoolThreads} " +
                        $"alloc={(GC.GetTotalAllocatedBytes() - allocatedStart) / 1048576.0 / seconds:F0}MB/s " +
                        $"gcpause={100 * (GC.GetTotalPauseDuration() - pauseStart).TotalSeconds / seconds:F1}% gen2={GC.CollectionCount(2) - gen2Start} " +
                        $"peakws={process.PeakWorkingSet64 / 1048576}MB heap={GC.GetGCMemoryInfo().HeapSizeBytes / 1048576}MB " +
                        $"live={GC.GetTotalMemory(true) / 1048576}MB lastseq={agent.LastSequence}");
                    break;
                }
            }

            Volatile.Write(ref stop, true);
            server.Stop();
            agent.Stop();
        }

        #endregion

        #region "Clients"

        private static volatile bool _measuring;

        private sealed class StreamStats
        {
            public int Parts;
            public long Bytes;
            public readonly List<double> LatencyMs = new List<double>();
            public bool Connected;
            public string Error;
        }

        private static async Task<int> Run(int streams, int pollers, int rate, int seconds, int interval)
        {
            var port = GetFreePort();
            var child = Process.Start(new ProcessStartInfo(Environment.ProcessPath, $"server {port} {rate}")
            {
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false
            });

            try
            {
                if (child.StandardOutput.ReadLine() != "ready") throw new InvalidOperationException("Server did not start");

                using var handler = new SocketsHttpHandler { MaxConnectionsPerServer = int.MaxValue, UseProxy = false };
                using var http = new HttpClient(handler) { Timeout = Timeout.InfiniteTimeSpan };
                var baseUrl = $"http://127.0.0.1:{port}";
                await WaitForServer(http, baseUrl);

                using var cancel = new CancellationTokenSource();
                var streamStats = Enumerable.Range(0, streams).Select(_ => new StreamStats()).ToArray();
                var streamTasks = streamStats.Select(s => Task.Run(() => StreamClient(http, $"{baseUrl}/sample?interval={interval}&heartbeat=10000&count=1000", s, cancel.Token))).ToArray();

                var pollLatencies = new List<double>[pollers];
                var pollErrors = new int[pollers];
                var pollTasks = Enumerable.Range(0, pollers).Select(i =>
                {
                    pollLatencies[i] = new List<double>();
                    return Task.Run(() => Poller(http, $"{baseUrl}/current", pollLatencies[i], pollErrors, i, cancel.Token));
                }).ToArray();

                // Warm up (connections, JIT, initial catch-up parts), then measure
                await Task.Delay(3000);
                _measuring = true;
                child.StandardInput.WriteLine("start");
                await Task.Delay(seconds * 1000);
                child.StandardInput.WriteLine("stop");
                _measuring = false;
                string stats;
                while ((stats = child.StandardOutput.ReadLine()) != null && !stats.StartsWith("stats")) { }

                cancel.Cancel();
                await Task.WhenAny(Task.WhenAll(streamTasks.Concat(pollTasks)), Task.Delay(5000));

                var latencies = streamStats.SelectMany(s => s.LatencyMs).OrderBy(o => o).ToArray();
                var polls = pollLatencies.SelectMany(o => o).OrderBy(o => o).ToArray();
                var connected = streamStats.Count(s => s.Connected);
                var errors = streamStats.Where(s => s.Error != null).Select(s => s.Error).ToArray();

                Console.WriteLine($"streams={streams} connected={connected} failed={errors.Length} pollers={pollers} rate={rate}/s interval={interval}ms seconds={seconds}");
                if (errors.Length > 0) Console.WriteLine("  stream errors: " + string.Join("; ", errors.GroupBy(e => e).Select(g => $"{g.Count()}x {g.Key}")));
                Console.WriteLine($"  parts/s={streamStats.Sum(s => s.Parts) / (double)seconds:F0} KB/s={streamStats.Sum(s => s.Bytes) / 1024.0 / seconds:F0} " +
                                  $"latency ms p50={Percentile(latencies, 50):F0} p95={Percentile(latencies, 95):F0} p99={Percentile(latencies, 99):F0} max={(latencies.Length > 0 ? latencies[^1] : double.NaN):F0}");
                if (pollers > 0) Console.WriteLine($"  current/s={polls.Length / (double)seconds:F0} ms p50={Percentile(polls, 50):F1} p99={Percentile(polls, 99):F1} errors={pollErrors.Sum()}");
                foreach (var e in _pollErrors.OrderByDescending(o => o.Value).Take(5)) Console.WriteLine($"    {e.Value}x {e.Key}");
                Console.WriteLine("  server " + stats);
                return 0;
            }
            finally
            {
                if (!child.WaitForExit(5000)) child.Kill(true);
            }
        }

        private static async Task StreamClient(HttpClient http, string url, StreamStats stats, CancellationToken cancel)
        {
            try
            {
                using var response = await http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancel);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    stats.Error = "HTTP " + (int)response.StatusCode;
                    return;
                }

                var contentType = response.Content.Headers.ContentType;
                var boundary = "--" + contentType?.Parameters.FirstOrDefault(p => p.Name.Equals("boundary", StringComparison.OrdinalIgnoreCase))?.Value?.Trim('"');
                using var stream = await response.Content.ReadAsStreamAsync(cancel);
                var reader = new PartReader(stream);

                while (!cancel.IsCancellationRequested)
                {
                    // Skip to the next boundary (also skips the CRLF that ends the previous part)
                    string line;
                    do line = await reader.ReadLineAsync(cancel);
                    while (line != null && line != boundary);
                    if (line == null) break;

                    var length = -1;
                    while (!string.IsNullOrEmpty(line = await reader.ReadLineAsync(cancel)))
                    {
                        if (line.StartsWith("Content-length:", StringComparison.OrdinalIgnoreCase)) length = int.Parse(line.Substring(15).Trim());
                    }
                    if (line == null || length < 0) break;

                    var body = await reader.ReadBytesAsync(length, cancel);
                    if (body == null) break;
                    stats.Connected = true;

                    if (_measuring)
                    {
                        var received = DateTime.UtcNow.Ticks;
                        stats.Parts++;
                        stats.Bytes += length;
                        var newest = NewestValueTicks(body);
                        if (newest > 0) stats.LatencyMs.Add((received - newest) / (double)TimeSpan.TicksPerMillisecond);
                    }
                }

                if (!cancel.IsCancellationRequested) stats.Error = "stream ended";
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                if (!cancel.IsCancellationRequested) stats.Error = ex.GetType().Name + ": " + ex.Message;
            }
        }

        private static async Task Poller(HttpClient http, string url, List<double> latencies, int[] errors, int index, CancellationToken cancel)
        {
            var localErrors = 0;
            while (!cancel.IsCancellationRequested)
            {
                var sw = Stopwatch.StartNew();
                try
                {
                    using var response = await http.GetAsync(url, cancel);
                    await response.Content.ReadAsByteArrayAsync(cancel);
                    if (response.StatusCode != HttpStatusCode.OK) { localErrors++; RecordPollError("HTTP " + (int)response.StatusCode); }
                    else if (_measuring) latencies.Add(sw.Elapsed.TotalMilliseconds);
                }
                catch (OperationCanceledException) { break; }
                catch (Exception ex) { localErrors++; RecordPollError(ex.GetType().Name + ": " + (ex.InnerException?.Message ?? ex.Message)); }
            }
            errors[index] = localErrors;
        }

        private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, int> _pollErrors = new System.Collections.Concurrent.ConcurrentDictionary<string, int>();
        private static void RecordPollError(string error) => _pollErrors.AddOrUpdate(error, 1, (k, v) => v + 1);

        // Values are "T<ticks>"; returns the newest one in the part (0 when there is none)
        private static long NewestValueTicks(byte[] body)
        {
            long newest = 0;
            for (var i = 0; i < body.Length - 2; i++)
            {
                if (body[i] == (byte)'>' && body[i + 1] == (byte)'T' && body[i + 2] >= (byte)'0' && body[i + 2] <= (byte)'9')
                {
                    long value = 0;
                    var j = i + 2;
                    while (j < body.Length && body[j] >= (byte)'0' && body[j] <= (byte)'9') value = value * 10 + (body[j++] - '0');
                    if (value > newest) newest = value;
                    i = j;
                }
            }
            return newest;
        }

        private sealed class PartReader
        {
            private readonly Stream _stream;
            private readonly byte[] _buffer = new byte[65536];
            private int _position;
            private int _length;

            public PartReader(Stream stream) => _stream = stream;

            private async Task<bool> FillAsync(CancellationToken cancel)
            {
                _position = 0;
                _length = await _stream.ReadAsync(_buffer, 0, _buffer.Length, cancel);
                return _length > 0;
            }

            public async Task<string> ReadLineAsync(CancellationToken cancel)
            {
                var line = new StringBuilder();
                while (true)
                {
                    if (_position >= _length && !await FillAsync(cancel)) return null;
                    var b = _buffer[_position++];
                    if (b == (byte)'\n') return line.ToString().TrimEnd('\r');
                    line.Append((char)b);
                }
            }

            public async Task<byte[]> ReadBytesAsync(int count, CancellationToken cancel)
            {
                var result = new byte[count];
                var offset = 0;
                while (offset < count)
                {
                    if (_position >= _length && !await FillAsync(cancel)) return null;
                    var n = Math.Min(count - offset, _length - _position);
                    Buffer.BlockCopy(_buffer, _position, result, offset, n);
                    _position += n;
                    offset += n;
                }
                return result;
            }
        }

        #endregion

        private static async Task WaitForServer(HttpClient http, string baseUrl)
        {
            var deadline = DateTime.UtcNow.AddSeconds(15);
            while (DateTime.UtcNow < deadline)
            {
                try
                {
                    using var response = await http.GetAsync(baseUrl + "/probe");
                    if (response.StatusCode == HttpStatusCode.OK) return;
                }
                catch { }
                await Task.Delay(100);
            }
            throw new TimeoutException("Server did not respond");
        }

        private static double Percentile(double[] sorted, int percentile)
        {
            if (sorted.Length == 0) return double.NaN;
            return sorted[Math.Min(sorted.Length - 1, (int)(sorted.Length * percentile / 100.0))];
        }

        private static int GetFreePort()
        {
            using var probe = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            probe.Bind(new IPEndPoint(IPAddress.Loopback, 0));
            return ((IPEndPoint)probe.LocalEndPoint).Port;
        }
    }
}
