# Adapter CLI

`MTConnect-Adapter` is the standalone adapter host. It loads an `adapter.config.yaml` (or `adapter.config.json`) configuration file, builds an `MTConnectAdapter` per configured output module, wires the configured `DataSource` into the input side, and pumps observations downstream to one or more MTConnect agents.

The CLI surface is defined by `MTConnectAdapterApplication.Run(args)` in `adapter/MTConnect.NET-Applications-Adapter/MTConnectAdapterApplication.cs`. The shipped `mtconnect.net-adapter` binary in `adapter/MTConnect.NET-Adapter/` is a thin entry point that constructs a `DataSource` and calls `Run(args, true)` on the application class. Embedders that build their own adapter from `MTConnect.NET-Applications-Adapter` get the same CLI surface.

## Synopsis

```text
mtconnect.net-adapter [help|install|install-start|start|stop|remove|debug|run|run-service] [configuration_file]
```

If no command is given, the adapter runs in `run` mode (foreground, info-level logging). If no configuration file is given, the adapter looks for `adapter.config.yaml` in the executable's directory, copying `adapter.config.default.yaml` into place if it does not exist.

## Commands

| Command | Description |
|---|---|
| `help` | Print usage information and exit. |
| `run` | Run the adapter in the foreground at info-level logging. Default when no command is supplied. Blocks until killed. |
| `debug` | Run the adapter in the foreground at **debug-level** logging on the console target. |
| `run-service` | Run the adapter as a Windows service (non-blocking). On non-Windows operating systems the adapter logs that the command is unsupported and exits. |
| `install` | Install the adapter as a Windows service. Windows only. Stops and removes any existing service of the same name first. |
| `install-start` | Install the adapter as a Windows service and start it immediately. Windows only. |
| `start` | Start the previously installed Windows service. Windows only. |
| `stop` | Stop the Windows service. Windows only. |
| `remove` | Stop and remove the Windows service. Windows only. |

Unlike the [agent CLI](./agent), the adapter does not have a `trace` command and does not have a `reset` command (the adapter does not own a persistent buffer in the way the agent does).

## Arguments

| Argument | Description |
|---|---|
| `configuration_file` | Path to the adapter configuration file. Absolute or relative to the executable's directory. If omitted, the adapter looks for `adapter.config.yaml` in the executable's directory and copies `adapter.config.default.yaml` into place if it does not exist. |

## Example invocations

Run in the foreground against an explicit config file:

```bash
./mtconnect.net-adapter run /etc/mtconnect/adapter.config.yaml
```

Run with debug-level console logging while bringing up a new data source:

```bash
./mtconnect.net-adapter debug
```

Install the adapter as a Windows service and start it:

```powershell
.\mtconnect.net-adapter.exe install-start C:\ProgramData\MTConnect\adapter.config.yaml
```

Run from source against the default config during development:

```bash
dotnet run --project adapter/MTConnect.NET-Adapter -- debug
```

## Configuration file

The adapter reads YAML by default (CamelCase keys). Default filenames live alongside the adapter binary:

| File | Role |
|---|---|
| `adapter.config.yaml` | Active configuration. |
| `adapter.config.json` | Alternative active configuration, JSON format. |
| `adapter.config.default.yaml` | Shipped default. Copied to `adapter.config.yaml` on first run if the active file does not exist. |
| `NLog.config` | NLog configuration shipped alongside the adapter binary. |

The active configuration is reloaded when `monitorConfigurationFiles: true` is set (the default) and the file is edited at runtime, with a minimum gap of `configurationFileRestartInterval` seconds between restarts.

### Top-level keys

Backed by `AdapterApplicationConfiguration` in `libraries/MTConnect.NET-Common/Configurations/`.

