# MTConnect.NET Benchmarks

`MTConnect.NET-Benchmarks` is a small throughput harness for the agent hot paths:

- observation ingest
- `/sample` and `/current` queries
- XML and cppagent-JSON formatting
- SHDR line parsing

It uses only public APIs, so the same `Program.cs` can be copied into an older checkout and run against those libraries for a before/after comparison.

The project is not part of the solution. Run it with:

```bash
dotnet run -c Release --project benchmarks/MTConnect.NET-Benchmarks
```

Add `-- quick` for a run with one tenth of the iterations.

### Profiling

`-- profile <scenario> [seconds]` runs one scenario continuously so a profiler can attach. The scenarios are `ingest`, `sample-xml` and `sample-json`. For example, with [dotnet-trace](https://learn.microsoft.com/dotnet/core/diagnostics/dotnet-trace):

```bash
dotnet-trace collect --profile dotnet-sampled-thread-time -o ingest.nettrace -- benchmarks/MTConnect.NET-Benchmarks/bin/Release/net8.0/MTConnect.NET-Benchmarks.exe profile ingest 20
```

The run also prints garbage-collection counts and the total GC pause time.

## Scenarios

The agent has one Device with 150 EVENT, 150 SAMPLE and 20 CONDITION DataItems, a 131,072-observation buffer, and validation enabled.

| Scenario | What runs |
|---|---|
| Ingest: AddObservation (changing values) | `AddObservation` with values that change, so every call is stored |
| Ingest: AddObservation (unchanged values) | `AddObservation` with the current value, so every call is processed, then discarded as unchanged |
| Ingest: condition updates | Condition fault states that alternate between WARNING and NORMAL with different native codes |
| Query: sample (1000 observations) | `GetDeviceStreamsResponseDocument(from, 0, 1000)` |
| Query: current | `GetDeviceStreamsResponseDocument()` |
| Format: … | `ResponseDocumentFormatter.Format` for `xml` / `json-cppagent` |
| SHDR: parse multi-key line | `ShdrDataItem.FromString` on a line with 7 key/value pairs |

## Results

These were measured on 2026-10-28 on one machine: 16 logical cores, Windows 11, .NET 8.0.17 x64, Release build. The "before" column is `origin/master` (`53dd8f43`) and the "after" column is `perf/agent-hot-paths`. Each value is the mean of two runs.

| Scenario | Before (ops/s) | After (ops/s) | Change | Allocated before → after (KB/op) |
|---|---:|---:|---:|---:|
| Ingest: AddObservation (changing values) | 232,000 | 332,000 | 1.4× | 3.38 → 1.87 |
| Ingest: AddObservation (unchanged values) | 1,398,000 | 4,038,000 | 2.9× | 2.07 → 0.89 |
| Ingest: condition updates | 196,000 | 365,000 | 1.9× | 8.80 → 4.03 |
| Query: sample (1000 observations) | 739 | 6,976 | 9.4× | 1,566 → 282 |
| Query: current | 9,337 | 12,256 | 1.3× | 210 → 210 |
| Format: current as XML | 2,997 | 3,967 | 1.3× | 154 → 108 |
| Format: current as JSON (cppagent) | 546 | 1,980 | 3.6× | 518 → 133 |
| Format: sample (1000) as XML | 1,188 | 1,319 | 1.1× | 551 → 313 |
| Format: sample (1000) as JSON (cppagent) | 716 | 1,477 | 2.1× | 1,182 → 384 |
| SHDR: parse multi-key line | 243,000 | 452,000 | 1.9× | 2.58 → 2.58 |

Repeated runs on the same machine varied by up to about ±20%, so treat differences under about 1.3× as uncertain.

These numbers are single-threaded, in-process timings. They don't cover the following, which also changed:

- removing the forced garbage collections, which paused every thread;
- `/sample` scans no longer holding the ingest lock;
- event-driven HTTP streams and MQTT disconnect handling;
- the MQTT publish queues;
- network I/O.

## Garbage collection and ingest

When values change on every observation, ingest is limited by garbage collection rather than by the agent's own code. Each buffered observation keeps its values alive until the ring buffer overwrites it, so a steady stream of objects reaches the oldest generation.

With the default GC settings (workstation, concurrent), a 10-second `profile ingest` run spent about 25% of its time in GC pauses, with around 100 gen2 collections. The same run under other GC settings gave:

| GC setting | Ingest throughput | GC pause share | Peak working set |
|---|---:|---:|---:|
| Workstation, concurrent (default) | 1.0× | ~25% | 86 MB |
| Server | 1.2× | ~13% | 325 MB |
| Server with dynamic adaptation (`GCDynamicAdaptationMode=1`) | 1.1× | ~22% | 143 MB |

The GC mode is a deployment setting, so the agent doesn't change it. It can be set with `<ServerGarbageCollection>` and `<GarbageCollectionAdaptationMode>` in the project file, or with the `DOTNET_gcServer` and `DOTNET_GCDynamicAdaptationMode` environment variables.

## Forced collections

The agent used to request full, blocking collections through a `GarbageCollector` helper class: after every `/sample` query, and after loading the file buffer at startup. Forcing collections no longer reduced memory once the query path stopped making large intermediate copies, so the helper has been removed.

`profile large-sample` runs 10,000-observation `/sample` queries formatted as XML while ingest continues. `profile large-sample-forced-gc` does the same and forces a full collection after each query, at most every 500 ms, as the helper did. Mean of two 20-second runs:

| Variant | Queries | GC pause time | Peak working set | Peak GC heap |
|---|---:|---:|---:|---:|
| No forced collections | 3,135 | 2.6 s | 122 MB | 55 MB |
| Forced collection after each query | 3,032 | 3.8 s | 128 MB | 67 MB |

Forced collections added GC pause time without lowering peak memory.
