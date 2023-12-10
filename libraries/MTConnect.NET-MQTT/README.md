![MTConnect.NET Logo](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/mtconnect-net-03-md.png) 

# MTConnect.NET-MQTT
MTConnect.NET-MQTT is an extension library to MTConnect.NET that provides an MQTT Broker & Client interface to an IMTConnectAgentBroker interface.

> Updated for Version 6

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
            <td>MTConnect.NET-MQTT</td>
            <td><img src="https://img.shields.io/nuget/dt/MTConnect.NET-MQTT?style=for-the-badge&logo=nuget&label=%20&color=%23333"/></td>
            <td><a href="https://www.nuget.org/packages/MTConnect.NET-MQTT">https://www.nuget.org/packages/MTConnect.NET-MQTT</a></td>
        </tr>
    </tbody>
</table>

## Protocols

[Output](https://github.com/TrakHound/MTConnect.NET/tree/master/libraries/MTConnect.NET-MQTT/README.md#Output) - Protocol for reading the output of an MTConnect Agent

[Input](https://github.com/TrakHound/MTConnect.NET/tree/master/libraries/MTConnect.NET-MQTT/README.md#Input) - Protocol for publishing data from a Data Source to an MTConnect Agent

# Output

## Topic Structures

The **Document Topic Structure** uses the standard MTConnect Response documents as message payload and the standard MTConnect API endpoints as topics.

`[TOPIC_PREFIX]/Probe/[DEVICE_UUID]` - The topic where MTConnectDevices Response documents are published

`[TOPIC_PREFIX]/Current/[DEVICE_UUID]` - The topic where MTConnectStreams Response documents are published at the configured **CurrentInterval**

`[TOPIC_PREFIX]/Sample/[DEVICE_UUID]` - The topic where MTConnectStreams Response documents are published at the the configured **SampleInterval** when new data is added to the Agent

`[TOPIC_PREFIX]/Asset/[DEVICE_UUID]/[ASSET_ID]` - The topic where MTConnectAssets Response documents are published


## Protocol

### Probe
Each device is sent in an MTConnectDevices Response document. The message is published upon Agent start/restart or when a Device is changed or a new Device is added. 

#### Topic
```
[TOPIC_PREFIX]/Probe/[DEVICE_UUID]
```

#### Payload
The payload currently defaults to use the **json-cppagent** Document Format ID.

> See example MTConnectDevices Response Document Payload: (https://github.com/TrakHound/MTConnect.NET/tree/master/libraries/MTConnect.NET-JSON-cppagent/Examples/MTConnectDevicesResponseDocument.json)


### Current

#### Topic
```
[TOPIC_PREFIX]/Current/[DEVICE_UUID]
```

#### Payload
The payload currently defaults to use the **json-cppagent** Document Format ID.

> See example MTConnectDevices Response Document Payload: (https://github.com/TrakHound/MTConnect.NET/tree/master/libraries/MTConnect.NET-JSON-cppagent/Examples/MTConnectStreamsResponseDocument.json)

### Sample

#### Topic
```
[TOPIC_PREFIX]/Sample/[DEVICE_UUID]
```

#### Payload
The payload currently defaults to use the **json-cppagent** Document Format ID.

> See example MTConnectDevices Response Document Payload: (https://github.com/TrakHound/MTConnect.NET/tree/master/libraries/MTConnect.NET-JSON-cppagent/Examples/MTConnectStreamsResponseDocument.json)

### Asset
```
[TOPIC_PREFIX]/Asset/[DEVICE_UUID]/[ASSET_ID]
```

#### Payload
The payload currently defaults to use the **json-cppagent** Document Format ID.

> See example MTConnectDevices Response Document Payload: (https://github.com/TrakHound/MTConnect.NET/tree/master/libraries/MTConnect.NET-JSON-cppagent/Examples/MTConnectStreamsResponseDocument.json)

# Input
The input protocol is used to publish data to an MTConnect Agent using MQTT. This can be either publishing to an external MQTT broker or to the internal MQTT broker (using the MqttBroker Agent Module).
