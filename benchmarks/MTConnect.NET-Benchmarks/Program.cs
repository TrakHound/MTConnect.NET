// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Formatters;
using MTConnect.Observations;
using MTConnect.Shdr;
using MTConnect.Streams.Output;
using System;
using System.Diagnostics;
using System.Linq;

namespace MTConnect.Benchmarks
{
    /// <summary>
    /// Throughput harness for the agent hot paths: observation ingest, /sample queries, response formatting and SHDR
    /// parsing. Uses only public APIs so the same file can be run against different versions of the libraries.
    /// Usage: dotnet run -c Release --project benchmarks/MTConnect.NET-Benchmarks [-- quick]
    /// </summary>
    public static class Program
    {
        private const string DeviceUuid = "bench-device";
        private const int Events = 150;
        private const int Samples = 150;
        private const int Conditions = 20;

        private static int _scale = 1;

        public static void Main(string[] args)
        {
            if (args.Contains("quick")) _scale = 10;

            // Profile mode: run one scenario continuously so a profiler can attach (e.g. dotnet-trace)
            // Usage: -- profile ingest|sample-xml|sample-json [seconds]
            if (args.Length > 1 && args[0] == "profile")
            {
                Profile(args[1], args.Length > 2 ? int.Parse(args[2]) : 30);
                return;
            }

            Console.WriteLine($"MTConnect.NET hot path benchmarks ({Environment.Version}, {(Environment.Is64BitProcess ? "x64" : "x86")})");
            Console.WriteLine($"{"Scenario",-44}{"ops/s",14}{"us/op",12}{"KB/op",12}");

            var agent = CreateAgent();

            Run("Ingest: AddObservation (changing values)", 200000 / _scale, i =>
            {
                var n = i % (Events + Samples);
                if (n < Events) agent.AddObservation(DeviceUuid, "e" + n, (object)("VALUE_" + (i % 7)), DateTime.UtcNow);
                else agent.AddObservation(DeviceUuid, "s" + (n - Events), (i % 1000) * 0.5, DateTime.UtcNow);
            });

            Run("Ingest: AddObservation (unchanged values)", 200000 / _scale, i =>
            {
                var n = i % Events;
                agent.AddObservation(DeviceUuid, "e" + n, (object)"VALUE_0", DateTime.UtcNow);
            });

            Run("Ingest: condition updates", 50000 / _scale, i =>
            {
                var n = i % Conditions;
                var level = (i % 3) == 0 ? ConditionLevel.WARNING : ConditionLevel.NORMAL;
                agent.AddObservation(DeviceUuid, new Input.ConditionFaultStateObservationInput("c" + n, level) { NativeCode = "N" + (i % 4) });
            });

            var last = agent.LastSequence;
            Run("Query: sample (1000 observations)", 2000 / _scale, i =>
            {
                var from = last > 5000 ? last - 5000 + (ulong)(i % 4000) : 1;
                agent.GetDeviceStreamsResponseDocument(from, 0, 1000);
            });

            Run("Query: current", 5000 / _scale, i =>
            {
                agent.GetDeviceStreamsResponseDocument();
            });

            var current = agent.GetDeviceStreamsResponseDocument();
            var sample = agent.GetDeviceStreamsResponseDocument(last > 1000 ? last - 1000 : 1, 0, 1000);

            Run("Format: current as XML", 2000 / _scale, i => Format("xml", current));
            Run("Format: current as JSON (cppagent)", 2000 / _scale, i => Format("json-cppagent", current));
            Run("Format: sample (1000) as XML", 1000 / _scale, i => Format("xml", sample));
            Run("Format: sample (1000) as JSON (cppagent)", 1000 / _scale, i => Format("json-cppagent", sample));

            var shdrLine = "2024-01-01T00:00:00.000Z|e1|ACTIVE|e2|AUTOMATIC|s1|123.456|s2|7.5|e3|O1234|s3|0.001|e4|READY";
            Run("SHDR: parse multi-key line", 200000 / _scale, i => ShdrDataItem.FromString(shdrLine).Count());
        }

