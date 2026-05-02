# Agent CLI

`MTConnect-Agent` is the standalone HTTP agent host. It loads an `agent.config.yaml` (or `agent.config.json`) configuration file, builds an `MTConnectAgentBroker`, instantiates every configured agent module, and runs them inside a single process that can be invoked from a terminal, hosted as a Windows service, or run as a systemd unit.

The CLI surface is defined by `MTConnectAgentApplication.Run(args)` in `agent/MTConnect.NET-Applications-Agents/MTConnectAgentApplication.cs`. The shipped `mtconnect.net-agent` binary in `agent/MTConnect.NET-Agent/` is a thin entry point that calls `Run(args, true)` on this class. Embedders that scaffold their own agent from the `MTConnect.NET-Agent-Template` NuGet template get the same CLI surface for free.

## Synopsis

```text
mtconnect.net-agent [help|install|install-start|start|stop|remove|debug|run|run-service] [configuration_file]
```

If no command is given, the agent runs in `run` mode (foreground, info-level logging). If no configuration file is given, the agent looks for `agent.config.json` first, then `agent.config.yaml`, then copies `agent.config.default.yaml` into place if neither file exists.

## Commands

| Command | Description |
|---|---|
| `help` | Print usage information and exit. |
| `run` | Run the agent in the foreground at info-level logging. Default when no command is supplied. The process blocks until killed with `Ctrl+C` or SIGINT. |
| `debug` | Run the agent in the foreground at **debug-level** logging on the console target. Same lifecycle as `run`. |
| `trace` | Run the agent in the foreground at **trace-level** logging on the console target. Same lifecycle as `run`. |
| `run-service` | Run the agent as a Windows service (non-blocking). On non-Windows operating systems the agent logs that the command is unsupported and exits. |
| `install` | Install the agent as a Windows service. Windows only. Stops and removes any existing service of the same name first. |
| `install-start` | Install the agent as a Windows service and immediately start it. Windows only. |
| `start` | Start the previously installed Windows service. Windows only. |
| `stop` | Stop the Windows service. Windows only. |
| `remove` | Stop and remove the Windows service. Windows only. |
| `reset` | Clear the durable observation buffer, the durable asset buffer, and the file-index directory at `durableBufferPath`. Use after a corrupted shutdown or when a fresh-state restart is wanted. |

## Arguments

| Argument | Description |
|---|---|
| `configuration_file` | Path to the agent configuration file. Absolute or relative to the executable's directory. If omitted, the agent looks for `agent.config.json`, then `agent.config.yaml`, in the executable's directory, and copies `agent.config.default.yaml` into place if neither exists. |

## Example invocations

Run in the foreground against an explicit config file:

```bash
./mtconnect.net-agent run /etc/mtconnect/agent.config.yaml
```

Run with debug-level console logging while developing a new module:

```bash
./mtconnect.net-agent debug
```

Install the agent as a Windows service named per `serviceName` in the config and start it immediately:

```powershell
.\mtconnect.net-agent.exe install-start C:\ProgramData\MTConnect\agent.config.yaml
```

Clear the durable buffer (e.g. after schema-incompatible config changes):

```bash
./mtconnect.net-agent reset
```

Run from source against an embedded `agent.config.yaml` during development:

```bash
dotnet run --project agent/MTConnect.NET-Agent -- debug
```

## Configuration file

The agent reads YAML by default (CamelCase keys). JSON is also accepted; when both are present, JSON takes precedence and is converted to YAML on first run. Default filenames live alongside the agent binary:

| File | Role |
|---|---|
| `agent.config.yaml` | The active configuration. |
| `agent.config.json` | Alternative active configuration, JSON format. |
| `agent.config.default.yaml` | Shipped default. Copied to `agent.config.yaml` on first run if neither active file exists. |
| `NLog.config` | NLog configuration. Copied from `NLog.default.config` on first run if missing. |
| `Devices.xml` | Device-information-model file (or directory) loaded into the agent. Path comes from the `devices` key in the config. |

The active configuration is reloaded when `monitorConfigurationFiles: true` is set (the default) and the file is edited at runtime, with a minimum gap of `configurationFileRestartInterval` seconds between restarts.

### Top-level keys

Backed by `AgentApplicationConfiguration` and the inherited `AgentConfiguration` base.

