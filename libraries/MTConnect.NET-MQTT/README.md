![MTConnect.NET Logo](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/mtconnect-net-03-md.png) 

# MTConnect.NET-MQTT
MTConnect.NET-MQTT is an extension library to MTConnect.NET that provides an MQTT Broker & Client interface to an IMTConnectAgentBroker interface.

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

# Topic Structures
`Document` - Topics are the same as HTTP (Probe, Current, Sample, & Assets). Payloads are the corresponding MTConnect Response Documents. This provides a simple protocol that is performance based for applications with high frequency updates.

`Entity` - Topics are expanded where each Entity has it's own topic. This provides an easy to read interface for tools such as NodeRed, etc.


## Document Topic Structure
This topic structure is the official MQTT based protocol for MTConnect.

## Topics

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

> **[Learn More](https://github.com/TrakHound/MTConnect.NET/tree/master/libraries/MTConnect.NET-JSON-cppagent/Examples/MTConnectDevicesResponseDocument.json)** : See example MTConnectDevices Response Document Payload

### Current

#### Topic
```
[TOPIC_PREFIX]/Current/[DEVICE_UUID]
```

#### Payload
The payload currently defaults to use the **json-cppagent** Document Format ID.

> **[Learn More](https://github.com/TrakHound/MTConnect.NET/tree/master/libraries/MTConnect.NET-JSON-cppagent/Examples/MTConnectStreamsResponseDocument.json)** : See example MTConnectDevices Response Document Payload

### Sample

#### Topic
```
[TOPIC_PREFIX]/Sample/[DEVICE_UUID]
```

#### Payload
The payload currently defaults to use the **json-cppagent** Document Format ID.

> **[Learn More](https://github.com/TrakHound/MTConnect.NET/tree/master/libraries/MTConnect.NET-JSON-cppagent/Examples/MTConnectStreamsResponseDocument.json)** : See example MTConnectDevices Response Document Payload

### Asset
```
[TOPIC_PREFIX]/Asset/[DEVICE_UUID]/[ASSET_ID]
```

#### Payload
The payload currently defaults to use the **json-cppagent** Document Format ID.

> **[Learn More](https://github.com/TrakHound/MTConnect.NET/tree/master/libraries/MTConnect.NET-JSON-cppagent/Examples/MTConnectStreamsResponseDocument.json)** : See example MTConnectDevices Response Document Payload


## Entity Topic Structure
This topic structure is designed to supplement the Document topic structure and be used for use cases that may require a more simple protocol that requires less payload parsing or the ability to subscribe to specific DataItems.


### Devices
The **MTConnect/Devices** topics are used to send data that is in an MTConnectDevicesResponse document.

```
MTConnect/Devices/[DEVICE_UUID]/Device
```

### Observations
Observations may use the **MTConnectMqttFormat.Flat** or **MTConnectMqttFormat.Hierarchy** option to specify how the topics are structured.

MTConnectMqttFormat.Flat Format:
```
MTConnect/Devices/[DEVICE_UUID]/Observations/[DATA_ITEM_ID]
```

MTConnectMqttFormat.Hierarchy Format:
```
MTConnect/Devices/[DEVICE_UUID]/Observations/[COMPONENT_TYPE]/[COMPONENT_ID]/[DATA_ITEM_CATEGORY]/[DATA_ITEM_TYPE]/[DATA_ITEM_ID]
MTConnect/Devices/[DEVICE_UUID]/Observations/[COMPONENT_TYPE]/[COMPONENT_ID]/[DATA_ITEM_CATEGORY]/[DATA_ITEM_TYPE]/SubTypes/[DATA_ITEM_SUBTYPE]/[DATA_ITEM_ID]
```

The "Flat" format is typically used for brokers that limit the topic depth (number of forward slash "/" characters). For example, AWS IoT Core.

#### Device Condition Observations
Condition messages are sent as an array of Observations since a Condition may have multiple Fault States. This is similar to how the Current request functions in an HTTP Agent.

### Device Assets
```
MTConnect/Devices/[DEVICE_UUID]/Assets/[ASSET_TYPE]/[ASSET_ID]
```

### Topics

> [Node] = (Payload)

```
- MTConnect
   ─ Devices
      ─ [DEVICE_UUID]
        - Device = (JSON)
        - Observations
          - [DATA_ITEM_ID] = (JSON Array)
          - [DATA_ITEM_ID] = (JSON Array)
          - [DATA_ITEM_ID] = (JSON Array)
        - Assets
          - [ASSET_TYPE]
            - [ASSET_ID] = (JSON)
```

### Example
```
- MTConnect
   ─ Devices
      ─ OKUMA.Lathe.123456
        - Device
        - Observations
          - L2avail = {"dataItemId":"L2avail","name":"avail","type":"AVAILABILITY","timestamp":"2023-02-07T20:02:26.8978653Z","result":"AVAILABLE"}
          - L2estop = {"dataItemId":"L2estop","name":"estop","type":"EMERGENCY_STOP","timestamp":"2023-02-07T20:02:26.8978653Z","result":"ARMED"}
          - L2p1execution = {"dataItemId":"L2p1execution","name":"p1execution","type":"EXECUTION","timestamp":"2023-02-07T20:02:26.7671421Z","result":"UNAVAILABLE"}
          - L2p1system = [{"level":"WARNING","dataItemId":"L2p1system","name":"p1system","type":"SYSTEM","timestamp":"2023-02-07T20:30:16.8639659Z","result":"Not Found","nativeCode":"404"},{"level":"FAULT","dataItemId":"L2p1system","name":"p1system","type":"SYSTEM","timestamp":"2023-02-07T20:30:38.9662297Z","result":"Interval Error","nativeCode":"500"}]         
        - Assets
          - CuttingTool
            - 5.12 = {"assetId":"5.12","type":"CuttingTool","timestamp":"2023-02-07T13:36:04.7288143Z","deviceUuid":"OKUMA.Lathe.123456","serialNumber":"12345678946","toolId":"12","cuttingToolLifeCycle":{"cutterStatus":["AVAILABLE","NEW","MEASURED"],"location":{"type":"SPINDLE"},"programToolGroup":"5","programToolNumber":"12","measurements":[{"type":"FunctionalLength","value":7.6543,"units":"MILLIMETER","code":"LF"},{"type":"CuttingDiameterMax","value":0.375,"units":"MILLIMETER","code":"DC"}]}}
```
