# Getting started

This walkthrough takes you from a blank machine to a running `MTConnect.NET` agent serving the `/probe` and `/current` endpoints to a local consumer, in three steps:

1. Install the agent (NuGet template, prebuilt binary, or Docker).
2. Run it locally.
3. Point a consumer at it.

For a deeper tour — every config knob, every module, every wire format — head to [Configure & Use](/configure/) once you have the agent running.

## 1. Install

Choose the path that matches how you want to deploy.

### Option A: scaffold a custom agent from the dotnet template

The fastest way to embed an agent inside your own application. Requires the [.NET SDK](https://dotnet.microsoft.com/download) (9.0 recommended).

```bash
dotnet new install MTConnect.NET-Agent-Template
mkdir my-agent && cd my-agent
dotnet new mtconnect.net-agent
dotnet run
```

The template produces a `Program.cs` with an `MTConnectAgentApplication` host plus a sample data-source module wired up to it. Open `agent.config.yaml` to point the data-source at your PLC, or remove the sample module and add your own.

### Option B: add the agent NuGet package to an existing project

```bash
dotnet add package MTConnect.NET-Applications-Agents
```

Then in `Program.cs`:

```csharp
using MTConnect.Applications;

namespace MyAgent
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var app = new MTConnectAgentApplication();
            app.Run(args, true);
        }
    }
}
```

`MTConnectAgentApplication` handles the HTTP server, SHDR adapters, command-line arguments, device management, buffer management, logging, and Windows-service hosting. Add or remove modules through `agent.config.yaml`.

### Option C: download the prebuilt agent

Grab the latest installer (Windows) or zip (Linux / macOS) from the [releases page](https://github.com/TrakHound/MTConnect.NET/releases/latest). The installer drops the agent into `Program Files\MTConnect.NET\Agent\` (Windows) or wherever you extract the archive (Linux / macOS) and ships with a default `agent.config.yaml` and a sample `devices/` directory.

### Option D: Docker

```bash
docker pull trakhound/mtconnect.net-agent
docker run --rm -p 5000:5000 trakhound/mtconnect.net-agent
```

Mount your own `agent.config.yaml` and `devices/` directory with `-v`. See [Run](/configure/run) for the production-grade invocation.

## 2. Run it

If you used the template or installed the prebuilt binary, the default config opens an HTTP server on port `5000`:

```bash
dotnet run                               # template / source build
./MTConnect.NET-Agent                    # prebuilt binary on Linux / macOS
MTConnect.NET-Agent.exe                  # prebuilt binary on Windows
```

The console prints the agent header, the loaded modules, and the discovered devices. The HTTP server is now listening on `http://localhost:5000/`.

The shipped `agent.config.yaml` enables the HTTP server module by default. To add or remove modules — MQTT broker, MQTT relay, SHDR adapter, MQTT adapter, HTTP adapter — uncomment the relevant block in the config file or add a new one. The agent picks up config changes automatically.

## 3. Point a consumer at it

The HTTP server speaks the MTConnect REST API. Three endpoints cover most use cases:

```bash
# What devices and data items does this agent expose?
curl http://localhost:5000/probe

# What is the most recent value for every data item?
curl http://localhost:5000/current

# Stream historic samples from sequence 100 onwards.
curl 'http://localhost:5000/sample?from=100&count=100'
```

Append `Accept: application/json` (or `?documentFormat=json`) to receive the JSON-CPPAGENT v2 codec output instead of XML:

```bash
curl -H 'Accept: application/json' http://localhost:5000/current
```

For an MQTT-based consumer, enable the `mqtt-relay` module in `agent.config.yaml` and point a subscriber at the relay's topic tree. See [Wire formats](/wire-formats/) for the topic shape and codec details.

For a programmatic .NET consumer, use the `MTConnectHttpClient` or `MTConnectMqttClient` classes from `MTConnect.NET-HTTP` / `MTConnect.NET-MQTT`:

```csharp
using MTConnect.Clients;

var client = new MTConnectHttpClient("http://localhost:5000");
var probe = await client.GetProbeAsync();

foreach (var device in probe.Devices)
{
    Console.WriteLine($"{device.Name}: {device.DataItems.Count} data items");
}
```

## Next steps

- Add real data: connect an [adapter](/configure/adapter) over SHDR or MQTT.
- Deploy for production: run the agent [as a service](/configure/run) on Windows or Linux.
- Build a custom module: read the [Cookbook](/cookbook/) recipe for writing a module.
- Understand the data model: read [Concepts](/concepts/) for Devices, Components, DataItems, Observations, and Assets.
