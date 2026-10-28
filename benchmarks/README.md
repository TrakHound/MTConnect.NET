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
| Ingest: AddObservation (changing values) | 232,000 | 279,000 | 1.2× | 3.38 → 2.83 |
| Ingest: AddObservation (unchanged values) | 1,398,000 | 2,025,000 | 1.4× | 2.07 → 1.32 |
| Ingest: condition updates | 196,000 | 317,000 | 1.6× | 8.80 → 4.64 |
| Query: sample (1000 observations) | 739 | 6,882 | 9.3× | 1,566 → 282 |
| Query: current | 9,337 | 13,219 | 1.4× | 210 → 210 |
| Format: current as XML | 2,997 | 3,977 | 1.3× | 154 → 154 |
| Format: current as JSON (cppagent) | 546 | 1,448 | 2.7× | 518 → 353 |
| Format: sample (1000) as XML | 1,188 | 1,205 | 1.0× | 551 → 551 |
| Format: sample (1000) as JSON (cppagent) | 716 | 917 | 1.3× | 1,182 → 1,179 |
| SHDR: parse multi-key line | 243,000 | 474,000 | 2.0× | 2.58 → 2.58 |

Repeated runs on the same machine varied by up to about ±12%.

These numbers are single-threaded, in-process timings. They don't cover the following, which also changed:

- removing the forced garbage collections, which paused every thread;
- `/sample` scans no longer holding the ingest lock;
- event-driven HTTP streams and MQTT disconnect handling;
- the MQTT publish queues;
- network I/O.
