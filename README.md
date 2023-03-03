![MTConnect.NET Logo](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/dev/img/mtconnect-net-03-md.png) 

# MTConnect.NET

[![MTConnect.NET](https://github.com/TrakHound/MTConnect.NET/actions/workflows/dotnet.yml/badge.svg)](https://github.com/TrakHound/MTConnect.NET/actions/workflows/dotnet.yml)
[![GitHub all releases](https://img.shields.io/github/downloads/TrakHound/MTConnect.NET/total?label=Release%20Downloads)](https://github.com/TrakHound/MTConnect.NET/releases/latest)
[![Nuget](https://img.shields.io/nuget/dt/MTConnect.NET?label=Nuget%20Downloads)](https://www.nuget.org/packages/MTConnect.NET)

> 1/22/2023 Updated to support MTConnect 2.1

## Overview
MTConnect.NET is a fully featured .NET library for MTConnect to develop Agents, Adapters, and Clients. Supports MTConnect Versions up to 2.1.

The Agent, Buffers, and Adapter are separated into individual classes in order to allow for modular implementations such as the following : 

- A traditional Agent that uses a REST Api, in-memory buffer, and Adapters that communicate using the SHDR protocol
- An agent imbedded with the Adapter (which elminates the need for the Adapter TCP communication)
- Supports SHDR Apdaters
- Supports HTTP and MQTT
- Integration with cloud services such as AWS and Azure

Other features of MTConnect.NET :
- Extensible through plugin libraries to extend Types
- Presistent Buffers that are backed up on the File System. Retains state after Agent is restarted
- Supports multiple MTConnect Version output. Automatically removes data that is not compatible with the requested version
- Supports running as Windows Service with easy to use command line arguments
- Full data validation
  - Validation on Input
  - XML Schema Validation on output
  - Configurable Validation Levels
- Fully documented objects using text from the MTConnect Standard. This enables Intellisense in applications such as Visual Studio.
- Full list of Device, Component, Composition, and DataItem types. See [Devices](https://github.com/TrakHound/MTConnect.NET/tree/master/src/MTConnect.NET-Common/Devices) for more information.
- Full list of Asset types. See [Devices](https://github.com/TrakHound/MTConnect.NET/tree/master/src/MTConnect.NET-Common/Assets) for more information.
- Fully supports Unit conversion. Default Units and UnitConversion is done automatically when sending Streams and when reading Streams.
- Full client support for requesting data from any MTConnect Agent (Probe, Current, Sample Stream, Assets, etc.). See [Clients](https://github.com/TrakHound/MTConnect.NET/tree/master/src/MTConnect.NET-HTTP/Clients/Rest) for more information.
- (In-Progress) [Models](https://github.com/TrakHound/MTConnect.NET/tree/master/src/MTConnect.NET-Models/Models) framework for setting and accessing data using an object model as opposed to DataItem ID's and Types

## Agent Applications

#### Recommended (Windows / Linux)
- [MTConnect HTTP Agent](https://github.com/TrakHound/MTConnect.NET/tree/master/applications/Agents/MTConnect-Agent-Http) : MTConnect Agent application is fully compatible with the latest Version 2.1 of the MTConnect Standard. It uses the SHDR protocol to receive data from Adapters, an in-memory buffer with an optional durable file system based buffer, and an Http REST interface for retrieving data.

- [MTConnect HTTP Gateway Agent](https://github.com/TrakHound/MTConnect.NET/tree/master/applications/Agents/MTConnect-Agent-Http-Gateway) : MTConnect Agent application is fully compatible with the latest Version 2.1 of the MTConnect Standard. It receives data from other MTConnect Agents using HTTP, an in-memory buffer with an optional durable file system based buffer, and an Http REST interface for retrieving data.

- [MTConnect MQTT Relay Agent](https://github.com/TrakHound/MTConnect.NET/tree/master/applications/Agents/MTConnect-Agent-MQTT-Relay) : This MTConnect Agent application is fully compatible with the latest Version 2.1 of the MTConnect Standard. It uses the SHDR protocol to receive data from Adapters, an in-memory buffer with an optional durable file system based buffer, and an MQTT client to publish messages to an external MQTT Broker.

- [MTConnect MQTT Broker Agent](https://github.com/TrakHound/MTConnect.NET/tree/master/applications/Agents/MTConnect-Agent-MQTT-Broker) : This MTConnect Agent application is fully compatible with the latest Version 2.1 of the MTConnect Standard. It uses the SHDR protocol to receive data from Adapters, an in-memory buffer with an optional durable file system based buffer, and a built-in MQTT broker.

#### Specialized (IIS)

- [MTConnect HTTP Agent - AspNetCore](https://github.com/TrakHound/MTConnect.NET/tree/master/applications/Agents/MTConnect-Agent-Http-AspNetCore) : Similar to the MTConnect Agent application but uses either the built-in Kestrel server or can be setup through IIS (Internet Information Services). This allows the agent to be used with all of the features available through ASP.NET and IIS such as security, permissions, monitoring, etc.

- [MTConnect HTTP Gateway Agent - AspNetCore](https://github.com/TrakHound/MTConnect.NET/tree/master/applications/Agents/MTConnect-Agent-Http-Gateway-AspNetCore) : An Agent that runs mulitple MTConnectClients on the backend and passes that data to an MTConnectAgent. This can be used to access MTConnect data on a central server. Uses either the built-in Kestrel server or can be setup through IIS (Internet Information Services). This allows the agent to be used with all of the features available through ASP.NET and IIS such as security, permissions, monitoring, etc.

#### In Progress

- [MTConnect MQTT Gateway Agent](https://github.com/TrakHound/MTConnect.NET/tree/master/applications/Agents/MTConnect-Agent-MQTT-Gateway) : (In-Progress) An MTConnect Gateway Agent with an MQTT broker built-in.

### Live Demo
A live demo of the MTConnect Gateway HTTP Agent (AspNetCore) application is running at [https://mtconnect.trakhound.com](https://mtconnect.trakhound.com?outputComments=true&indentOutput=true&version=1.8).
- [https://mtconnect.trakhound.com/current](https://mtconnect.trakhound.com/current?outputComments=true&indentOutput=true&version=1.8)
- [https://mtconnect.trakhound.com/sample](https://mtconnect.trakhound.com/sample?outputComments=true&indentOutput=true&version=1.8&count=500)
- [https://mtconnect.trakhound.com/assets](https://mtconnect.trakhound.com/assets?outputComments=true&indentOutput=true)

## Docker
Docker images are located at : 
- MTConnect HTTP Agent : [https://hub.docker.com/r/trakhound/mtconnect-agent-http](https://hub.docker.com/r/trakhound/mtconnect-agent-http)

## Nuget Packages
The Nuget packages for the libraries in this repo are listed below:
- [MTConnect.NET](https://www.nuget.org/packages/MTConnect.NET/)
- [MTConnect.NET-Common](https://www.nuget.org/packages/MTConnect.NET-Common/)
- [MTConnect.NET-HTTP](https://www.nuget.org/packages/MTConnect.NET-HTTP/)
- [MTConnect.NET-HTTP-AspNetCore](https://www.nuget.org/packages/MTConnect.NET-HTTP-AspNetCore/)
- [MTConnect.NET-XML](https://www.nuget.org/packages/MTConnect.NET-XML/)
- [MTConnect.NET-JSON](https://www.nuget.org/packages/MTConnect.NET-JSON/)
- [MTConnect.NET-SHDR](https://www.nuget.org/packages/MTConnect.NET-SHDR/)
- [MTConnect.NET-MQTT](https://www.nuget.org/packages/MTConnect.NET-MQTT/)
- [MTConnect.NET-Services](https://www.nuget.org/packages/MTConnect.NET-Services/)
- [MTConnect.NET-DeviceFinder](https://www.nuget.org/packages/MTConnect.NET-DeviceFinder/)
- [MTConnect.NET-Applications-Agents](https://www.nuget.org/packages/MTConnect.NET-Applications-Agents/)
- [MTConnect.NET-Applications-Agents-MQTT](https://www.nuget.org/packages/MTConnect.NET-Applications-Agents-MQTT/)

## Supported Frameworks
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

## MTConnect Version Compatibility
MTConnect.NET is designed to be fully compatible for all versions of the MTConnect standard. This is done through processing by the [MTConnectAgent](https://github.com/TrakHound/MTConnect.NET/tree/master/src/MTConnect.NET-Common/Agents/MTConnectAgent.cs) class before data is output. This allows the version to be a parameter when requesting data from the Agent. More information can be found in the [Devices README](https://github.com/TrakHound/MTConnect.NET/tree/master/src/MTConnect.NET-Common/Devices/README.md).

## Data Validation
Validation is performed on a Device, Component, Composition, or DataItem level through the classes in [Devices](https://github.com/TrakHound/MTConnect.NET/tree/master/src/MTConnect.NET-Common/Devices). This allows for validation without the need to use XML schemas (although XML Validation against XSD schemas is supported).

## Releases
Releases are available at : [Releases](https://github.com/TrakHound/MTConnect.NET/releases)

## Agents
Agents are implemented using the MTConnectAgent class and IMTConnectAgent interface. The MTConnectAgent class implements the MTConnect standard and is inteded to be full implemenation. More information about agents can be found at [Agents](https://github.com/TrakHound/MTConnect.NET/tree/master/src/MTConnect.NET-Common/Agents) and Agent Applications can be found at [Agent Applications](https://github.com/TrakHound/MTConnect.NET/tree/master/applications/Agents).

### SHDR > HTTP Implementation
A SHDR to HTTP implementation is the traditional MTConnect Agent configuration. The agent reads from one or more Adapter applications that implement the SHDR Protocol. Data is then read from the Agent using the HTTP REST protocol. The agent and adapter(s) are typically separate applications. The agent and adapter(s) can still be run on the same PC (or HMI) but there is still TCP communication between them.

![Traditional Agent Architecture](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/mtconnect-agent-http-shdr-communication-white.png)

### HTTP > HTTP Implementation
An HTTP to HTTP implementation reads from other MTConnect Agents and forwards that data to a central MTConnect Agent. This implementation can be used to create a "Gateway" that multiple other MTConnect Agents can be forwarded to. This can be used to provide a single access point, implement stricter security policies, or upgrade an older agent without effecting other applications that may already be using the older version.

![Traditional Agent Architecture](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/mtconnect-agent-http-http-communication-white.png)

### SHDR > MQTT Implementation
A SHDR to MQTT implementation uses MQTT to send and receive messages. The agent reads from one or more Adapter applications that implement the SHDR Protocol. Data is then read from the Agent using the MQTT protocol. The agent and adapter(s) are typically separate applications. The agent and adapter(s) can still be run on the same PC (or HMI) but there is still TCP communication between them.

![Traditional Agent Architecture](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/mtconnect-agent-mqtt-shdr-communication-white.png)

### Embedded Implementation
An embedded implementation uses the MTConnect.NET library to implement an MTConnect Agent in the same application that is reading from the machine PLC. This creates a simple and compact solution that can be deployed as a single application/product. When compared to a SHDR to HTTP implementation, this eliminates the need to use the SHDR protocol as well as eliminates the TCP communication between the Adapter and the Agent. Implementation is simplified using the [MTConnect.NET-Applications-Agents](https://github.com/TrakHound/MTConnect.NET/tree/master/src/MTConnect.NET-Applications-Agents) Library that can be as simple as a few lines of code and can be kept up to date using Nuget.

![Traditional Agent Architecture](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/mtconnect-agent-http-communication-white.png)

## Adapters
### SHDR Adapter
Adapters are used to convert data read from a machine or PLC to the SHDR Protocol that can then be sent over TCP to an MTConnect Agent. There are several adapter types available in the [MTConnect.NET-SHDR](https://github.com/TrakHound/MTConnect.NET/tree/master/src/MTConnect.NET-SHDR) library that are listed below:
- [ShdrAdapter](https://github.com/TrakHound/MTConnect.NET/blob/master/src/MTConnect.NET-SHDR/Adapters/Shdr/ShdrAdapter.cs) : Sends the most recent values On-Demand using the SendCurrent() method. This is used when full control of the communication is needed.
- [ShdrIntervalAdapter](https://github.com/TrakHound/MTConnect.NET/blob/master/src/MTConnect.NET-SHDR/Adapters/Shdr/ShdrIntervalAdapter.cs) : Sends the most recent values at the specified Interval. This is used when a set interval is adequate and the most recent value is all that is needed
- [ShdrQueueAdapter](https://github.com/TrakHound/MTConnect.NET/blob/master/src/MTConnect.NET-SHDR/Adapters/Shdr/ShdrQueueAdapter.cs) : Queues all values that are sent from the PLC and sends them all on demand using the SendBuffer() method. This is used when all values are needed and full control of the communication is needed.
- [ShdrIntervalQueueAdapter](https://github.com/TrakHound/MTConnect.NET/blob/master/src/MTConnect.NET-SHDR/Adapters/Shdr/ShdrIntervalQueueAdapter.cs) : Queues all values that are sent from the PLC and sends any queued values at the specified Interval. This is used when all values are needed but an interval is adequate.

## Developer Notes
This repo along with the libraries and applications are free to use and hopefully will help those that are looking at either getting started using MTConnect or those that are looking to use MTConnect for more advanced use cases.

Feel free to comment, or create pull-requests for anything that could be coded, formatted, or worded better. Attention to detail and continuous improvement are important in manufacturing so they should be just as important for manufacturing software.

One of this project's goals is to expand the use cases for MTConnect and by breaking apart the functionalities of the agent, hopefully that will allow others to be creative in how to use the MTConnect standard.

Hopefully this repo will serve as a "one stop shop" for .NET developers looking to use MTConnect. If anyone is interested in developing a similar repo for another framework or language, feel free to use this as a guide as I imagine some of the classes (which is the most tedious part of the code) could be converted to other languages fairly easily.

This MTConnect.NET update is Part 1 of **The TrakHound Project** which is a project to provide open source code as well as products for each part of a full IIOT implementation. Please show support for our project at [www.TrakHound.com](http://www.trakhound.com).

Thanks for your interest in using these libraries and applications and feel free to contribute or give feedback.

\- Patrick
