# Operate

This page covers the operational surface of a running agent or adapter — logs (where they land, what each logger reports), metrics (observation rate, asset count), health checks, reload + restart semantics, and durable-buffer handling. It is the page an on-call operator opens at 03:00 when something is unhappy.

The previous page, [Connect a consumer](./consumer), covers the data plane. This page covers the control plane — everything the operator (not the consumer) needs to know.

## Logs

Logging is provided by NLog. The shipped `NLog.config` next to the agent / adapter executable defines five named loggers, each writing to its own daily-rolled file under `logs/`:

| Logger | Source | File pattern |
|---|---|---|
| `application-logger` | Top-level lifecycle (startup, shutdown, configuration load). | `logs/application-<YYYY-MM-DD>.log` |
| `agent-logger` | Agent broker internals (device add, observation enqueue, buffer events). | `logs/agent-<YYYY-MM-DD>.log` |
| `agent-validation` | Input validation issues (rejected observations, type mismatches). | `logs/agent-validation-<YYYY-MM-DD>.log` |
| `agent-metrics` | Periodic agent-metrics emitter (observation rate, asset rate). | Routed to its own file when configured; otherwise rolls into `agent-`. |
| `module` / per-module loggers | One file per loaded module (the logger name is the module key — `mqtt-relay`, `shdr-adapter`, etc.). | `logs/<module-name>-<YYYY-MM-DD>.log` |
| `processor` / per-processor loggers | One file per loaded processor (e.g. `agent-processor-python`). | `logs/<processor-name>-<YYYY-MM-DD>.log` |

Every file rolls daily and keeps 90 archives by default. Edit `NLog.config` to raise the archive count, change the layout, or add a Syslog / TCP / cloud target — the shipped configuration is intended as a working starting point, not a frozen contract.

### Raising the console log level

The `debug` and `trace` CLI verbs (see [Run](./run)) raise the console-target log level on a single foreground run without editing `NLog.config`. For a service deployment, edit `NLog.config` directly — the agent re-reads it without a restart when `internalLoggingLevel` is set to `Trace`.

### NLog at a glance

The shipped file targets use the layout:

```
${longdate}|${event-properties:item=EventId_Id:whenEmpty=0}|${uppercase:${level}}|${logger}|${message} ${exception:format=tostring}
```

Pipe-separated; columns are time, event-ID, level, logger, message + optional exception. The format ingests cleanly into typical log-aggregation pipelines (Elastic / Loki / Datadog) — point a parser at the `logs/` directory and parse on the pipe character.

## Metrics

The agent runs a built-in metrics timer (see [`MTConnectAgentMetrics`](/api/MTConnect.Agents.Metrics.MTConnectAgentMetrics)). On each tick it reads the current observation count, the current asset count, the rolling-window averages, and emits debug-level lines through the `agent-metrics` logger:

```
agent-metrics | Debug | Observations - Delta for last 10 seconds: 423
agent-metrics | Debug | Observations - Average for last 5 minutes: 41.78
agent-metrics | Debug | Assets - Delta for last 10 seconds: 0
agent-metrics | Debug | Assets - Average for last 5 minutes: 0.13
```

The tick interval (`updateInterval`) and the window length (`windowInterval`) come from the agent's `metrics:` block in `agent.config.yaml`:

```yaml
metrics:
  updateInterval: 10        # seconds between emit ticks
  windowInterval: 5         # minutes for the rolling average
```

For machine-readable metrics, query the agent's `/probe` self-describing Agent Device when `enableAgentDevice: true` is set. The Agent Device emits `AVAILABILITY`, `ASSET_CHANGED`, `ASSET_REMOVED`, `MTCONNECT_VERSION`, and a battery of internal observations. Probes / current / sample work on the Agent Device the same way as any other device:

```sh
curl -s 'http://agent.local:5000/Agent/current'
```

## Health checks

