# CLI tools

Reference pages for every shipped and in-repo command-line tool that the `MTConnect.NET` codebase exposes. Each page lists the supported subcommands or flags, an example invocation that runs against a real working directory, the config-file knobs the tool honors (with defaults and permissible values), and cross-links into the rest of the docs.

## Shipped CLIs

These tools ship as released binaries (or NuGet templates that produce released binaries). End users run them in production.

- **[Agent](./agent)** — `MTConnect-Agent`. The standalone HTTP agent host. Hosts the `MTConnectAgentBroker`, every configured agent module (HTTP server, MQTT broker, MQTT relay, SHDR adapter, MQTT adapter, HTTP adapter), and the Windows-service / systemd-unit lifecycle wrappers. Reads `agent.config.yaml` (or `agent.config.json`) at startup.
- **[Adapter](./adapter)** — `MTConnect-Adapter`. The standalone adapter host. Pumps observations from a data source (SHDR socket, MQTT topic, or a custom `DataSource` implementation) into a downstream agent. Reads `adapter.config.yaml` at startup.

## Contributor CLIs

These tools live in the repo but are not shipped to end users. They exist for contributors working on the codebase itself.

- **[`tools/dotnet.sh`](./dotnet-sh)** — wraps the `dotnet` CLI so every contributor command runs against either the host `dotnet` on `PATH` or an official `mcr.microsoft.com/dotnet/sdk` container. Used by every other contributor script in the repo.
- **[`tools/test.sh`](./test-sh)** — the local test entry point. Iterates every `tests/**/*.csproj`, optionally pulls in the Compliance tier and the Docker-gated E2E tier, and emits a unified coverage report under `coverage-report/`.
- **[SysML importer](./sysml-import)** — `MTConnect.NET-SysML-Import`. Parses an `MTConnectSysMLModel.xml` (the XMI export of the standard's SysML model) and regenerates the `*.g.cs` source files under `libraries/MTConnect.NET-Common/`, `libraries/MTConnect.NET-XML/`, and `libraries/MTConnect.NET-JSON-cppagent/`. Run by maintainers when a new spec version lands.

A PowerShell sibling lives alongside each shell script (`tools/dotnet.ps1`, `tools/test.ps1`) with identical semantics and flag names. Each shell-script page calls out the parameter spelling difference; the underlying behavior is the same on Windows, macOS, and Linux.

## Cross-references

- [Configure & Use → Configure an agent](/configure/agent) — how to author the `agent.config.yaml` file the agent CLI reads.
- [Configure & Use → Configure an adapter](/configure/adapter) — how to author the `adapter.config.yaml` file the adapter CLI reads.
- [Configure & Use → Run](/configure/run) — how to invoke the agent / adapter in development, in Docker, or as a Windows service / systemd unit.
- [Modules](/modules/) — the per-module configuration the agent CLI loads at startup.
- [API reference](/api/) — the C# types that back each CLI (e.g. `MTConnectAgentApplication`, `MTConnectAdapterApplication`, `AgentApplicationConfiguration`, `AdapterApplicationConfiguration`).
