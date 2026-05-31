---
layout: home

hero:
  name: MTConnect.NET
  text: The .NET implementation of the MTConnect Standard
  tagline: 100% public-surface API coverage. 100% MTConnect Standard compliance.
  actions:
    - theme: brand
      text: Get started
      link: /getting-started
    - theme: alt
      text: View on GitHub
      link: https://github.com/TrakHound/MTConnect.NET

features:
  - title: MTConnect Standard compliance
    details: |
      Every shipped envelope (Streams, Devices, Assets, Error) validates against the corresponding XSD across MTConnect v1.0 through v2.7. Wire output is byte-for-byte cppagent-parity for the JSON-CPPAGENT v2 array-of-wrappers codec.
    link: /compliance/
    linkText: Compliance posture
  - title: Public-surface API coverage
    details: |
      Every public class, interface, method, property, event, and enum has a documentation page generated from the library's XML doc comments — with a real usage example.
    link: /api/
    linkText: API reference
  - title: Configure-and-use guides
    details: |
      End-to-end guides for installing the NuGet packages, configuring an agent, configuring an adapter, running locally / in Docker / as a service, and connecting a consumer over HTTP or MQTT.
    link: /configure/
    linkText: Configure & Use
  - title: Wire-format reference
    details: |
      XML, JSON v1, JSON-CPPAGENT (v2 codec), JSON-CPPAGENT-MQTT, SHDR — sample envelopes, codec class names, round-trip pseudocode, and Mermaid diagrams for the wire-flow handshakes.
    link: /wire-formats/
    linkText: Wire formats
  - title: Cookbook
    details: |
      Recipes for common problems — write an agent, write an adapter, write a module, configure the MQTT relay, write a JSON-MQTT consumer.
    link: /cookbook/
    linkText: Cookbook
  - title: Runnable examples
    details: |
      Four self-contained console applications in `examples/` — an embedded agent, an HTTP client, an MQTT client, and an SHDR client. Each is `dotnet run`-able directly from the repository.
    link: /examples/
    linkText: Examples
  - title: Troubleshooting
    details: |
      XSD validation failures, schema-version mismatches, MQTT TLS handshake failures — common errors and the fixes that work.
    link: /troubleshooting/
    linkText: Troubleshooting
---

## What is MTConnect.NET?

`MTConnect.NET` is a fully open-source .NET library set, agent application, and adapter application that implements the [MTConnect Standard](https://www.mtconnect.org/) end to end. It ships:

- **A standalone agent application** — a preconfigured executable that hosts an MTConnect agent with a modular architecture (HTTP server, MQTT broker, MQTT relay, SHDR adapter, MQTT adapter, HTTP adapter). Runs on Windows, Linux, and macOS, with installers, a Docker image, and Windows-service / systemd-unit deployment paths.
- **A standalone adapter application** — for shipping data from a PLC or other source into an MTConnect agent over SHDR or MQTT.
- **Embeddable libraries** — fourteen NuGet packages (`MTConnect.NET`, `MTConnect.NET-Common`, `MTConnect.NET-HTTP`, `MTConnect.NET-SHDR`, `MTConnect.NET-MQTT`, `MTConnect.NET-XML`, `MTConnect.NET-JSON`, `MTConnect.NET-JSON-cppagent`, `MTConnect.NET-TLS`, `MTConnect.NET-Services`, `MTConnect.NET-DeviceFinder`, `MTConnect.NET-SysML`, plus the `MTConnect.NET-Applications-Agents` and `MTConnect.NET-Applications-Adapter` host packages) for embedding agent / adapter / client functionality inside a custom application.
- **Spec coverage through v2.7** — the SysML model drives source generation, so every spec-version-introduced type is present, every deprecation is recorded, and every type carries its `MinimumVersion` / `MaximumVersion` metadata.

## Target frameworks

The libraries multi-target .NET 9.0, .NET 8.0, .NET 7.0, .NET 6.0, .NET 5.0, .NET Core 3.1, .NET Standard 2.0, and .NET Framework 4.6.1 through 4.8.

## Architecture at a glance

```mermaid
flowchart LR
  subgraph adapters[Adapters]
    SHDR[SHDR adapter]
    HTTPADAPT[HTTP adapter]
  end

  subgraph agent[MTConnect.NET Agent]
    direction TB
    INPUT[Input pipeline]
    BUFFER[Sequence buffer]
    PROBE[Probe / Current / Sample / Asset]
    MODULES[Modules]
  end

  subgraph wire[Wire formats]
    XMLENV[XML envelope]
    JSONV2[JSON-CPPAGENT v2]
    MQTT[MQTT relay topics]
  end

  subgraph consumers[Consumers]
    DASHBOARD[Dashboards]
    BRIDGE[Bridges]
    HISTORIAN[Historians]
  end

  SHDR --> INPUT
  HTTPADAPT --> INPUT
  INPUT --> BUFFER
  BUFFER --> PROBE
  PROBE --> MODULES
  MODULES --> XMLENV
  MODULES --> JSONV2
  MODULES --> MQTT
  XMLENV --> DASHBOARD
  JSONV2 --> BRIDGE
  MQTT --> HISTORIAN
```

The diagram above is rendered by the Mermaid plugin. Every architecture / sequence / state-machine / wire-flow diagram in this site is authored in Mermaid — no ASCII art, no external image renders for schematic content.

## Where to next

- New to MTConnect.NET? Start with [Getting started](/getting-started).
- Deciding whether the library covers your spec target? Read the [Compliance](/compliance/) posture.
- Standing up an agent against real equipment? Walk through [Configure & Use](/configure/).
- Looking up a specific class or method? Open the [API reference](/api/).
- Want to see a working program first? Browse the [Examples](/examples/) — four `dotnet run`-able console apps for the common integration shapes.