For load-balancer health probes, the simplest signal is an HTTP `GET /probe` on the agent's port. A `200 OK` with a `<MTConnectDevices>` body means the HTTP server is up and the agent loaded at least one device. A `503 Service Unavailable` with an `<MTConnectError>` envelope means the agent is running but in an error state.

A liveness-only probe (server is up, but the buffer / device-model state is unverified) can hit the agent's static-root handler:

```sh
curl -s -o /dev/null -w '%{http_code}' http://agent.local:5000/
```

A `200` confirms the HTTP server is responding without exercising any device-model lookups.

## Reload + restart

The agent supports two restart shapes:

1. **Soft reload** — set `monitorConfigurationFiles: true` in `agent.config.yaml`. The agent watches `agent.config.yaml` and every `Devices.xml` file under `devices:` and reloads automatically when any of them changes (debounced by `configurationFileRestartInterval`, default 2 seconds). The HTTP server keeps its port; existing long-poll connections are gracefully closed and reconnect from the consumer side.
2. **Hard restart** — the OS-level mechanism: `systemctl restart mtconnect-agent` on Linux; `sc stop / sc start MTConnect.NET-Agent` (or the bundled `MTConnect.NET-Agent stop` + `start` verbs) on Windows.

The soft reload preserves the in-memory buffer; the hard restart loses it unless `durable: true` is set (see below).

When `monitorConfigurationFiles: true` is enabled, an edit to `Devices.xml` triggers a [`DeviceConfigurationFileWatcher`](/api/MTConnect.Configurations.DeviceConfigurationFileWatcher) event; the agent re-parses the file, validates the result against the loaded XSD, and atomically swaps the device tree. A parse failure leaves the previous device tree in place and surfaces the error through `application-logger`.

## Durable buffer

When `durable: true` is set, the agent writes its observation buffer pages to `durableBufferPath` (default `buffer/` next to the executable). On startup the agent restores the buffer, preserves the `instanceId`, and resumes from the last persisted `nextSequence`. A consumer paginating across a restart sees a continuous sequence-cursor rather than a reset.

Operational notes:

- The buffer directory grows with `observationBufferSize` * the per-observation page size; provision disk accordingly. The shipped default of `observationBufferSize: 150000` typically occupies tens of megabytes.
- A corrupted buffer (truncated page, disk-full mid-write) prevents startup. The `reset` CLI verb (see [Run](./run)) wipes the buffer and clears the instance state so the agent can boot fresh.
- The buffer is not backed up to off-machine storage by default. For deployments where buffer survival across machine failure matters, snapshot the `buffer/` directory through the usual filesystem-snapshot machinery (LVM, ZFS, EBS snapshots).

## Common operational patterns

- **Tail the agent log in production**: `tail -F logs/agent-$(date +%F).log` (Linux) or `Get-Content -Tail 50 -Wait logs\agent-(Get-Date -Format yyyy-MM-dd).log` (PowerShell).
- **Tail validation rejections only**: `tail -F logs/agent-validation-$(date +%F).log`. A spike here signals a misconfigured adapter or a Devices.xml drift.
- **Diagnose a single module without restarting**: enable the module's debug level by editing `NLog.config`'s per-module rule. NLog reloads the config automatically.
- **Test the agent's reachability from inside a Docker network**: `docker exec mtc-agent curl -sf http://localhost:5000/probe > /dev/null`. A non-zero exit means the in-container HTTP server is down even if the host-side port mapping appears healthy.

## See also

- [Run](./run) — starting and stopping the agent.
- [Connect a consumer](./consumer) — the consumer side of the running agent.
- [Configure an agent](./agent-config) — every `agent.config.yaml` key including the operational ones (`durable`, `metrics`, `monitorConfigurationFiles`).
- [Troubleshooting overview](/troubleshooting/) — diagnosis playbooks for the common failure modes.
- [Concepts: Agent validation events](/concepts/agent-validation-events) — the in-process event family the `agent-validation` logger writes from.
