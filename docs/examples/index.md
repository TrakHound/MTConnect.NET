# Examples

Runnable, self-contained sample applications in the `examples/` directory of the repository. Each example is a small .NET console project that demonstrates one integration shape — embedding an agent inside an application, or consuming an agent from a client.

## Available examples

- **[Embedded agent](/examples/agent-embedded)** — a console application that hosts an MTConnect agent inside its own process and writes observations into it directly, without an external SHDR adapter or TCP hop.
- **[HTTP client](/examples/client-http)** — a console application that connects to a running MTConnect agent over HTTP, polls `/probe` and `/current`, and prints each observation as it arrives.
- **[MQTT client](/examples/client-mqtt)** — a console application that subscribes to an MQTT broker carrying MTConnect-formatted messages (typically published by the `mqtt-relay` or `mqtt-broker` agent modules) and prints each device and observation it receives.
- **[SHDR client](/examples/client-shdr)** — a console application that connects to a raw SHDR endpoint and prints each protocol line as it arrives. Useful when debugging an adapter or building a custom consumer of the SHDR stream.

## How to run an example

Each example is a standalone .NET project. From the repository root:

```
dotnet run --project examples/MTConnect.NET-Agent-Embedded
dotnet run --project examples/MTConnect.NET-Client-HTTP
dotnet run --project examples/MTConnect.NET-Client-MQTT
dotnet run --project examples/MTConnect.NET-Client-SHDR
```

The client examples prompt for connection details on standard input (hostname, port). The embedded-agent example reads its module configuration from `examples/MTConnect.NET-Agent-Embedded/agent.config.yaml`.

## See also

- [Getting started](/getting-started) — the shortest path from a fresh checkout to a running agent.
- [Cookbook](/cookbook/) — task-driven recipes that mirror the patterns these examples illustrate.
- [Modules](/modules/) — the agent modules the examples interact with on the wire (HTTP server, MQTT relay/broker, SHDR adapter).
