# SHDR client

- **Source path** — `examples/MTConnect.NET-Client-SHDR/`
- **Project type** — .NET console application
- **NuGet package** — [`MTConnect.NET-SHDR`](https://www.nuget.org/packages/MTConnect.NET-SHDR)

## Purpose

Demonstrates a minimal SHDR consumer: connect to a TCP endpoint that speaks the SHDR protocol, print every protocol line as it arrives, and exit cleanly on shutdown. Useful when debugging an SHDR adapter, building a custom downstream consumer, or replaying a captured stream against a parser.

## Build and run

Point the example at a running SHDR endpoint — typically an `shdr-output`-enabled agent module, the `mtconnect-adapter` CLI, or a custom adapter. Then:

```
dotnet run --project examples/MTConnect.NET-Client-SHDR
```

The example prompts for hostname and port on standard input. Once connected, each SHDR line prints verbatim:

```
Connecting to (localhost:7878)..
Connection Established
2026-05-15T11:08:43.0000000Z|availability|AVAILABLE
2026-05-15T11:08:43.0000000Z|emergencystop|ARMED
2026-05-15T11:08:43.0000000Z|x_position_actual|0.0002
```

## How the example is structured

`Program.cs` is the smallest of the four examples — it wires three events on `ShdrClient` and starts the connection:

```csharp
var client = new ShdrClient(hostname, port);

client.Connected        += (s, e)    => Console.WriteLine("Connection Established");
client.ProtocolReceived += (s, line) => Console.WriteLine(line);
client.Disconnected     += (s, e)    => Console.WriteLine("Disconnected");

client.Start();
Console.ReadLine();
client.Stop();
```

`ShdrClient` handles the underlying TCP loop, reconnect-on-disconnect behaviour, and per-line buffering. `ProtocolReceived` fires once per `\n`-terminated line; the consumer is responsible for parsing the pipe-delimited fields itself if it needs structured data.

## When to pick this client

Choose `ShdrClient` when you want:

- the raw on-the-wire SHDR text, not parsed observations (debugging, replay, custom routing),
- a smaller dependency footprint than the HTTP or MQTT client (no XML/JSON parser, no envelope reconstruction),
- a pluggable building block for a custom adapter that needs to consume one SHDR stream and emit another.

If you want parsed observations rather than raw lines, point an agent at the same SHDR endpoint with the `shdr-adapter` module and consume `/current` or `/sample` from the agent instead.

## See also

- [SHDR wire format](/wire-formats/shdr) — the line-level grammar and the canonical examples.
- [SHDR adapter module](/modules/shdr-adapter) — the agent-side consumer that turns SHDR lines into observations.
- [SHDR output module](/modules/shdr-output) — the agent-side emitter that produces these lines on the wire.
- [Cookbook: write an adapter](/cookbook/write-an-adapter) — building a producer instead of a consumer.
