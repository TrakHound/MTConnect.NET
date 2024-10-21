![MTConnect.NET Logo](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/mtconnect-net-03-md.png) 

# MTConnect Agent

[![MTConnect.NET](https://github.com/TrakHound/MTConnect.NET/actions/workflows/dotnet.yml/badge.svg)](https://github.com/TrakHound/MTConnect.NET/actions/workflows/dotnet.yml)

## Download
<table>
    <thead>
        <tr>
            <th style="text-align: left;min-width: 100px;">Name</th>
            <th style="text-align: left;"></th>
            <th style="text-align: left;">Link</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td>Installer</td>
            <td><img src="https://img.shields.io/github/downloads/TrakHound/MTConnect.NET/total?style=for-the-badge&logo=github&label=%20&color=%23333"/></td>
            <td><a href="https://github.com/TrakHound/MTConnect.NET/releases/latest">https://github.com/TrakHound/MTConnect.NET/releases/latest</a></td>
        </tr>
        <tr>
            <td>Docker</td>
            <td><img src="https://img.shields.io/docker/pulls/trakhound/mtconnect.net-agent?style=for-the-badge&logo=docker&label=%20&color=%23333"/></td>
            <td><a href="https://hub.docker.com/repository/docker/trakhound/mtconnect.net-agent">https://hub.docker.com/repository/docker/trakhound/mtconnect.net-agent</a></td>
        </tr>
    </tbody>
</table>

## Overview
This project is a full implementation of an MTConnect Agent used to read data from industrial machine tools and devices. This MTConnect Agent application is fully compatible with the latest **Version 2.4 of the MTConnect Standard**.

#### Features
- Plugin architecture to support Http Server, Mqtt Server, SHDR Adapters, etc.
- Processors for transforming input data using simple Python scripts
- Easy setup with Windows Installers
- Options to run as Windows Service or as a console application (typically for testing/debugging)
- Optional 'Durable' buffer used to retain the Agent data between application/machine restarts
- High performance / Low resource usage
- Flexible Device configurations (traditional 'devices.xml' file or 'devices' directory with individual Device files)
- On-Demand MTConnect Versioning allowing for older clients to request the version of MTConnect they are compatible with using HTTP Url parameters
- Configuration File monitoring to automatically restart the Agent upon configuration file changes
- Flexible Logging using NLog which can be used to output log information to separate files for easier analysis

![Agent-Diagram](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/agent-diagram.png)

## Capabilites
| Description                          | Supported          | Description                          | Supported          |
| :----------------------------------- |:------------------:| :----------------------------------- |:------------------:|
| MTConnect Probe                      | :white_check_mark: | HTTP Server                          | :white_check_mark: |
| MTConnect Current                    | :white_check_mark: | MQTT Server (Internal Broker)        | :white_check_mark: |
| MTConnect Sample                     | :white_check_mark: | MQTT Server (External Broker)        | :white_check_mark: |
| MTConnect Assets                     | :white_check_mark: | SHDR Adapters (Input Data)           | :white_check_mark: |
| MTConnect Interfaces                 | :white_check_mark: | HTTP Adapters (Input Data)           | :white_check_mark: |
| Configuration File Monitoring        | :white_check_mark: | MQTT Adapters (Input Data)           | :white_check_mark: |
| Unit Conversion                      | :white_check_mark: | XML Stylesheets                      | :white_check_mark: |
| Script Transformation (Python)       | :white_check_mark: | XML Static Files                     | :white_check_mark: |
| Windows Service                      | :white_check_mark: | XML Schemas                          | :white_check_mark: |
| Linux Supported                      | :white_check_mark: | XML Validation                       | :white_check_mark: |
| Durable File Buffer                  | :white_check_mark: | JSON (MTConnect.NET v5)              | :white_check_mark: |
| Debug Logging                        | :white_check_mark: | JSON (cppagent)                      | :white_check_mark: |
| MTConnect Entity Validation          | :white_check_mark: | TLS (HTTP & MQTT)                    | :white_check_mark: |


## Installation
Follow the steps below to install the MTConnect Agent HTTP application.

*Note: Installer (Innosetup) source code is available and will be added to repo this in a future version*

### Step #1
Read through the license agreement (MIT License) and click **"I accept the agreement"** if you agree with the terms.
Then click **"Next"** to proceed to the next page.
*For more information about the MIT license click [Here](https://choosealicense.com/licenses/mit/)*


![screenshot](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/MTConnect-Agent/Installation/installer-license.png) 

### Step #2
Select the directory to install the application in using the **"Browse"** button or accept the default.
Then click **"Next"** to proceed to the next page.


![screenshot](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/MTConnect-Agent/Installation/installer-directory.png) 

### Step #3
Select the .NET version and Architecture (x64 or x86) to install the application for
Then click **"Next"** to proceed to the next page.


![screenshot](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/MTConnect-Agent/Installation/installer-components.png) 

### Step #4
Review the list of components that will be installed and if correct then click **"Install"** to begin 
Then click **"Next"** to proceed to the next page.


![screenshot](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/MTConnect-Agent/Installation/installer-review.png) 

### Step #5
Select the **"Open Probe in Web Browser"** and/or **"Open Current in Web Browser"** to view the Agent output and verify access 
Then click **"Next"** to proceed to the next page.


![screenshot](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/MTConnect-Agent/Installation/installer-finish.png) 

### Step #6
View the Probe and Current requests in a Web Browser
- [Probe - http://localhost:5000](http://localhost:5000/)
- [Current - http://localhost:5000/current](http://localhost:5000/current)

#### Probe Request
![screenshot](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/MTConnect-Agent/Browser/probe-request-web-browser.png) 

#### Current Request
![screenshot](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/MTConnect-Agent/Browser/current-request-web-browser.png) 


## Usage
The Agent can be run from a command line prompt or as a Windows Service using the format below:
```
agent [help|install|install-start|start|stop|remove|debug|run|run-service] [configuration_file]

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
```
#### Example 1:
Install the Agent as a Windows Service (Note: requires Administrator Privileges)
 ```
 agent install
 ```

#### Example 2:
Install the Agent as a Windows Service using the configuration file "agent-config.json" and Starts the service (Note: requires Administrator Privileges)
```
agent install-start agent-config.json
```

#### Example 3:
Starts the Windows Service (Note: requires Administrator Privileges)
```
agent start
```


## Configuration
More information about [Configurations](https://github.com/TrakHound/MTConnect.NET/tree/master/libraries/MTConnect.NET-Common/Configurations). The default configuration file is shown below :
```yaml
# - Device Configuration -
devices: devices

# - Processors -
processors:
- python: # - Add Python Processor
    directory: processors

# - Modules -
modules:
  
- http-server: # - Add HTTP Server module
    hostname: localhost
    port: 5000
    allowPut: true
    indentOutput: true
    documentFormat: xml
    responseCompression:
    - gzip
    - br
    files:
    - path: schemas
    location: schemas
    - path: styles
    location: styles
    - path: styles/favicon.ico
    location: favicon.ico

- mqtt-relay: # - Add MQTT Relay module
    server: localhost
    port: 1883
    currentInterval: 5000
    sampleInterval: 500

- shdr-adapter: # - Add SHDR Adapter module for Device = M12346 and Port = 7878
    deviceKey: M12346
    hostname: localhost
    port: 7878

- shdr-adapter: # - Add SHDR Adapter module for Device = OKUMA-Lathe and Port = 7879
    deviceKey: OKUMA-Lathe
    hostname: localhost
    port: 7879

- mqtt-adapter: # - Add MQTT Adapter module for Device = M12346 and Topic = cnc-01
    deviceKey: M12346
    server: localhost
    port: 1883
    topic: cnc-01


# The maximum number of Observations the agent can hold in its buffer
observationBufferSize: 150000

# The maximum number of assets the agent can hold in its buffer
assetBufferSize: 1000

# Sets whether the Agent buffers are durable and retain state after restart
durable: false

# Sets the default MTConnect version to output response documents for.
defaultVersion: 2.3
```

#### Device Configuration

* `devices` - The Path to look for the file(s) that represent the Device Information Models to load into the Agent. The path can either be a single file or a directory. The path can be absolute or relative to the executable's directory

#### Module Configuration

* `modules` - Contains configurations for Agent Modules. These configurations are specific to the individual modules and may be customized for custom modules.

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


#### Windows Service Configuration

* `serviceName` - Changes the service name when installing or removing the service. This allows multiple agents to run as services on the same machine.

* `serviceAutoStart` - Sets the Service Start Type. True = Auto | False = Manual


## Logging
Logging is done using [NLog](https://github.com/NLog/NLog) which allows for customized logging through the NLog.config file located in the application's install directory. The loggers are setup so that there is a separate logger for:
- **(agent-logger)** MTConnect Agent
- **(agent-validation-logger)** MTConnect Data Validation Errors
- **(module-logger)** Modules

The default [NLog Configuration File](https://github.com/TrakHound/MTConnect.NET/blob/master/agent/MTConnect.NET-Applications-Agents/NLog.config) is shown below:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">

	<!-- the targets to write to -->
	<targets>

		<!--Console-->
		<target name="logconsole" xsi:type="Console" />

		<!--Application Log File-->
		<target xsi:type="File" name="application-file" fileName="logs\application-${shortdate}.log"
			layout="${longdate}|${event-properties:item=EventId_Id:whenEmpty=0}|${uppercase:${level}}|${logger}|${message} ${exception:format=tostring}" />
		
		<!--Agent Log File-->
		<target xsi:type="File" name="agent-file" fileName="logs\agent-${shortdate}.log"
			layout="${longdate}|${event-properties:item=EventId_Id:whenEmpty=0}|${uppercase:${level}}|${logger}|${message} ${exception:format=tostring}" />

		<!--Agent Validation Log File-->
		<target xsi:type="File" name="agent-validation-file" fileName="logs\agent-validation-${shortdate}.log"
			layout="${longdate}|${event-properties:item=EventId_Id:whenEmpty=0}|${uppercase:${level}}|${logger}|${message} ${exception:format=tostring}" />

		<!--Module Log File-->
		<target xsi:type="File" name="module-file" fileName="logs\module-${shortdate}.log"
			layout="${longdate}|${event-properties:item=EventId_Id:whenEmpty=0}|${uppercase:${level}}|${logger}|${message} ${exception:format=tostring}" />

		<!--Processor Log File-->
		<target xsi:type="File" name="processor-file" fileName="logs\processor-${shortdate}.log"
			layout="${longdate}|${event-properties:item=EventId_Id:whenEmpty=0}|${uppercase:${level}}|${logger}|${message} ${exception:format=tostring}" />

	</targets>

	<!-- rules to map from logger name to target -->
	<rules>

		<!--Write to Console-->
		<logger name="*" minlevel="Debug" writeTo="logconsole" />

		<!--Application Logger-->
		<logger name="application-logger" minlevel="Info" writeTo="application-file" final="true" />

		<!--Agent Logger-->
		<logger name="agent-logger" minlevel="Info" writeTo="agent-file" final="true" />
		
		<!--Agent Validation Logger (Used to log Data Validation Errors)-->
		<logger name="agent-validation-logger" minlevel="Warning" writeTo="agent-validation-file" final="true" />
		
		<!--Module Logger-->
		<logger name="module-logger" minlevel="Info" writeTo="module-file" final="true" />

		<!--Processor Logger-->
		<logger name="processor-logger" minlevel="Info" writeTo="processor-file" final="true" />

		<!--Skip non-critical Microsoft logs and so log only own logs (BlackHole) -->
		<logger name="Microsoft.*" maxlevel="Info" final="true" />
		<logger name="System.Net.Http.*" maxlevel="Info" final="true" />

	</rules>
</nlog>
```

## Source Code
This project uses the MTConnect.NET-Applications-Agents library (available on [Nuget](https://www.nuget.org/packages/MTConnect.NET-Applications-Agents)) to create an MTConnect Agent application. More information about this library can be found [Here](https://github.com/TrakHound/MTConnect.NET/tree/master/libraries/MTConnect.NET-Applications-Agents). The MTConnect.NET-Applications-Agents library makes creating an MTConnect Agent application simple as well as makes it easy to keep updated using Nuget. A fully functionaly MTConnect Application can be created in just a few lines of code.

## Contribution / Feedback
- Please use the [Issues](https://github.com/TrakHound/MTConnect.NET/issues) tab to create issues for specific problems that you may encounter 
- Please feel free to use the [Pull Requests](https://github.com/TrakHound/MTConnect.NET/pulls) tab for any suggested improvements to the source code
- For any other questions or feedback, please contact TrakHound directly at **info@trakhound.com**.

## License
This application and it's source code is licensed under the [MIT License](https://choosealicense.com/licenses/mit/) and is free to use and distribute.
