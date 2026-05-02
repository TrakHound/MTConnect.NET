# Install

`MTConnect.NET` ships in three forms: a standalone agent application, a standalone adapter application, and a set of embeddable NuGet libraries. Pick the shape that matches what you are building.

## Decision matrix

| You want to … | Install |
|---|---|
| Run a preconfigured agent against `Devices.xml` and adapters | The standalone agent application |
| Ship data from a custom data source (PLC, sensor, file) over SHDR or MQTT | The standalone adapter application |
| Embed agent / adapter / client functionality inside your own .NET application | The NuGet libraries |
| Build a tool that parses MTConnect wire-format payloads | `MTConnect.NET-XML`, `MTConnect.NET-JSON`, or `MTConnect.NET-JSON-cppagent` NuGet packages |

## The standalone agent application

The agent ships as a self-contained executable in three deployment forms:

- **Installer**: Windows MSI, macOS pkg, Linux deb / rpm. Linked from the [project releases page](https://github.com/TrakHound/MTConnect.NET/releases).
- **Docker image**: `trakhound/mtconnect.net-agent:<version>` on Docker Hub. The image entry-point is `MTConnect.NET-Agent` with `agent.config.yaml` resolved from `/config/agent.config.yaml`.
- **Source build**: `dotnet build agent/MTConnect.NET-Agent/MTConnect.NET-Agent.csproj -c Release` from a clone of the repo.

Quick start with Docker:

```sh
docker run --rm -d --name mtc-agent \
  -p 5000:5000 \
  -v "$PWD/agent.config.yaml:/config/agent.config.yaml:ro" \
  -v "$PWD/Devices.xml:/config/devices/Devices.xml:ro" \
  trakhound/mtconnect.net-agent:latest

curl -s http://localhost:5000/probe | head -50
```

The default config exposes the HTTP server on port `5000`. The MTConnect endpoints (`/probe`, `/current`, `/sample`, `/asset`) are served under that port.

## The standalone adapter application

The adapter ships through the same release channel. The single executable reads from a custom data source (a CSV file tail, a TCP socket, a PLC's ladder API) and writes SHDR lines or MQTT messages to a configured downstream:

```sh
MTConnect.NET-Adapter --config adapter.config.yaml
```

The adapter's config file declares the input source and the output protocol. The `MTConnect.NET-Applications-Adapter` NuGet package hosts the same logic for embedding inside a custom adapter executable.

## The embeddable NuGet libraries

Every published library is on [nuget.org](https://www.nuget.org/) under the `MTConnect.NET-*` prefix. The two-layer install pattern:

- Start with the **meta package** `MTConnect.NET`, which transitively pulls every other shipped library.
- Or pin the **specific libraries** you need.

### Meta package

```sh
dotnet add package MTConnect.NET
```

This installs every library and is the right choice when you are building an embedded agent or adapter end-to-end.

### Per-library install

The library set:

| Library | Purpose |
|---|---|
| `MTConnect.NET-Common` | Devices / Components / DataItems / Observations / Assets type system. |
| `MTConnect.NET-HTTP` | HTTP-server module: `/probe`, `/current`, `/sample`, `/asset`. |
| `MTConnect.NET-SHDR` | SHDR adapter protocol — both consumer and producer. |
| `MTConnect.NET-MQTT` | MQTT broker / relay / adapter modules. |
| `MTConnect.NET-XML` | XML envelope codec — every Streams / Devices / Assets / Error envelope. |
| `MTConnect.NET-JSON` | JSON v1 envelope codec. |
| `MTConnect.NET-JSON-cppagent` | JSON-CPPAGENT v2 envelope codec (cppagent-parity). |
| `MTConnect.NET-TLS` | TLS configuration for the HTTP / MQTT modules. |
| `MTConnect.NET-Services` | Windows service / systemd-unit hosting. |
| `MTConnect.NET-DeviceFinder` | Network discovery for MTConnect agents (mDNS). |
| `MTConnect.NET-SysML` | SysML-XMI-driven source generator (build-time only). |
| `MTConnect.NET-Applications-Agents` | Agent host package — used by the standalone agent application. |
| `MTConnect.NET-Applications-Adapter` | Adapter host package — used by the standalone adapter application. |

Install one or more:

```sh
dotnet add package MTConnect.NET-Common
dotnet add package MTConnect.NET-HTTP
dotnet add package MTConnect.NET-XML
```

## Target frameworks

The libraries multi-target a wide matrix: .NET 9.0, .NET 8.0, .NET 7.0, .NET 6.0, .NET 5.0, .NET Core 3.1, .NET Standard 2.0, and .NET Framework 4.6.1 through 4.8. The matrix is declared in each `.csproj`'s `<TargetFrameworks>` property.

The .NET Standard 2.0 target is the lowest common denominator and is the target of choice for libraries embedding `MTConnect.NET` from a class library. The .NET Framework 4.6.1 target is the lowest .NET Framework version supported; consumers on .NET Framework 4.8 receive the .NET Framework 4.8 target's binary, which has access to features the 4.6.1 binary cannot use.

The standalone agent ships as a .NET 8.0 single-file executable by default; the Docker image is built from the same.

## Version pinning

Pin to a specific version in production:

```xml
<PackageReference Include="MTConnect.NET-Common" Version="6.10.0"/>
```

The library follows semantic versioning across major versions but with the caveat that the public-surface contracts are not yet formally semver-stable — minor version bumps occasionally add a property or a class. Pin to the major.minor pair (`6.10.*`) for the safest auto-update window.

## Verify the install

A two-line smoke test:

```csharp
using MTConnect;
using MTConnect.Devices;

var d = new Device { Id = "smoke-test", Name = "Smoke" };
Console.WriteLine($"Created Device {d.Uuid} with hash {d.GenerateHash()}");
```

A successful run prints the Device's UUID (a random GUID, since none was specified) and an SHA-1 hash of the empty model. If you see "package not found" instead, double-check that `MTConnect.NET-Common` is in your project's `<PackageReference>` list.

## Where to next

- [Configure an agent](/configure/agent-config) — the agent's YAML configuration.
- [Configure an adapter](/configure/adapter-config) — the adapter's YAML configuration.
- [Configure modules](/configure/module-config) — per-module configuration keys.
- [Cookbook: Write an agent](/cookbook/write-an-agent) — your first agent in code.
- [Cookbook: Write an adapter](/cookbook/write-an-adapter) — your first adapter.
