![MTConnect.NET Logo](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/dev/img/mtconnect-net-03-md.png) 

# MTConnect SHDR Adapter

[![MTConnect.NET](https://github.com/TrakHound/MTConnect.NET/actions/workflows/dotnet.yml/badge.svg)](https://github.com/TrakHound/MTConnect.NET/actions/workflows/dotnet.yml)

## Overview
This project is a full implementation of an MTConnect SHDR Adapter used to send data from a PLC (or other Data Source) to an MTConnect Agent.
This project is designed to be used by a developer to serve as a "shell" application for an Adapter solution. 
Contained in this project is the functionality to:
- Send data using the SHDR Protocol
- Run / Install / Remove as a Windows Service
- Read and detect changes to a customizable configuration file
- Tools to build as a Windows Installer exe

## Develop
To develop an Adapter using this project, edit the [AdpaterEngine.cs](https://github.com/TrakHound/MTConnect.NET/tree/master/applications/Adapters/MTConnect-Adapter-SHDR/AdapterEngine.cs) file so that it follows the pattern below:

```c#
class AdapterEngine : MTConnectShdrAdapterEngine<AdapterConfiguration>
{
    protected readonly Logger _engineLogger = LogManager.GetLogger("engine-logger");
    private PlcSimulator _dataSource;


    protected override void OnStart()
    {
        _engineLogger.Info($"Connected to PLC @ {Configuration.PlcAddress} : Port = {Configuration.PlcPort}");

        _dataSource = new PlcSimulator(Configuration.PlcAddress, Configuration.PlcPort, 50);
        _dataSource.Connect();
    }

    protected override void OnStop()
    {
        Adapter.SetUnavailable();

        _dataSource.Disconnect();

        _engineLogger.Info($"Disconnected from PLC @ {Configuration.PlcAddress} : Port = {Configuration.PlcPort}");
    }

    protected override void OnRead()
    {
        // Using a single Timestamp (per OnRead() call) can consolidate the SHDR output as well as make MTConnect data more "aligned" and easier to process
        var ts = UnixDateTime.Now;

        Adapter.AddDataItem("avail", _dataSource.Connected ? Availability.AVAILABLE : Availability.UNAVAILABLE, ts);

        Adapter.AddDataItem("estop", _dataSource.EmergencyStop ? EmergencyStop.ARMED : EmergencyStop.TRIGGERED, ts);

        switch (_dataSource.Mode)
        {
            case 0: Adapter.AddDataItem("mode", ControllerMode.MANUAL, ts); break;
            case 1: Adapter.AddDataItem("mode", ControllerMode.SEMI_AUTOMATIC, ts); break;
            case 2: Adapter.AddDataItem("mode", ControllerMode.AUTOMATIC, ts); break;
            case 3: Adapter.AddDataItem("mode", ControllerMode.EDIT, ts); break;
        }

        Adapter.AddDataItem("program", _dataSource.ProcessDatas[0].Program, ts);
        Adapter.AddDataItem("tool", _dataSource.ProcessDatas[0].ToolNumber, ts);
        Adapter.AddDataItem("tool_offset", _dataSource.ProcessDatas[0].ToolOffset, ts);

        for (var i = 0; i < _dataSource.AxisDatas.Length; i++)
        {
            var axis = _dataSource.AxisDatas[i];
            Adapter.AddDataItem($"axis_{i}_pos", axis.MachinePosition, ts);
        }

        Adapter.AddAsset(Examples.CuttingTool());


        // The Adapter (ShdrAdapter) handles sending the data to the MTConnect Agent using the SHDR Protocol
    }
```

### OnStart()
The OnStart() method is used to contain code to allow a developer to call a method to connect to the PLC or data source.

### OnStop()
The OnStop() method is used to contain code to allow a developer to call a method to disconnect from the PLC or data source.

### OnRead()
The OnRead() method is used to contain code to allow a developer to add data to the underlying **Adapter** property. 
The OnRead() method runs at the **Interval** specified in the configuration file.
This method is run on a separate worker thread than the underlying ShdrAdapter. The thread is exited when the AdapterEngine is stopped.

### OnReadAsync()
The OnReadAsync() method has the same functionality of OnRead() but runs as an Async method.

## Usage
The Adapter can be run from a command line prompt or as a Windows Service using the format below:
```
adapter [help|install|install-start|start|stop|remove|debug|run|run-service] [configuration_file] [tcp_port]

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
                         Default : agent.config.json

           tcp_port  |  Specifies the TCP Port to use for the SHDR communication
                         Note : This overrides what is read from the Configuration file
```
#### Example 1:
Install the Adapter as a Windows Service (Note: requires Administrator Privileges)
 > adapter install

#### Example 2:
Install the Adapter as a Windows Service using the configuration file "adapter-config.yaml" and Starts the service (Note: requires Administrator Privileges)
> adapter install-start agent-config.json

#### Example 3:
Starts the Windows Service (Note: requires Administrator Privileges)
> adapter start

#### Example 4:
Runs the Adapter in the command line prompt using verbose logging and overrides the TCP Port to 7579
> adapter debug "" 7579

## Configuration
More information about [Configurations](https://github.com/TrakHound/MTConnect.NET/tree/master/src/MTConnect.NET-Common/Configurations). The default configuration file is shown below :
```yaml
id: CustomAdapter
deviceKey: OKUMA-Lathe
heartbeat: 10000
timeout: 5000
readInterval: 50
writeInterval: 100
outputTimestamps: true
enableBuffer: false
filterDuplicates: true
plcAddress: pallet_pool  # Customizable Property
plcPort: 7679            # Customizable Property
```


## Logging
Logging is done using [NLog](https://github.com/NLog/NLog) which allows for customized logging through the NLog.config file located in the application's install directory. The loggers are setup so that there is a separate logger for:
- **(adapter-logger)** MTConnect Adapter

The default [NLog Configuration File](https://github.com/TrakHound/MTConnect.NET/blob/master/src/MTConnect.NET-Applications-Adapter-SHDR/NLog.config) is shown below:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">

	<!-- the targets to write to -->
	<targets>

		<!--Console-->
		<target name="logconsole" xsi:type="Console" />
		
		<!--Adapter Log File-->
		<target xsi:type="File" name="adapter-file" fileName="logs\adapter-${shortdate}.log"
			layout="${longdate}|${event-properties:item=EventId_Id:whenEmpty=0}|${uppercase:${level}}|${logger}|${message} ${exception:format=tostring}" />

	</targets>

	<!-- rules to map from logger name to target -->
	<rules>

		<!--Write to Console-->
		<logger name="*" minlevel="Trace" writeTo="logconsole" />

		<!--Adapter Logger-->
		<logger name="adapter-logger" minlevel="Trace" writeTo="adapter-file" final="true" />
		
		<!--Skip non-critical Microsoft logs and so log only own logs (BlackHole) -->
		<logger name="Microsoft.*" maxlevel="Info" final="true" />
		<logger name="System.Net.Http.*" maxlevel="Info" final="true" />

	</rules>
</nlog>
```

## Releases
Releases for this application are located under the Releases tab. The current release is listed below:
- [MTConnect Agent Current Release](https://github.com/TrakHound/MTConnect.NET/releases)

## Source Code
This project uses the MTConnect.NET-Applications-Adapters-SHDR library (available on [Nuget](https://www.nuget.org/packages/MTConnect.NET-Applications-Adapter-SHDR)) to create an MTConnect SHDR Adapter application. 

## Contribution / Feedback
- Please use the [Issues](https://github.com/TrakHound/MTConnect.NET/issues) tab to create issues for specific problems that you may encounter 
- Please feel free to use the [Pull Requests](https://github.com/TrakHound/MTConnect.NET/pulls) tab for any suggested improvements to the source code
- For any other questions or feedback, please contact TrakHound directly at **info@trakhound.com**.

## License
This application and it's source code is licensed under the [MIT License](https://choosealicense.com/licenses/mit/) and is free to use.