| Key (YAML) | Type | Default | Description |
|---|---|---|---|
| `changeToken` | string | new GUID per save | Internal token regenerated when the adapter rewrites the file. |
| `id` | string | a random 6-character string per fresh config | Unique identifier for this adapter instance. Used when the adapter participates in a topology with several siblings. |
| `deviceKey` | string | `null` | Name or UUID of the MTConnect device the adapter is producing observations for. Must match the device's `name=` or `uuid=` in the downstream agent's `Devices.xml`. |
| `readInterval` | int (ms) | `100` | Interval at which the configured `DataSource` is polled for new values. |
| `writeInterval` | int (ms) | `100` | Interval at which queued observations are flushed to the downstream agent. |
| `filterDuplicates` | bool | `true` | When `true`, observations whose value equals the previous observation for the same data item are dropped at the adapter (the agent does the same on its end, but the early filter saves wire bytes). |
| `outputTimestamps` | bool | `true` | When `true`, the adapter prepends a timestamp to every emitted SHDR line. Set to `false` only if the downstream agent is configured with `ignoreTimestamps: true` and you want the agent's clock to win. |
| `enableBuffer` | bool | `false` | When `true`, the adapter queues observations between `writeInterval` ticks rather than sending each one as it arrives. Send-on-flush gives consistent batching at the cost of up to `writeInterval` ms of latency per observation. |
| `serviceName` | string | `null` (falls back to a per-platform default) | Windows-service short name. Lets multiple adapters coexist on one machine. |
| `serviceAutoStart` | bool | `true` | Windows-service start type. `true` = Automatic, `false` = Manual. |
| `monitorConfigurationFiles` | bool | `true` | Restart the adapter when `adapter.config.yaml` changes on disk. |
| `configurationFileRestartInterval` | int (seconds) | `2` | Minimum gap between auto-restarts triggered by the file watcher. |
| `engine` | map | `{}` | Free-form key/value pairs forwarded to the `DataSource` for source-specific configuration (e.g. a serial port name, a host address, an OPC UA endpoint). Accessed by the data source via `GetEngineProperty(name)`. |
| `modules` | list of single-key maps | `[]` | Each entry instantiates one adapter output module. See [Modules](/modules/) for the per-module schema. |

### Module list shape

`modules` is a YAML sequence where each element is a single-key map keyed by the module name. Each module's value object is the module's per-instance configuration.

```yaml
modules:
  - shdr:
      port: 7878
  - mqtt:
      server: localhost
      port: 1883
```

The two adapter output modules shipped today are:

- **`shdr`** — the SHDR socket server. Backed by `MTConnect.NET-AdapterModule-SHDR`. The downstream agent connects on `port` via a `shdr-adapter` module. See [Modules](/modules/) for the full SHDR key set.
- **`mqtt`** — the MQTT publisher. Backed by `MTConnect.NET-AdapterModule-MQTT`. Publishes observations to `server:port` on the configured topic tree. See [Modules](/modules/) for the full MQTT key set.

Multiple instances of the same module are allowed; each entry is a distinct module instance.

### Engine block — data-source-specific settings

The `engine` map is free-form. Whatever keys you write there become accessible to the `DataSource` implementation via `configuration.GetEngineProperty("keyName")`. The shipped `DataSource` in `adapter/MTConnect.NET-Adapter/DataSource.cs` is a stub — embedders override it. Examples:

```yaml
engine:
  serialPort: COM3
  baudRate: 115200
  parity: None
```

```yaml
engine:
  opcUaEndpoint: opc.tcp://192.168.1.50:4840
  username: mtconnect
```

## Sample shipped default

The default config shipped at `adapter/MTConnect.NET-Adapter/adapter.config.default.yaml` wires an SHDR module on port `7878` and an MQTT module on `localhost:1883` for the device `OKUMA-Lathe`:

```yaml
id: PatrickAdapter
deviceKey: OKUMA-Lathe

modules:
  - shdr:
      port: 7878
  - mqtt:
      server: localhost
      port: 1883
```

Edit `deviceKey`, the SHDR `port`, and the MQTT endpoint to match your topology before pointing the adapter at a real data source.

## NLog configuration

Logging is routed through NLog. The shipped `NLog.config` defines the following named loggers:

| Logger name | Used for |
|---|---|
| `application-logger` | Application-level events: config load, service install / start / stop, fatal errors. |
| `adapter-logger` | Adapter-pump events: data-source reads, observation flushes. |
| `module` | Module-loaded, module-start, module-stop, and module-side log events. |

To raise the console target to debug-level on a single run, use the `debug` command. To raise it permanently, edit `NLog.config`.

## Exit behavior

- `run`, `debug` block forever; `Ctrl+C` / SIGINT triggers a graceful shutdown that flushes pending observations and runs every module's `Stop()` hook.
- `run-service` returns control to the service controller; lifecycle is managed by Windows.
- `install`, `install-start`, `start`, `stop`, `remove`, `help` complete and exit.

## See also

- [Configure & Use → Configure an adapter](/configure/adapter) — annotated end-to-end `adapter.config.yaml` walkthrough plus data-source bring-up.
- [Configure & Use → Run](/configure/run) — local-dev, Windows-service, and systemd-unit deployment.
- [Modules](/modules/) — per-module configuration schemas referenced by the `modules:` list.
- [API reference → `MTConnect.Applications.MTConnectAdapterApplication`](/api/) — the C# class that backs the CLI.
- [API reference → `MTConnect.Configurations.AdapterApplicationConfiguration`](/api/) — the configuration POCO the YAML deserializes into.
- [API reference → `MTConnect.Adapters.MTConnectAdapter`](/api/) — the inner adapter type that orchestrates `WriteObservations` / `WriteAssets` / `WriteDevices`.
- [Agent CLI](./agent) — the sibling agent host that consumes the adapter's output.
