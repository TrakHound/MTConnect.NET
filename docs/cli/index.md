# CLI tools

Reference pages for the command-line tools that ship with `MTConnect.NET`. Each page lists the supported subcommands or flags, an example invocation that runs against a real working directory, the config-file knobs the tool honors (with defaults and permissible values), and cross-links into the rest of the docs.

## Shipped CLIs

These tools ship as released binaries (or NuGet templates that produce released binaries). End users run them in production.

- **[Agent](./agent)** — `MTConnect-Agent`. The standalone HTTP agent host. Hosts the `MTConnectAgentBroker`, every configured agent module (HTTP server, MQTT broker, MQTT relay, SHDR adapter, MQTT adapter, HTTP adapter), and the Windows-service / systemd-unit lifecycle wrappers. Reads `agent.config.yaml` (or `agent.config.json`) at startup.
- **[Adapter](./adapter)** — `MTConnect-Adapter`. The standalone adapter host. Pumps observations from a data source (SHDR socket, MQTT topic, or a custom `DataSource` implementation) into a downstream agent. Reads `adapter.config.yaml` at startup.

Tools for working on the repository itself (`tools/test.sh`, `tools/dotnet.sh`, the SysML importer and the release builder) are documented for contributors in [`contributing/`](https://github.com/TrakHound/MTConnect.NET/tree/master/contributing).

## Cross-references

- [Configure & Use → Configure an agent](/configure/agent-config) — how to author the `agent.config.yaml` file the agent CLI reads.
- [Configure & Use → Configure an adapter](/configure/adapter-config) — how to author the `adapter.config.yaml` file the adapter CLI reads.
- [Configure & Use → Run](/configure/run) — how to invoke the agent / adapter in development, in Docker, or as a Windows service / systemd unit.
- [Modules](/modules/) — the per-module configuration the agent CLI loads at startup.
- [API reference](/api/) — the C# types that back each CLI (e.g. `MTConnectAgentApplication`, `MTConnectAdapterApplication`, `AgentApplicationConfiguration`, `AdapterApplicationConfiguration`).
