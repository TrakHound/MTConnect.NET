# Run

This page covers how to run a configured agent or adapter — the CLI verbs the shipped executables accept, the deployment shapes (local terminal, Docker, Windows service, systemd unit), the configuration-file resolution order, and the first-boot signals that confirm a healthy start.

The previous pages — [Install](./install), [Configure an agent](./agent-config), [Configure an adapter](./adapter-config) — assume the binaries are installed and the configuration files are authored. This page picks up at "config is ready; how do I start the process?"

## CLI verbs

The shipped agent (`MTConnect.NET-Agent`) and adapter (`MTConnect.NET-Adapter`) accept the same command set. Each verb maps to a behavior the underlying [`MTConnectAgentApplication`](/api/MTConnect.Applications.MTConnectAgentApplication) (or the adapter equivalent) implements.

| Verb | Behavior |
|---|---|
| `run` | Foreground execution with the standard log levels. The default when no verb is passed. |
| `debug` | Foreground execution with the console log level raised to `Debug` for the duration of the run. |
| `trace` | Foreground execution with the console log level raised to `Trace`. The noisiest setting; use for diagnosing a single misbehaving module. |
| `install` | Install the executable as a Windows service. The service name, display name, and description come from the agent / adapter configuration. |
| `install-start` | Install the service and immediately start it. |
| `remove` | Stop and uninstall the Windows service. |
| `start` | Start a previously-installed service. |
| `stop` | Stop a running service. |
| `reset` | Erase the durable buffer and the cached observation state, then exit. Useful when a corrupted buffer is preventing startup. |
| `help` | Print the verb list and exit. |

The Windows-service verbs are no-ops on Linux and macOS; use `systemd` (or `launchd`) to manage the lifecycle there. See [Operate](./operate) for a sample systemd unit and a sample Windows service descriptor.

## Configuration-file resolution

The agent reads `agent.config.yaml` from these locations in order, stopping at the first hit:

1. The path passed as a positional CLI argument after the verb — `MTConnect.NET-Agent run /etc/mtconnect/agent.config.yaml`.
2. `agent.config.yaml` next to the executable.
3. `agent.config.default.yaml` next to the executable (the shipped fallback). The agent emits a warning when it falls through to the default file so the operator knows the deployment-specific config wasn't found.

The adapter resolves `adapter.config.yaml` against the same three locations.

The Docker image at `trakhound/mtconnect.net-agent` sets `WORKDIR /app` and uses `ENTRYPOINT ["dotnet", "agent.dll"]` with a default `CMD ["debug"]`. `agent.config.yaml` is loaded from the `/app` working directory, so a deployment that wants to override the shipped default bind-mounts its own `agent.config.yaml` over `/app/agent.config.yaml`.

## Local development

```sh
# From a source clone.
dotnet run --project agent/MTConnect.NET-Agent/MTConnect.NET-Agent.csproj

# From an installed package.
MTConnect.NET-Agent run
```

The foreground process logs to stdout. A clean startup ends with the HTTP server module's `Listening at` line:

```
modules.http-server | Info | Listening at http://*:5000/..
```

The agent is then accepting HTTP requests on the configured port (default `5000` from the shipped `agent.config.default.yaml`).

For a quick sanity check:

```sh
curl -s http://localhost:5000/probe | head -40
```

A `<MTConnectDevices>` envelope listing every device the agent loaded confirms end-to-end startup.

## Docker

```sh
docker run --rm -d --name mtc-agent \
  -p 5000:5000 \
  -v "$PWD/agent.config.yaml:/app/agent.config.yaml:ro" \
  -v "$PWD/devices:/app/devices:ro" \
  trakhound/mtconnect.net-agent:latest
```

Volumes:

- `/app/agent.config.yaml` — the agent's main configuration file.
- `/app/devices` — the directory the `devices:` key in `agent.config.yaml` points at. Mount as read-only for production deployments; mount read-write only when `monitorConfigurationFiles: true` and the device files must be editable at runtime.
- `/app/buffer` — durable-buffer storage. Mount only when `durable: true` and the buffer must survive container restarts.

