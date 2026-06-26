![MTConnect.NET Logo](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/logo.png) 

# MTConnect.NET Adapter

[![MTConnect.NET](https://github.com/TrakHound/MTConnect.NET/actions/workflows/dotnet.yml/badge.svg)](https://github.com/TrakHound/MTConnect.NET/actions/workflows/dotnet.yml)

## Overview
This project is a ready-to-run MTConnect Adapter application. It reads data from a connected device or simulator (implemented in `DataSource.cs`) and forwards that data to an MTConnect Agent using the SHDR protocol or via MQTT, depending on which output modules are configured.

The application shell is thin: `Program.cs` wires a `DataSource` instance to an `MTConnectAdapterApplication` and delegates all lifecycle management—including command-line parsing, Windows Service support, and configuration file monitoring—to the [MTConnect.NET-Applications-Adapter](../MTConnect.NET-Applications-Adapter/) library.

To build your own adapter for a specific machine or controller, replace the `DataSource` class with one that reads from your hardware, then configure the output modules in `adapter.config.yaml`.

## Usage
The adapter can be run from a command-line prompt or installed as a Windows Service:

```
adapter [help|install|install-start|start|stop|remove|debug|run|run-service] [configuration_file]

--------------------

Options :

           help  |  Prints usage information
        install  |  Install as a Service
  install-start  |  Install as a Service and Start the Service
          start  |  Start the Service
           stop  |  Stop the Service
         remove  |  Remove the Service
          debug  |  Runs the Adapter in the terminal (with verbose logging)
            run  |  Runs the Adapter in the terminal
    run-service  |  Runs the Adapter as a Service

Arguments :
--------------------

  configuration_file  |  Specifies the Adapter Configuration file to load
                         Default : adapter.config.yaml
```

#### Example 1:
Run the adapter in the terminal
```
adapter run
```

#### Example 2:
Run the adapter in the terminal with verbose (debug) logging
```
adapter debug
```

#### Example 3:
Install the adapter as a Windows Service (requires Administrator privileges)
```
adapter install
```

#### Example 4:
Install and immediately start the adapter as a Windows Service using a specific configuration file
```
adapter install-start adapter-production.yaml
```

## Configuration
The default configuration file (`adapter.config.yaml`) is shown below:

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

* `id` - A unique identifier for this adapter instance.

* `deviceKey` - The Name or UUID of the Device this adapter reports for.

* `modules` - Output module configurations. Each entry specifies the protocol used to forward observations to an MTConnect Agent.

For the full set of configuration properties supported by each module, see:
- [MTConnect.NET-AdapterModule-SHDR](../Modules/MTConnect.NET-AdapterModule-SHDR/) — SHDR output module
- [MTConnect.NET-AdapterModule-MQTT](../Modules/MTConnect.NET-AdapterModule-MQTT/) — MQTT output module

## Build
```
dotnet publish -c:Release -f:net8.0 -r:win-x64 -p:PublishSingleFile=true --self-contained false
```

## Source Code
This project uses the [MTConnect.NET-Applications-Adapter](../MTConnect.NET-Applications-Adapter/) library to provide the application host. Creating a custom adapter requires only implementing a `DataSource` subclass and configuring the output modules.

## Contribution / Feedback
- Please use the [Issues](https://github.com/TrakHound/MTConnect.NET/issues) tab to create issues for specific problems that you may encounter 
- Please feel free to use the [Pull Requests](https://github.com/TrakHound/MTConnect.NET/pulls) tab for any suggested improvements to the source code
- For any other questions or feedback, please contact TrakHound directly at **info@trakhound.com**.

## License
This application and its source code is licensed under the [MIT License](https://choosealicense.com/licenses/mit/) and is free to use and distribute.
