![MTConnect.NET Logo](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/mtconnect-net-03-md.png) 

# MTConnect.NET-Applications-Adapter
MTConnect.NET-Applications-Adapter contains the base classes and infrastructure needed to build a fully featured MTConnect Adapter application, including command-line argument handling, Windows Service support, configuration file monitoring, and output module management.

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
            <td>MTConnect.NET-Applications-Adapter</td>
            <td><img src="https://img.shields.io/nuget/dt/MTConnect.NET-Applications-Adapter?style=for-the-badge&logo=nuget&label=%20&color=%23333"/></td>
            <td><a href="https://www.nuget.org/packages/MTConnect.NET-Applications-Adapter">https://www.nuget.org/packages/MTConnect.NET-Applications-Adapter</a></td>
        </tr>
    </tbody>
</table>

## Overview
This library makes it straightforward to create a production-quality MTConnect Adapter application. The central class is `MTConnectAdapterApplication`, which handles:

- **Command-line interface** — `run`, `debug`, `install`, `install-start`, `start`, `stop`, `remove`, and `run-service` commands identical to the MTConnect Agent CLI.
- **Windows Service** — installs and runs as a background service on Windows; degrades gracefully on Linux.
- **Configuration management** — reads `adapter.config.yaml` on startup; optional file-system monitoring restarts the adapter automatically when the configuration file changes.
- **Module loading** — discovers and starts all configured output modules (SHDR, MQTT, etc.) and wires each module's outbound queue to a dedicated `MTConnectAdapter` instance.
- **Data source lifecycle** — starts and stops the user-supplied `IMTConnectDataSource` implementation in sync with the adapter modules.
- **Logging** — structured NLog logging with separate log targets for application and per-module output.

## Usage

### Minimal application
```csharp
using MTConnect.Applications;

// 1. Implement your data source
var dataSource = new MyDataSource();

// 2. Create the application host
var app = new MTConnectAdapterApplication(dataSource);

// 3. Run (pass args so CLI commands work)
app.Run(args, isBlocking: true);
```

### Implementing a data source
```csharp
using MTConnect.Input;

public class MyDataSource : MTConnectDataSource
{
    protected override void OnRead()
    {
        // Called on the configured interval; add observations here
        AddObservation("L2estop", "ARMED");
        AddObservation("L2p1execution", "AUTOMATIC");
    }
}
```

## Configuration
The adapter reads `adapter.config.yaml` from its working directory. The full set of supported properties is documented in [MTConnect.NET-Common/Configurations](../../libraries/MTConnect.NET-Common/). Key top-level properties:

* `id` - A unique identifier for this adapter instance.
* `deviceKey` - The Name or UUID of the Device this adapter reports for.
* `writeInterval` - The interval (in milliseconds) at which buffered observations are flushed to the output modules.
* `enableBuffer` - Whether to buffer observations between write intervals.
* `monitorConfigurationFiles` - Whether to watch the configuration file for changes and automatically restart on update.
* `configurationFileRestartInterval` - Minimum time (in seconds) between restarts triggered by configuration file changes.
* `serviceName` - Windows Service name (used when installing or removing the service).
* `serviceAutoStart` - When `true`, the Windows Service starts automatically at boot.
* `modules` - List of output module configurations (SHDR, MQTT, etc.).

## Output Modules
Each entry in the `modules` list activates an output module that forwards observations from the data source to an MTConnect Agent:

- [MTConnect.NET-AdapterModule-SHDR](../Modules/MTConnect.NET-AdapterModule-SHDR/) — SHDR TCP server
- [MTConnect.NET-AdapterModule-MQTT](../Modules/MTConnect.NET-AdapterModule-MQTT/) — MQTT publisher

## Related Libraries
- [MTConnect.NET-Common](../../libraries/MTConnect.NET-Common/) — Core MTConnect entity model
- [MTConnect.NET-Applications-Agents](../../agent/MTConnect.NET-Applications-Agents/) — Symmetrical host library for MTConnect Agent applications

## Contribution / Feedback
- Please use the [Issues](https://github.com/TrakHound/MTConnect.NET/issues) tab to create issues for specific problems that you may encounter 
- Please feel free to use the [Pull Requests](https://github.com/TrakHound/MTConnect.NET/pulls) tab for any suggested improvements to the source code
- For any other questions or feedback, please contact TrakHound directly at **info@trakhound.com**.

## License
This library and its source code is licensed under the [MIT License](https://choosealicense.com/licenses/mit/) and is free to use and distribute.
