![MTConnect.NET Logo](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/logo.png) 

# MTConnect.NET-Common
This library project contains the basic classes for implementing MTConnect. It has no dependency on a transport (HTTP, MQTT, SHDR) or a document format (XML, JSON); those are provided by the other MTConnect.NET libraries.

For end-to-end documentation and concept pages, see the [MTConnect.NET Docs](https://www.trakhound.com/docs/mtconnect-net).

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
            <td>MTConnect.NET-Common</td>
            <td><img src="https://img.shields.io/nuget/dt/MTConnect.NET-Common?style=for-the-badge&logo=nuget&label=%20&color=%23333"/></td>
            <td><a href="https://www.nuget.org/packages/MTConnect.NET-Common">https://www.nuget.org/packages/MTConnect.NET-Common</a></td>
        </tr>
    </tbody>
</table>

## Contents
- [Agents](https://github.com/TrakHound/MTConnect.NET/tree/master/libraries/MTConnect.NET-Common/Agents) : The `MTConnectAgent` class (Devices, Observations, and Assets) and the `MTConnectAgentBroker` class (adds the buffers and the MTConnect Response Documents)
- [Buffers](https://github.com/TrakHound/MTConnect.NET/tree/master/libraries/MTConnect.NET-Common/Buffers) : In-memory and File System backed Observation and Asset buffers
- [Devices](https://github.com/TrakHound/MTConnect.NET/tree/master/libraries/MTConnect.NET-Common/Devices) : Device, Component, Composition, and DataItem classes (including a class for each type defined in the MTConnect Standard)
- [Observations](https://github.com/TrakHound/MTConnect.NET/tree/master/libraries/MTConnect.NET-Common/Observations) and [Streams](https://github.com/TrakHound/MTConnect.NET/tree/master/libraries/MTConnect.NET-Common/Streams) : Observation classes and the MTConnectStreams Response Document
- [Assets](https://github.com/TrakHound/MTConnect.NET/tree/master/libraries/MTConnect.NET-Common/Assets) : Asset classes and the MTConnectAssets Response Document
- [Clients](https://github.com/TrakHound/MTConnect.NET/tree/master/libraries/MTConnect.NET-Common/Clients) : Interfaces that are implemented by the HTTP and MQTT clients
- [Adapters](https://github.com/TrakHound/MTConnect.NET/tree/master/libraries/MTConnect.NET-Common/Adapters) : The `IMTConnectAdapter` interface and `MTConnectAdapter` class that the SHDR adapters and the Adapter application are built on
- [Configurations](https://github.com/TrakHound/MTConnect.NET/tree/master/libraries/MTConnect.NET-Common/Configurations) : Agent and Adapter configuration classes

## Usage
```c#
using MTConnect.Agents;
using MTConnect.Devices;
using MTConnect.Devices.DataItems;
using MTConnect.Observations.Events;

// Create a Device with an Availability DataItem
var device = new Device();
device.Uuid = "2f2a6b9e-7d1c-4a55-9a1e-0d5b3f6c1a01";
device.Id = "device1";
device.Name = "Device1";

var availability = new AvailabilityDataItem(device.Id);
device.AddDataItem(availability);

// Create the Agent (uses in-memory buffers by default)
var agent = new MTConnectAgentBroker();
agent.Start();

// Add the Device and an Observation
agent.AddDevice(device);
agent.AddObservation(device.Uuid, availability.Id, Availability.AVAILABLE);

// Get the MTConnect Response Documents
var devicesDocument = agent.GetDevicesResponseDocument();
var streamsDocument = agent.GetDeviceStreamsResponseDocument();
```

## Supported Frameworks
.NET 10.0, 9.0, 8.0, 7.0, 6.0, .NET Standard 2.0, and .NET Framework 4.6.1 through 4.8
