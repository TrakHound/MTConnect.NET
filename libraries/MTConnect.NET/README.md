![MTConnect.NET Logo](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/mtconnect-net-02-md.png) 

# MTConnect.NET

[![MTConnect.NET](https://github.com/TrakHound/MTConnect.NET/actions/workflows/dotnet.yml/badge.svg)](https://github.com/TrakHound/MTConnect.NET/actions/workflows/dotnet.yml)


## Overview
MTConnect.NET is a fully featured .NET library for MTConnect to develop Agents, Adapters, and Clients. Supports MTConnect Versions up to 2.0.

The Agent, Buffers, and Adapter are separated into individual classes in order to allow for modular implementations such as the following : 

- A traditional Agent that uses a REST Api, in-memory buffer, and Adapters that communicate using the SHDR protocol
- An agent imbedded with the Adapter (which elminates the need for the Adapter TCP communication)
- Supports SHDR Apdaters
- Interfaces other than Http REST such as MQTT
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

## Supported Frameworks
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

## Agents
Agents are implemented using the MTConnectAgent class and IMTConnectAgent interface. The MTConnectAgent class implements the MTConnect standard and is inteded to be full implemenation. More information about agents can be found at [Agents](https://github.com/TrakHound/MTConnect.NET/tree/master/src/MTConnect.NET-Common/Agents) and Agent Applications can be found at [Agent Applications](https://github.com/TrakHound/MTConnect.NET/tree/master/applications/Agents).

### SHDR > HTTP Implementation
A SHDR to HTTP implementation is the traditional MTConnect Agent configuration. The agent reads from one or more Adapter applications that implement the SHDR Protocol. Data is then read from the Agent using the HTTP REST protocol. The agent and adapter(s) are typically separate applications. The agent and adapter(s) can still be run on the same PC (or HMI) but there is still TCP communication between them.

![Traditional Agent Architecture](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/dev/img/mtconnect-agent-http-shdr-communication-white.png) 

### HTTP > HTTP Implementation
An HTTP to HTTP implementation reads from other MTConnect Agents and forwards that data to a central MTConnect Agent. This implementation can be used to create a "Gateway" that multiple other MTConnect Agents can be forwarded to. This can be used to provide a single access point, implement stricter security policies, or upgrade an older agent without effecting other applications that may already be using the older version.

![Traditional Agent Architecture](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/dev/img/mtconnect-agent-http-http-communication-white.png) 

### Embedded Implementation
An embedded implementation uses the MTConnect.NET library to implement an MTConnect Agent in the same application that is reading from the machine PLC. This creates a simple and compact solution that can be deployed as a single application/product. When compared to a SHDR to HTTP implementation, this eliminates the need to use the SHDR protocol as well as eliminates the TCP communication between the Adapter and the Agent. Implementation is simplified using the [MTConnect.NET-Applications-Agents](https://github.com/TrakHound/MTConnect.NET/tree/master/src/MTConnect.NET-Applications-Agents) Library that can be as simple as a few lines of code and can be kept up to date using Nuget.

![Traditional Agent Architecture](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/dev/img/mtconnect-agent-http-communication-white.png) 
