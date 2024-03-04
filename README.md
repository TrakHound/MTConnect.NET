![MTConnect.NET Logo](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/mtconnect-net-03-md.png) 

# MTConnect.NET

[![MTConnect.NET](https://github.com/TrakHound/MTConnect.NET/actions/workflows/dotnet.yml/badge.svg)](https://github.com/TrakHound/MTConnect.NET/actions/workflows/dotnet.yml)

> 12/4/2023 : Version 6.0 Released with new Agent, Adapter, & SysML Import

## Download
<table>
    <thead>
        <tr>
            <th style="text-align: left;min-width: 100px;">Name</th>
            <th style="text-align: left;min-width: 100px;"></th>
            <th style="text-align: left;"></th>
            <th style="text-align: left;">Link</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td>Agent</td>
            <td>Installer</td>
            <td><img src="https://img.shields.io/github/downloads/TrakHound/MTConnect.NET/total?style=for-the-badge&logo=github&label=%20&color=%23333"/></td>
            <td><a href="https://github.com/TrakHound/MTConnect.NET/releases/download/v5.4.3/MTConnect-Agent-HTTP-5.4.3-Install.exe">https://github.com/TrakHound/MTConnect.NET/releases</a></td>
        </tr>        
        <tr>
            <td>Agent</td>
            <td>Docker</td>
            <td><img src="https://img.shields.io/docker/pulls/trakhound/mtconnect.net-agent?style=for-the-badge&logo=docker&label=%20&color=%23333"/></td>
            <td><a href="https://hub.docker.com/repository/docker/trakhound/mtconnect.net-agent">https://hub.docker.com/repository/docker/trakhound/mtconnect.net-agent</a></td>
        </tr>
        <tr>
            <td>Client</td>
            <td>Nuget</td>
            <td><img src="https://img.shields.io/nuget/dt/MTConnect.NET?style=for-the-badge&logo=nuget&label=%20&color=%23333"/></td>
            <td><a href="https://www.nuget.org/packages/MTConnect.NET">https://www.nuget.org/packages/MTConnect.NET</a></td>
        </tr>
    </tbody>
</table>

## Overview
MTConnect.NET is a fully featured and fully Open Source .NET library for MTConnect to develop Agents, Adapters, and Clients. Supports MTConnect Versions up to 2.3. A pre-compiled Agent application is available to download as well as an Adapter application that can be easily customized.

- .NET Native MTConnect Agent
- Adapter framework used to send data to an MTConnect Agent
- Libraries to easily implement MTConnect Agent, Adapters, or Clients into custom applications

### Features
- Module based Agent & Adapter architecture
    - Supports running as Windows Service with easy to use command line arguments
    - Presistent Agent Buffers that are backed up on the File System. Retains state after Agent is restarted
- Fully compatible up to the latest MTConnect v2.3
    - Kept up to date by utilizing the MTConnect SysML Model to generate source files
    - Supports multiple MTConnect Version output. Automatically removes data that is not compatible with the requested version
- Full client support for requesting data from any MTConnect Agent (Probe, Current, Sample Stream, Assets, etc.).
    - Supports HTTP, MQTT, and SHDR
    - Supports compression (polling & streaming)
    - Supports XML & JSON
    - Supports HTTPS & TLS for secure communication
- Python Input Processors to transform data before loading into Agent
- Full data validation
  - Validation on Input
  - XML Schema Validation on output
  - Configurable Validation Levels
- Fully documented objects using text from the MTConnect Standard. This enables Intellisense in applications such as Visual Studio.
- Fully supports Unit conversion. Default Units and UnitConversion is done automatically when sending Streams and when reading Streams.

### Integrate
- Easily integrate with cloud services such as AWS and Azure
- Use client libraries to export MTConnect data to a Database (ex. SQL, Redis, MongoDB, etc.)
- Create custom dashboards and data collection applications to utilize equipment data
- Embed an MTConnect Agent into your adapter (remove need for separate SHDR Adapter)

#### MTConnect Version Compatibility
MTConnect.NET is designed to be fully compatible for all versions of the MTConnect standard. This is done through processing by the [MTConnectAgent](https://github.com/TrakHound/MTConnect.NET/tree/master/src/MTConnect.NET-Common/Agents/MTConnectAgent.cs) class before data is output. This allows the version to be a parameter when requesting data from the Agent. More information can be found in the [Devices README](https://github.com/TrakHound/MTConnect.NET/tree/master/src/MTConnect.NET-Common/Devices/README.md).

#### Data Validation
Validation is performed on a Device, Component, Composition, or DataItem level through the classes in [Devices](https://github.com/TrakHound/MTConnect.NET/tree/master/src/MTConnect.NET-Common/Devices). This allows for validation without the need to use XML schemas (although XML Validation against XSD schemas is supported).

### Live Demo
A live demo of the MTConnect Gateway HTTP Agent (AspNetCore) application is running at [https://mtconnect.trakhound.com](https://mtconnect.trakhound.com?outputComments=true&indentOutput=true&version=2.1).
- [https://mtconnect.trakhound.com/current](https://mtconnect.trakhound.com/current?outputComments=true&indentOutput=true&version=2.1)
- [https://mtconnect.trakhound.com/sample](https://mtconnect.trakhound.com/sample?outputComments=true&indentOutput=true&version=2.1&count=500)
- [https://mtconnect.trakhound.com/assets](https://mtconnect.trakhound.com/assets?outputComments=true&indentOutput=true)

## Clients

#### Client Interfaces
- [IMTConnectClient](https://github.com/TrakHound/MTConnect.NET/blob/master/src/MTConnect.NET-Common/Clients/IMTConnectClient.cs) : Interface used to read MTConnect response documents (Probe, Current, Sample, and Assets)
- [IMTConnectEntityClient](https://github.com/TrakHound/MTConnect.NET/blob/master/src/MTConnect.NET-Common/Clients/IMTConnectEntityClient.cs) : Interface used to read MTConnect entities (Device, Observation, Asset)

#### Client Classes
- [MTConnectHttpClient](https://github.com/TrakHound/MTConnect.NET/blob/master/src/MTConnect.NET-HTTP/Clients/MTConnectHttpClient.cs) : Reads from MTConnect Agents using the MTConnect HTTP REST Api. Supports both polling and streaming. Supports compression. Supports XML & JSON.
- [MTConnectMqttClient](https://github.com/TrakHound/MTConnect.NET/blob/master/src/MTConnect.NET-MQTT/Clients/MTConnectMqttClient.cs) : Reads MTConnect data from an MQTT Broker. Supports the latest MTConnect MQTT Protocol.

## Agents

#### Module Agent Application
A preconfigured [Application](https://github.com/TrakHound/MTConnect.NET/tree/version-6.0/agent/MTConnect.NET-Agent) & [Library](https://github.com/TrakHound/MTConnect.NET/tree/version-6.0/agent/MTConnect.NET-Applications-Agents) to build an Agent is available and supports:
- Modular architecture
    - HTTP Server Module
    - SHDR Adapter Module(s)
    - MQTT Broker Module
    - MQTT Relay Module
    - etc.
- Easy Windows Installer
- Linux Compatible
- Run as a Windows Service
- Extensible configuration file and monitors for changes

#### Agent Classes
- [MTConnectAgent](https://github.com/TrakHound/MTConnect.NET/blob/master/src/MTConnect.NET-Common/Agents/MTConnectAgent.cs) : Handles MTConnect Entities (Device, Observation, Asset), Unit Conversion, Filtering, etc.
- [MTConnectAgentBroker](https://github.com/TrakHound/MTConnect.NET/blob/master/src/MTConnect.NET-Common/Agents/MTConnectAgent.cs) : Handles MTConnect Requests to respond with Response Documents (Probe, Current, Sample, Assets) specified in the MTConnect Standard, Buffers, etc.

## Adapters

#### Modular Adapter Application
A preconfigured [Application](https://github.com/TrakHound/MTConnect.NET/tree/version-6.0/adapter/MTConnect.NET-Adapter) & [Library](https://github.com/TrakHound/MTConnect.NET/tree/version-6.0/adapter/MTConnect.NET-Applications-Adapter) to build an Adapter is available and supports:
- Modular architecture
    - SHDR Module (export data to an MTConnect Agent using the SHDR protocol)
    - MQTT Module (export data to an MQTT Broker to be read by an MTConnect Agent)
- Run as a Windows Service
- Extensible configuration file and monitors for changes
- Customizable Data Source engine (to read from a PLC)
- Updated through a Nuget package (no source code copy & paste required when updating to new versions)

#### SHDR Adapter Classes
- [ShdrAdapter](https://github.com/TrakHound/MTConnect.NET/blob/master/src/MTConnect.NET-SHDR/Adapters/Shdr/ShdrAdapter.cs) : Sends the most recent values On-Demand using the SendCurrent() method. This is used when full control of the communication is needed.
- [ShdrIntervalAdapter](https://github.com/TrakHound/MTConnect.NET/blob/master/src/MTConnect.NET-SHDR/Adapters/Shdr/ShdrIntervalAdapter.cs) : Sends the most recent values at the specified Interval. This is used when a set interval is adequate and the most recent value is all that is needed
- [ShdrQueueAdapter](https://github.com/TrakHound/MTConnect.NET/blob/master/src/MTConnect.NET-SHDR/Adapters/Shdr/ShdrQueueAdapter.cs) : Queues all values that are sent from the PLC and sends them all on demand using the SendBuffer() method. This is used when all values are needed and full control of the communication is needed.
- [ShdrIntervalQueueAdapter](https://github.com/TrakHound/MTConnect.NET/blob/master/src/MTConnect.NET-SHDR/Adapters/Shdr/ShdrIntervalQueueAdapter.cs) : Queues all values that are sent from the PLC and sends any queued values at the specified Interval. This is used when all values are needed but an interval is adequate.

## Nuget Packages
The Nuget packages for the libraries in this repo are listed below:
<table>
    <thead>
        <tr>
            <th style="text-align: left;min-width: 100px;">Name</th>
            <th style="text-align: center;min-width: 90px;">Downloads</th>
            <th style="text-align: left;">Link</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td>MTConnect.NET</td>
            <td style="text-align: center;"><img src="https://img.shields.io/nuget/dt/MTConnect.NET?style=for-the-badge&logo=nuget&label=%20&color=%23333"/></td>
            <td><a href="https://www.nuget.org/packages/MTConnect.NET">https://www.nuget.org/packages/MTConnect.NET</a></td>
        </tr>
        <tr>
            <td>MTConnect.NET-Common</td>
            <td style="text-align: center;"><img src="https://img.shields.io/nuget/dt/MTConnect.NET-Common?style=for-the-badge&logo=nuget&label=%20&color=%23333"/></td>
            <td><a href="https://www.nuget.org/packages/MTConnect.NET-Common">https://www.nuget.org/packages/MTConnect.NET-Common</a></td>
        </tr>
        <tr>
            <td>MTConnect.NET-HTTP</td>
            <td style="text-align: center;"><img src="https://img.shields.io/nuget/dt/MTConnect.NET-HTTP?style=for-the-badge&logo=nuget&label=%20&color=%23333"/></td>
            <td><a href="https://www.nuget.org/packages/MTConnect.NET-HTTP">https://www.nuget.org/packages/MTConnect.NET-HTTP</a></td>
        </tr>
        <tr>
            <td>MTConnect.NET-SHDR</td>
            <td style="text-align: center;"><img src="https://img.shields.io/nuget/dt/MTConnect.NET-SHDR?style=for-the-badge&logo=nuget&label=%20&color=%23333"/></td>
            <td><a href="https://www.nuget.org/packages/MTConnect.NET-SHDR">https://www.nuget.org/packages/MTConnect.NET-SHDR</a></td>
        </tr>
        <tr>
            <td>MTConnect.NET-MQTT</td>
            <td style="text-align: center;"><img src="https://img.shields.io/nuget/dt/MTConnect.NET-MQTT?style=for-the-badge&logo=nuget&label=%20&color=%23333"/></td>
            <td><a href="https://www.nuget.org/packages/MTConnect.NET-MQTT">https://www.nuget.org/packages/MTConnect.NET-MQTT</a></td>
        </tr>
        <tr>
            <td>MTConnect.NET-XML</td>
            <td style="text-align: center;"><img src="https://img.shields.io/nuget/dt/MTConnect.NET-XML?style=for-the-badge&logo=nuget&label=%20&color=%23333"/></td>
            <td><a href="https://www.nuget.org/packages/MTConnect.NET-XML">https://www.nuget.org/packages/MTConnect.NET-XML</a></td>
        </tr>
        <tr>
            <td>MTConnect.NET-JSON</td>
            <td style="text-align: center;"><img src="https://img.shields.io/nuget/dt/MTConnect.NET-JSON?style=for-the-badge&logo=nuget&label=%20&color=%23333"/></td>
            <td><a href="https://www.nuget.org/packages/MTConnect.NET-JSON">https://www.nuget.org/packages/MTConnect.NET-JSON</a></td>
        </tr>
        <tr>
            <td>MTConnect.NET-Services</td>
            <td style="text-align: center;"><img src="https://img.shields.io/nuget/dt/MTConnect.NET-Services?style=for-the-badge&logo=nuget&label=%20&color=%23333"/></td>
            <td><a href="https://www.nuget.org/packages/MTConnect.NET-Services">https://www.nuget.org/packages/MTConnect.NET-Services</a></td>
        </tr>
        <tr>
            <td>MTConnect.NET-DeviceFinder</td>
            <td style="text-align: center;"><img src="https://img.shields.io/nuget/dt/MTConnect.NET-DeviceFinder?style=for-the-badge&logo=nuget&label=%20&color=%23333"/></td>
            <td><a href="https://www.nuget.org/packages/MTConnect.NET-DeviceFinder">https://www.nuget.org/packages/MTConnect.NET-DeviceFinder</a></td>
        </tr>
    </tbody>
</table>

## Supported Frameworks
- .NET 8.0
- .NET 7.0
- .NET 6.0
- .NET 5.0
- .NET Core 3.1
- .NET Standard 2.0
- .NET Framework 4.8
- .NET Framework 4.7.2
- .NET Framework 4.7.1
- .NET Framework 4.7
- .NET Framework 4.6.2
- .NET Framework 4.6.1

## Developer Notes
This repo along with the libraries and applications are free to use and distribute and hopefully will help those that are looking at either getting started using MTConnect or those that are looking to use MTConnect for both basic and more advanced use cases.

Feel free to comment, or create pull-requests for anything that could be coded, formatted, or worded better. Attention to detail and continuous improvement are important in manufacturing so they should be just as important for manufacturing software.

Thanks for your interest in using these libraries and applications and feel free to contribute or give feedback.

\- Patrick