The default container exposes port `5000` and runs the `run` verb. Override with `--entrypoint MTConnect.NET-Agent` and a fresh `CMD` to switch to `debug` or `trace` while diagnosing.

The example above publishes port `5000` without TLS or authentication. For anything beyond a trusted local network, terminate TLS in front of the container (reverse proxy, Kubernetes Ingress, etc.) or enable the HTTP server module's TLS + auth blocks — see [Module configuration: HTTP server](/configure/module-config#http-server).

## Windows service

```cmd
MTConnect.NET-Agent install-start
```

The verb creates a Windows service named `MTConnect.NET-Agent` (overridable via the `serviceName:` key in `agent.config.yaml`), sets the start-type to `Automatic`, and starts the service. The service runs under the `LocalSystem` account by default; an explicit account can be configured by editing the service after install (`sc config MTConnect.NET-Agent obj=<account>`). `LocalSystem` is convenient for evaluation; a dedicated low-privilege service account is preferred for production deployments.

To stop or uninstall:

```cmd
MTConnect.NET-Agent stop
MTConnect.NET-Agent remove
```

The service's standard out + error are routed to the Windows event log under the `Application` source matching the service name. Detailed diagnostics still go to `NLog.config`'s file targets — by default the `logs/` directory next to the executable.

## systemd unit

A minimal unit for the agent:

```ini
[Unit]
Description=MTConnect.NET Agent
After=network-online.target
Wants=network-online.target

[Service]
Type=simple
WorkingDirectory=/opt/mtconnect/agent
ExecStart=/opt/mtconnect/agent/MTConnect.NET-Agent run /opt/mtconnect/agent/agent.config.yaml
Restart=on-failure
RestartSec=5
User=mtconnect
Group=mtconnect

[Install]
WantedBy=multi-user.target
```

Place at `/etc/systemd/system/mtconnect-agent.service`, then:

```sh
systemctl daemon-reload
systemctl enable --now mtconnect-agent
journalctl -u mtconnect-agent -f
```

`Restart=on-failure` covers the cases where the agent exits non-zero (a fatal config-load error, an unrecoverable module failure). For a clean shutdown the agent always exits zero.

The adapter is identical with `MTConnect.NET-Agent` replaced by `MTConnect.NET-Adapter` and the `agent.config.yaml` path swapped for `adapter.config.yaml`.

## First-boot troubleshooting

Symptoms the operator sees first, and where to look:

- **No `Listening at` line** — the configuration-load path failed. Look earlier in the log for `Failed to load configuration` or `Error parsing YAML`. A malformed `agent.config.yaml` aborts startup before the HTTP server binds.
- **`Address already in use` on the agent's port** — another process is holding the port. The default is `5000`; reconfigure with `http-server: port:` in `agent.config.yaml`, or stop the conflicting process.
- **`/probe` returns empty `<Devices/>`** — the `devices:` path resolved, but no `.xml` files parsed. Check the directory contents and look for `Failed to load device` messages. Common cause: an XSD-invalid `Devices.xml` (see [Troubleshooting: XSD validation failures](/troubleshooting/xsd-validation-failures)).
- **Modules listed in `agent.config.yaml` not appearing in `/probe`** — the module-load path failed. Look for `Module load failed` lines naming the module key. Common causes: a missing `.dll` for an out-of-tree module, or a `tls:` block with an unreadable certificate path.
- **Agent starts but logs nothing** — `NLog.config` was overridden or moved. The shipped `NLog.default.config` writes to `logs/<level>.log` next to the executable.

For ongoing operational signals (logs, metrics, health) see [Operate](./operate). For consumer integration patterns (HTTP polling, MQTT subscriptions) see [Connect a consumer](./consumer).

## See also

- [Configure an agent](./agent-config) — the `agent.config.yaml` schema reference.
- [Configure an adapter](./adapter-config) — the `adapter.config.yaml` schema reference.
- [Connect a consumer](./consumer) — the consumer side of a running agent.
- [Operate](./operate) — logs, metrics, restarts, durable storage.
- [CLI: Agent](/cli/agent) — the full agent CLI reference including verbs not surfaced above.
- [CLI: Adapter](/cli/adapter) — the adapter CLI counterpart.