        private static void Profile(string scenario, int seconds)
        {
            var agent = CreateAgent();
            for (var i = 0; i < 20000; i++) agent.AddObservation(DeviceUuid, "e" + (i % Events), (object)("VALUE_" + (i % 7)), DateTime.UtcNow);
            var last = agent.LastSequence;
            var sample = agent.GetDeviceStreamsResponseDocument(last - 1000, 0, 1000);

            Console.WriteLine($"Profiling '{scenario}' for {seconds}s (pid {Environment.ProcessId})");
            var sw = Stopwatch.StartNew();
            long i2 = 0;
            while (sw.Elapsed.TotalSeconds < seconds)
            {
                switch (scenario)
                {
                    case "ingest":
                        var n = (int)(i2 % (Events + Samples));
                        if (n < Events) agent.AddObservation(DeviceUuid, "e" + n, (object)("VALUE_" + (i2 % 7)), DateTime.UtcNow);
                        else agent.AddObservation(DeviceUuid, "s" + (n - Events), (i2 % 1000) * 0.5, DateTime.UtcNow);
                        break;
                    case "buffer-observation":
                        var values = new System.Collections.Generic.List<ObservationValue> { new ObservationValue(ValueKeys.Result, "VALUE_1") };
                        new MTConnect.Buffers.BufferObservation(1, DataItemCategory.EVENT, DataItemRepresentation.VALUE, values, 1, 1, Quality.VALID, false, false);
                        break;
                    case "sample-xml": Format("xml", sample); break;
                    case "sample-json": Format("json-cppagent", sample); break;
                    default: throw new ArgumentException("Unknown scenario " + scenario);
                }
                i2++;
            }
            Console.WriteLine($"{i2} operations; GC gen0/1/2 = {GC.CollectionCount(0)}/{GC.CollectionCount(1)}/{GC.CollectionCount(2)}; GC pause {GC.GetTotalPauseDuration().TotalMilliseconds:N0} ms of {sw.Elapsed.TotalMilliseconds:N0} ms");
        }

        private static void Format(string formatId, IStreamsResponseOutputDocument document)
        {
            var result = ResponseDocumentFormatter.Format(formatId, ref document);
            if (!result.Success) throw new InvalidOperationException("Format failed : " + formatId);
            result.Content?.Dispose();
        }

        private static MTConnectAgentBroker CreateAgent()
        {
            var configuration = new AgentConfiguration { ObservationBufferSize = 131072, EnableValidation = true };
            var agent = new MTConnectAgentBroker(configuration);
            agent.Start();

            var device = new Device { Id = "bench", Name = "bench", Uuid = DeviceUuid };
            for (var i = 0; i < Events; i++) device.AddDataItem(new DataItem { Id = "e" + i, Name = "e" + i, Category = DataItemCategory.EVENT, Type = "MESSAGE", Representation = DataItemRepresentation.VALUE });
            for (var i = 0; i < Samples; i++) device.AddDataItem(new DataItem { Id = "s" + i, Name = "s" + i, Category = DataItemCategory.SAMPLE, Type = "POSITION", Units = "MILLIMETER", Representation = DataItemRepresentation.VALUE });
            for (var i = 0; i < Conditions; i++) device.AddDataItem(new DataItem { Id = "c" + i, Name = "c" + i, Category = DataItemCategory.CONDITION, Type = "SYSTEM", Representation = DataItemRepresentation.VALUE });

            if (agent.AddDevice(device) == null) throw new InvalidOperationException("AddDevice failed");
            return agent;
        }

        private static void Run(string name, int iterations, Action<int> action)
        {
            // Warm up (JIT, caches)
            var warmup = Math.Max(1, iterations / 10);
            for (var i = 0; i < warmup; i++) action(i);

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            var allocatedBefore = GC.GetAllocatedBytesForCurrentThread();
            var sw = Stopwatch.StartNew();
            for (var i = 0; i < iterations; i++) action(warmup + i);
            sw.Stop();
            var allocated = GC.GetAllocatedBytesForCurrentThread() - allocatedBefore;

            var opsPerSecond = iterations / sw.Elapsed.TotalSeconds;
            var microsecondsPerOp = sw.Elapsed.TotalMilliseconds * 1000 / iterations;
            var kilobytesPerOp = allocated / 1024.0 / iterations;

            Console.WriteLine($"{name,-44}{opsPerSecond,14:N0}{microsecondsPerOp,12:N2}{kilobytesPerOp,12:N2}");
        }
    }
}