| Key (YAML) | Type | Default | Description |
|---|---|---|---|
| `changeToken` | string | new GUID per save | Internal token regenerated whenever the agent rewrites the file. Surfaces in the Agent device's `mtconnect:ChangeToken` data item so consumers can detect config rewrites. |
| `devices` | string | `null` | Path to a single `Devices.xml` file or to a directory containing one or more `*.xml` device files. Absolute, or relative to the executable. |
| `serviceName` | string | `MTConnect.NET-Agent` | Windows-service short name. Lets multiple agents coexist on one machine. |
| `serviceDisplayName` | string | `MTConnect.NET Agent` | Windows-service display name shown in `services.msc`. |
| `serviceDescription` | string | `MTConnect Agent to provide access to device information using the MTConnect Standard` | Windows-service description text. |
| `serviceAutoStart` | bool | `true` | Windows-service start type. `true` = Automatic, `false` = Manual. |
| `observationBufferSize` | uint | `131072` | Maximum number of observations the in-memory ring buffer retains. The default config shipped in `agent.config.default.yaml` overrides this to `150000`. |
| `assetBufferSize` | uint | `1024` | Maximum number of assets retained. Default config overrides to `1000`. |
| `durable` | bool | `false` | When `true`, observation and asset buffers persist to disk and are reloaded on restart. |
| `durableBufferPath` | string | `null` (defaults to a sub-directory of the executable's dir) | Base directory for the durable buffers when `durable: true`. |
| `useBufferCompression` | bool | `false` | Compress the durable buffers on disk. |
| `monitorConfigurationFiles` | bool | `true` | Restart the agent when `agent.config.yaml`, `NLog.config`, or any device file changes on disk. |
| `configurationFileRestartInterval` | int (seconds) | `2` | Minimum gap between auto-restarts triggered by the file watcher. Prevents thrash when an editor writes multiple times in quick succession. |
| `timezoneOutput` | string (IANA tz name) | `null` (use system local) | TimeZone applied to timestamps emitted by the agent. |
| `ignoreTimestamps` | bool | `false` | When `true`, every incoming observation has its timestamp overwritten with the agent's clock. Trades client-side clock accuracy for clock-drift correction. |
| `defaultVersion` | string (e.g. `"2.5"`) | `MTConnectVersions.Max` (`2.5` in the current build) | MTConnect spec version applied to response documents when the request URL does not pin a version. The shipped default config overrides this to `2.2`. |
| `convertUnits` | bool | `true` | Convert observation values to the units declared on the data item when they arrive in a different compatible unit. |
| `ignoreObservationCase` | bool | `false` | Case-insensitive comparison of incoming observation values to the data item's value-space (CONDITION severities, enum members, etc.). |
| `enableValidation` | bool | `false` | Emit per-observation validation diagnostics on the `agent-validation` logger. |
| `inputValidationLevel` | enum: `Ignore` (`0`), `Warning` (`1`), `Remove` (`2`), `Strict` (`3`) | `Warning` | What the agent does when an observation or asset arrives that fails validation. `Ignore` accepts everything; `Warning` accepts and logs; `Remove` rejects but does not log; `Strict` rejects and logs. |
| `enableAgentDevice` | bool | `true` | Whether the agent emits its own meta-device (`Agent`) on `/probe`, exposing availability and the `mtconnect:ChangeToken` data item. |
| `enableMetrics` | bool | `true` | Emit per-minute observation-rate and asset-update-rate metrics on the `agent-metrics` logger. |
| `modules` | list of single-key maps | `[]` | Each entry instantiates one agent module. See [Modules](/modules/) for the per-module schema. |
| `processors` | list of single-key maps | `[]` | Each entry instantiates one observation-pipeline processor. |

### Module list shape

`modules` is a YAML sequence where each element is a single-key map keyed by the module name. Each module's value object is the module's per-instance configuration; the module catalog documents each schema.

```yaml
modules:
  - http-server:
      port: 5000
      documentFormat: xml
      indentOutput: true
  - mqtt-relay:
      server: localhost
      port: 1883
      durableRelay: true
  - shdr-adapter:
      deviceKey: M12346
      hostname: localhost
      port: 7878
```

Multiple instances of the same module are allowed — each entry produces a distinct module instance keyed by its position in the list. See [Modules](/modules/) for the available module names and their per-module config keys.

### Processor list shape

`processors` follows the same single-key-map-per-entry shape as `modules`. Each processor is a hook into the observation pipeline that runs between the adapter intake and the buffer write. The processor catalog lives under [API reference → MTConnect.Processors](/api/).

### Devices file

The `devices` key points at one `Devices.xml` file or at a directory containing several. The schema is the MTConnect Devices envelope; each device's `<Components>` and `<DataItems>` graph drives what the agent exposes on `/probe`. The agent re-reads the device file when `monitorConfigurationFiles: true` is set. See [Configure & Use → Configure an agent](/configure/agent) for an annotated `Devices.xml` example.

## NLog configuration

Logging is routed through NLog. The shipped `NLog.default.config` defines the following named loggers:

| Logger name | Used for |
|---|---|
| `application-logger` | Application-level events: config load, service install / start / stop, fatal errors. |
| `agent-logger` | Agent-broker events: device registration, observation ingest, response generation. |
| `agent-metrics` | Per-minute observation- and asset-rate metrics (when `enableMetrics: true`). |
| `agent-validation` | Per-observation validation diagnostics (when `enableValidation: true`). |
| `module` | Module-loaded, module-start, module-stop, and module-side log events. |
| `processor` | Processor-loaded, processor-applied, and processor-side log events. |

To raise the console target to `Debug` or `Trace` on a single run without editing `NLog.config`, use the `debug` or `trace` CLI command. To raise it permanently, edit `NLog.config` (the agent will pick the change up automatically when `monitorConfigurationFiles: true`).

## Exit behavior

- `run`, `debug`, `trace` block forever; `Ctrl+C` / SIGINT triggers a graceful shutdown that drains the buffers and runs every module's `Stop()` hook.
- `run-service` returns control to the service controller; lifecycle is managed by Windows.
- `install`, `install-start`, `start`, `stop`, `remove`, `reset`, `help` complete and exit.

## See also

- [Configure & Use → Configure an agent](/configure/agent) — annotated end-to-end `agent.config.yaml` walkthrough.
- [Configure & Use → Run](/configure/run) — local-dev, Docker, Windows-service, and systemd-unit deployment.
- [Modules](/modules/) — per-module configuration schemas referenced by the `modules:` list.
- [API reference → `MTConnect.Applications.MTConnectAgentApplication`](/api/) — the C# class that backs the CLI.
- [API reference → `MTConnect.Configurations.AgentApplicationConfiguration`](/api/) — the configuration POCO the YAML deserializes into.
- [API reference → `MTConnect.Configurations.AgentConfiguration`](/api/) — the base class that holds the buffer / version / validation knobs.
- [Adapter CLI](./adapter) — the sibling adapter host, run separately from the agent.
