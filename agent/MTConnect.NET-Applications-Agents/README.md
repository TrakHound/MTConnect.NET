![MTConnect.NET Logo](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/mtconnect-net-03-md.png) 

# MTConnect.NET-Applications-Agents
MTConnect.NET-Applications-Agents contains the base classes and infrastructure needed to build a fully featured MTConnect Agent application, including command-line argument handling, Windows Service support, device and buffer management, configuration file monitoring, and agent module loading.

## Nuget
<table>
    <thead>
        <tr>
            <td style="font-weight: bold;">Package Name</td>
            <td style="font-weight: bold;">Downloads</td>
            <td style="font-weight: bold;">Link</td>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td>MTConnect.NET-Applications-Agents</td>
            <td><img src="https://img.shields.io/nuget/dt/MTConnect.NET-Applications-Agents?style=for-the-badge&logo=nuget&label=%20&color=%23333"/></td>
            <td><a href="https://www.nuget.org/packages/MTConnect.NET-Applications-Agents">https://www.nuget.org/packages/MTConnect.NET-Applications-Agents</a></td>
        </tr>
    </tbody>
</table>

## Overview
This library makes it straightforward to create a production-quality MTConnect Agent application. The central class is `MTConnectAgentApplication`, which handles:

- **Command-line interface** — `run`, `debug`, `install`, `install-start`, `start`, `stop`, `remove`, and `run-service` commands (the same surface as the standalone MTConnect Agent binary).
- **Windows Service** — installs and runs as a background service on Windows; degrades gracefully on Linux.
- **Agent broker** — creates and manages the internal `MTConnectAgentBroker` that holds the observation and asset buffers.
- **Device management** — loads device model files from the configured path (a single file or a directory of per-device files) and registers a file-system watcher for live reloads.
- **Configuration management** — reads `agent.config.yaml` on startup; optional file-system monitoring restarts the agent automatically when the configuration file changes.
- **Module loading** — discovers and starts all configured agent modules (HTTP server, MQTT relay/broker, SHDR adapter, MQTT adapter, HTTP adapter, etc.).
- **Processor loading** — discovers and starts all configured agent processors (e.g., Python script transformation).
- **Durable buffers** — optional file-backed observation and asset buffers that survive application restarts.
- **Logging** — structured NLog logging with separate log targets for application, agent, validation, module, and processor output.

## Usage

### Minimal application
```csharp
using MTConnect.Applications;

// Create and run an agent application using the default MTConnectAgentApplication
var app = new MTConnectAgentApplication();

// Run (pass args so CLI commands work; true = block until Ctrl+C)
app.Run(args, isBlocking: true);
```

### Custom application with extended startup logic
```csharp
using MTConnect.Applications;
using MTConnect.Configurations;

public class MyAgentApplication : MTConnectAgentApplication
{
    protected override void OnAgentStarted(IAgentApplicationConfiguration configuration)
    {
        // Custom initialization after the agent broker and modules are running
        _applicationLogger.Info("Custom agent started successfully.");
    }
}
```

## Configuration
The agent reads `agent.config.yaml` from its working directory. The default configuration is shown below:

```yaml
# - Modules -
modules:

- http-server: # - Add HTTP Server module
    port: 5000
    indentOutput: true
    documentFormat: xml
    accept:
      text/xml: xml
      application/json: json
    responseCompression:
    - gzip
    - br

# The maximum number of Observations the agent can hold in its buffer
observationBufferSize: 150000

# The maximum number of assets the agent can hold in its buffer
assetBufferSize: 1000

# Sets whether the Agent buffers are durable and retain state after restart
durable: false

# Sets the default MTConnect version to output response documents for.
defaultVersion: 2.2
```

Key configuration properties:

* `devices` - Path to the Device Information Model file(s) to load. May be a single file or a directory. Defaults to a `devices` subdirectory.
* `observationBufferSize` - Maximum number of observations the agent holds in its ring buffer.
* `assetBufferSize` - Maximum number of assets held in the asset buffer.
* `durable` - When `true`, buffers are written to disk and survive application restarts.
* `defaultVersion` - The MTConnect version used for response documents when the client does not specify one.
* `monitorConfigurationFiles` - When `true`, configuration file changes trigger an automatic agent restart.
* `modules` - List of agent module configurations.
* `processors` - List of agent processor configurations.
* `serviceName` - Windows Service name used when installing or removing the service.
* `serviceAutoStart` - When `true`, the Windows Service starts automatically at boot.

## Agent Modules
Each entry in the `modules` list activates an agent module. The bundled modules are:

- [MTConnect.NET-AgentModule-HttpServer](../Modules/MTConnect.NET-AgentModule-HttpServer/) — MTConnect REST HTTP server
- [MTConnect.NET-AgentModule-MqttRelay](../Modules/MTConnect.NET-AgentModule-MqttRelay/) — MQTT relay to an external broker
- [MTConnect.NET-AgentModule-MqttBroker](../Modules/MTConnect.NET-AgentModule-MqttBroker/) — embedded MQTT broker
- [MTConnect.NET-AgentModule-ShdrAdapter](../Modules/MTConnect.NET-AgentModule-ShdrAdapter/) — SHDR adapter input
- [MTConnect.NET-AgentModule-MqttAdapter](../Modules/MTConnect.NET-AgentModule-MqttAdapter/) — MQTT adapter input
- [MTConnect.NET-AgentModule-HttpAdapter](../Modules/MTConnect.NET-AgentModule-HttpAdapter/) — HTTP adapter input

## Logging
Logging uses [NLog](https://github.com/NLog/NLog). The default configuration file creates separate log files for:
- **(application-logger)** — application startup, shutdown, and configuration load
- **(agent-logger)** — agent broker events (device add, observation enqueue)
- **(agent-validation-logger)** — input validation errors and type mismatches
- **(module-logger)** — per-module events
- **(processor-logger)** — per-processor events

## Related Libraries
- [MTConnect.NET-Common](../../libraries/MTConnect.NET-Common/) — Core MTConnect entity model
- [MTConnect.NET-Applications-Adapter](../../adapter/MTConnect.NET-Applications-Adapter/) — Symmetrical host library for MTConnect Adapter applications

## Contribution / Feedback
- Please use the [Issues](https://github.com/TrakHound/MTConnect.NET/issues) tab to create issues for specific problems that you may encounter 
- Please feel free to use the [Pull Requests](https://github.com/TrakHound/MTConnect.NET/pulls) tab for any suggested improvements to the source code
- For any other questions or feedback, please contact TrakHound directly at **info@trakhound.com**.

## License
This library and its source code is licensed under the [MIT License](https://choosealicense.com/licenses/mit/) and is free to use and distribute.
