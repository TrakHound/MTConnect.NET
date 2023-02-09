![MTConnect.NET Logo](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/dev/img/mtconnect-net-03-md.png) 

# MTConnect MQTT Relay Agent

[![MTConnect.NET](https://github.com/TrakHound/MTConnect.NET/actions/workflows/dotnet.yml/badge.svg)](https://github.com/TrakHound/MTConnect.NET/actions/workflows/dotnet.yml)

## Overview
This project is a full implementation of an MTConnect Agent used to read data from industrial machine tools and devices. This MTConnect Agent application is fully compatible with the latest **Version 2.1 of the MTConnect Standard**. It uses the SHDR protocol to receive data from Adapters, an in-memory buffer with an optional durable file system based buffer, and an MQTT client to publish messages to an external MQTT Broker.

#### Features
- MQTT support. [Learn More](https://github.com/TrakHound/MTConnect.NET/tree/master/src/MTConnect.NET-MQTT)
- Easy setup with Windows Installers availble in the latest [Releases](https://github.com/TrakHound/MTConnect.NET/releases)
- Options to run as Windows Service or as a console application (typically for testing/debugging)
- Optional 'Durable' buffer used to retain the Agent data between application/machine restarts
- High performance / Low resource usage
- Flexible Device configurations (traditional 'devices.xml' file or 'devices' directory with individual Device files)
- SHDR protocol compatibility for easy implementation with existing MTConnect Adapters
- Configuration File monitoring to automatically restart the Agent upon configuration file changes
- Flexible Logging using NLog which can be used to output log information to separate files for easier analysis


![MQTT Agent Architecture](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/mtconnect-agent-mqtt-shdr-communication.png#gh-light-mode-only) 
![MQTT Agent Architecture](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/mtconnect-agent-mqtt-shdr-communication-dark.png#gh-dark-mode-only) 

## Download
To download the latest release as a Windows Installer, use the link below:

- [Download Latest Release Windows Installer](https://github.com/TrakHound/MTConnect.NET/releases/download/v5.0.0/TrakHound-MTConnect-MQTT-Relay-Agent-Install-v5.0.0.exe)


## Usage
The Agent can be run from a command line prompt or as a Windows Service using the format below:
```
agent [help|install|install-start|start|stop|remove|debug|run|run-service] [configuration_file] [mqtt_port]

--------------------

Options :

           help  |  Prints usage information
        install  |  Install as a Service
  install-start  |  Install as a Service and Start the Service
          start  |  Start the Service
           stop  |  Stop the Service
         remove  |  Remove the Service
          debug  |  Runs the Agent in the terminal (with verbose logging)
            run  |  Runs the Agent in the terminal
    run-service  |  Runs the Agent as a Service

Arguments :
--------------------

  configuration_file  |  Specifies the Agent Configuration file to load
                         Default : agent.config.json

           mqtt_port  |  Specifies the TCP Port to use for the MQTT client
                         Note : This overrides what is read from the Configuration file
```
#### Example 1:
Install the Agent as a Windows Service (Note: requires Administrator Privileges)
 > agent install

#### Example 2:
Install the Agent as a Windows Service using the configuration file "agent-config.json" and Starts the service (Note: requires Administrator Privileges)
> agent install-start agent-config.json

#### Example 3:
Starts the Windows Service (Note: requires Administrator Privileges)
> agent start

#### Example 4:
Runs the Agent in the command line prompt using verbose logging and overrides the MQTT Port to 1884
> agent debug "" 1884


## MQTT Topic Structure

#### Devices
```
MTConnect/Devices/[DEVICE_UUID]/Device
```

#### Observations
```
MTConnect/Devices/[DEVICE_UUID]/Observations/[COMPONENT_TYPE]/[COMPONENT_ID]/[DATA_ITEM_CATEGORY]/[DATA_ITEM_TYPE]/[DATA_ITEM_ID]
MTConnect/Devices/[DEVICE_UUID]/Observations/[COMPONENT_TYPE]/[COMPONENT_ID]/[DATA_ITEM_CATEGORY]/[DATA_ITEM_TYPE]/SubTypes/[DATA_ITEM_SUBTYPE]/[DATA_ITEM_ID]
```
##### Conditions
Condition messages are sent as an array of Observations since a Condition may have multiple Fault States. This is similar to how the Current request functions in an HTTP Agent.

#### Assets
```
MTConnect/Devices/[DEVICE_UUID]/Assets/[ASSET_TYPE]/[ASSET_ID]
MTConnect/Assets/[ASSET_TYPE]/[ASSET_ID]
```
> Note: Assets are sent to two topics. One for the "Global" assets and one for the Device that the Asset was added to

#### Agent
The Agent topic contains information that would normally be in the Header of a Response Document from an HTTP Agent.
```
MTConnect/Assets/[AGENT_UUID]
```


## Configuration
More information about [Configurations](https://github.com/TrakHound/MTConnect.NET/tree/master/src/MTConnect.NET-Common/Configurations). The default configuration file is shown below :
```json
{
    "devices": "devices.xml",
    "port": 1883,
    "server": "localhost",

    "adapters": [
        {
            "deviceKey": "OKUMA.Lathe",
            "hostname": "localhost",
            "port": 7878
        }
    ],

    "devicesNamespaces": [
        {
            "alias": "e",
            "location": "urn:okuma.com:OkumaDevices:1.3",
            "path": "/schemas/OkumaDevices_1.3.xsd"
        }
    ],

    "streamsNamespaces": [
        {
            "alias": "e",
            "location": "urn:okuma.com:OkumaStreams:1.3",
            "path": "/schemas/OkumaStreams_1.3.xsd"
        }
    ]
}
```

#### MQTT Configuration

* `port` - The port number of the external MQTT broker to publish messages to.

* `server` - The server hostname of the external MQTT broker to publish messages to.

* `username` - The username of the external MQTT broker to publish messages to.

* `password` - The password of the external MQTT broker to publish messages to.

* `useTls` - Sets whether to use TLS in the connection to the external MQTT broker to publish messages to.


#### Agent Configuration

* `observationBufferSize` - The maximum number of Observations the agent can hold in its buffer

* `assetBufferSize` - The maximum number of assets the agent can hold in its buffer

* `durable` - Sets whether the Agent buffers are durable and retain state after restart

* `useBufferCompression` - Sets whether the durable Agent buffers use Compression

* `ignoreTimestamps` - Overwrite timestamps with the agent time. This will correct clock drift but will not give as accurate relative time since it will not take into consideration network latencies. This can be overridden on a per adapter basis.

* `defaultVersion` - Sets the default MTConnect version to output response documents for.

* `convertUnits` - Sets the default for Converting Units when adding Observations

* `ignoreObservationCase` - Sets the default for Ignoring the case of Observation values. Applicable values will be converted to uppercase

* `inputValidationLevel` - Sets the default input validation level when new Observations are added to the Agent. 0 = Ignore, 1 = Warning, 2 = Strict

* `monitorConfigurationFiles` - Sets whether Configuration files are monitored. If enabled and a configuration file is changed, the Agent will restart

* `configurationFileRestartInterval` - Sets the minimum time (in seconds) between Agent restarts when MonitorConfigurationFiles is enabled

* `enableMetrics` - Sets whether Agent Metrics are captured (ex. ObserationUpdateRate, AssetUpdateRate)


#### Device Configuration

* `devices` - The Path to look for the file(s) that represent the Device Information Models to load into the Agent. The path can either be a single file or a directory. The path can be absolute or relative to the executable's directory

#### Adapter Configuration

* `adapters` - List of SHDR Adapter connection configurations. For more information see()

* `allowShdrDevice` - Sets whether a Device Model can be sent from an SHDR Adapter

* `preserveUuid` - Do not overwrite the UUID with the UUID from the adapter, preserve the UUID for the Device. This can be overridden on a per adapter basis.

* `suppressIpAddress` - Suppress the Adapter IP Address and port when creating the Agent Device ids and names for 1.7. This applies to all adapters.

* `timeout` - The amount of time (in milliseconds) an adapter can be silent before it is disconnected.

* `reconnectInterval` - The amount of time (in milliseconds) between adapter reconnection attempts.

#### Windows Service Configuration

* `serviceName` - Changes the service name when installing or removing the service. This allows multiple agents to run as services on the same machine.

* `serviceAutoStart` - Sets the Service Start Type. True = Auto | False = Manual


## Logging
Logging is done using [NLog](https://github.com/NLog/NLog) which allows for customized logging through the NLog.config file located in the application's install directory. The loggers are setup so that there is a separate logger for:
- **(agent-logger)** MTConnect Agent
- **(agent-validation-logger)** MTConnect Data Validation Errors
- **(http-logger)** Http Server
- **(adapter-logger)** MTConnect Adapters
- **(adapter-shdr-logger)** Raw SHDR lines read by the Adapter (used for debugging adapters)

The default [NLog Configuration File](https://github.com/TrakHound/MTConnect.NET/blob/master/src/MTConnect.NET-Applications-Agents/NLog.config) is shown below:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">

    <!-- the targets to write to -->
    <targets>

        <!--Console-->
        <target name="logconsole" xsi:type="Console" />
        
        <!--Agent Log File-->
        <target xsi:type="File" name="agent-file" fileName="logs\agent-${shortdate}.log"
            layout="${longdate}|${event-properties:item=EventId_Id:whenEmpty=0}|${uppercase:${level}}|${logger}|${message} ${exception:format=tostring}" />

        <!--Agent Validation Log File-->
        <target xsi:type="File" name="agent-validation-file" fileName="logs\agent-validation-${shortdate}.log"
            layout="${longdate}|${event-properties:item=EventId_Id:whenEmpty=0}|${uppercase:${level}}|${logger}|${message} ${exception:format=tostring}" />

        <!--Mqtt Log File-->
        <target xsi:type="File" name="mqtt-file" fileName="logs\mqtt-${shortdate}.log"
            layout="${longdate}|${event-properties:item=EventId_Id:whenEmpty=0}|${uppercase:${level}}|${logger}|${message} ${exception:format=tostring}" />

        <!--Adapter Log File-->
        <target xsi:type="File" name="adapter-file" fileName="logs\adapter-${shortdate}.log"
                layout="${longdate}|${event-properties:item=EventId_Id:whenEmpty=0}|${uppercase:${level}}|${logger}|${message} ${exception:format=tostring}" />

        <!--Adapter SHDR Log File-->
        <target xsi:type="File" name="adapter-shdr-file" fileName="logs\adapter-shdr-${shortdate}.log"
                layout="${longdate}|${event-properties:item=EventId_Id:whenEmpty=0}|${uppercase:${level}}|${logger}|${message} ${exception:format=tostring}" />
        
    </targets>

    <!-- rules to map from logger name to target -->
    <rules>

        <!--Write to Console-->
        <logger name="*" minlevel="Info" writeTo="logconsole" />

        <!--Agent Logger-->
        <logger name="agent-logger" minlevel="Info" writeTo="agent-file" final="true" />
        
        <!--Agent Validation Logger (Used to log Data Validation Errors)-->
        <logger name="agent-validation-logger" minlevel="Warning" writeTo="agent-validation-file" final="true" />
        
        <!--Mqtt Logger-->
        <logger name="mqtt-logger" minlevel="Info" writeTo="mqtt-file" final="true" />
        
        <!--Adapter Logger-->
        <logger name="adapter-logger" minlevel="Info" writeTo="adapter-file" final="true" />
        
        <!--Adapter SHDR Logger (used to log raw SHDR data coming from an adapter)-->
        <logger name="adapter-shdr-logger" minlevel="Debug" writeTo="adapter-shdr-file" final="true" />

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
This project uses the MTConnect.NET-Applications-Agents-MQTT library (available on [Nuget](https://www.nuget.org/packages/MTConnect.NET-Applications-Agents-MQTT)) to create an MTConnect Agent application. More information about this library can be found [Here](https://github.com/TrakHound/MTConnect.NET/tree/master/src/MTConnect.NET-Applications-Agents-MQTT). The MTConnect.NET-Applications-Agents-MQTT library makes creating an MTConnect MQTT Agent application simple as well as makes it easy to keep updated using Nuget. A fully functionaly MTConnect Application can be created in just a few lines of code.

## Contribution / Feedback
- Please use the [Issues](https://github.com/TrakHound/MTConnect.NET/issues) tab to create issues for specific problems that you may encounter 
- Please feel free to use the [Pull Requests](https://github.com/TrakHound/MTConnect.NET/pulls) tab for any suggested improvements to the source code
- For any other questions or feedback, please contact TrakHound directly at **info@trakhound.com**.

## License
This application and it's source code is licensed under the [MIT License](https://choosealicense.com/licenses/mit/) and is free to use.
