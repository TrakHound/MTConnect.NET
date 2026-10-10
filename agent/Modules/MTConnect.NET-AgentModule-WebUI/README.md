![MTConnect.NET Logo](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/logo.png) 

# MTConnect Web UI Agent Module
This Agent Module hosts a browser user interface for the MTConnect.NET Agent. It can be used to:

* View Agent information and buffer state
* Browse Devices / Components and their current Observations (with auto refresh)
* View the recent history of any Data Item
* Browse Assets and view them as MTConnectAssets XML
* View and edit the Agent configuration file (`agent.config.yaml`)

## Configuration
```yaml
modules:
  - web-ui:
      server: localhost
      port: 5080
      allowConfigurationEdit: true
```

* `server` - The hostname to bind to. Defaults to `localhost` so the UI is only reachable from the Agent machine. Set to `*` to listen on all interfaces (on Windows this requires administrator rights or a `netsh http add urlacl` reservation).

* `port` - The port the UI listens on. Default is `5080`.

* `allowConfigurationEdit` - Sets whether the Agent configuration file can be saved from the UI. Default is `true`. When `false` the configuration is shown read-only.

Open `http://localhost:5080/` in a browser once the Agent is running.

The page and its fonts (Roboto and Roboto Mono, licensed under the SIL Open Font License 1.1) are embedded in the module assembly, so the UI makes no requests outside the Agent and works on networks without internet access.

## Applying configuration changes
Saving validates the YAML, keeps a copy of the previous file as `agent.config.yaml.bak`, and writes the new file. When `monitorConfigurationFiles` is enabled (the default) the Agent detects the change and restarts itself to apply it; otherwise restart the Agent manually.

> **Security** - The UI has no authentication. Anyone who can reach it can change the Agent configuration. Keep `server: localhost` or set `allowConfigurationEdit: false` when exposing it on a network.
