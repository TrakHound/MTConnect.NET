![MTConnect.NET Logo](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/mtconnect-net-03-md.png) 

# MTConnect.NET-MQTT
MTConnect.NET-MQTT is an extension library to MTConnect.NET that provides an MQTT Broker & Client interface to an IMTConnectAgentBroker interface.

> Updated for Version 6

# Topic Structures
`Document` - Topics are the same as HTTP (Probe, Current, Sample, & Assets). Payloads are the corresponding MTConnect Response Documents. This provides a simple protocol that is performance based for applications with high frequency updates.

`Entity` - Topics are expanded where each Entity has it's own topic. This provides an easy to read interface for tools such as NodeRed, etc.


## Document Topic Structure


## Entity Topic Structure



## Devices
The **MTConnect/Devices** topics are used to send data that is in an MTConnectDevicesResponse document.

```
MTConnect/Devices/[DEVICE_UUID]/Device
```

### Device Observations
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

### Topic Structure (MTConnectMqttFormat.Flat)

> [Node] = (Payload)

```bash
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
```bash
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

### Topic Structure (MTConnectMqttFormat.Hierarchy)

> [Node] = (Payload)

```bash
- MTConnect
   ─ Devices
      ─ [DEVICE_UUID]
        - Device = (JSON)
        - Observations
          - [COMPONENT_TYPE]
            - [COMPONENT_ID]
              - Events
                - [DATA_ITEM_TYPE]
                  - [DATA_ITEM_ID] = (JSON)
                  - SubTypes
                    - [DATA_ITEM_SUBTYPE]
                      - [DATA_ITEM_ID] = (JSON)
              - Samples
                - [DATA_ITEM_TYPE]
                  - [DATA_ITEM_ID] = (JSON)
              - Conditions
                - [DATA_ITEM_TYPE]
                  - [DATA_ITEM_ID] = (JSON Array)
        - Assets
          - [ASSET_TYPE]
            - [ASSET_ID] = (JSON)
```

### Example
```bash
- MTConnect
   ─ Devices
      ─ OKUMA.Lathe.123456
        - Device
        - Observations
          - Device
            - OKUMA.Lathe
              - Events
                - AVAILABILITY
                  - L2avail = {"dataItemId":"L2avail","name":"avail","type":"AVAILABILITY","timestamp":"2023-02-07T20:02:26.8978653Z","result":"AVAILABLE"}
                - ASSET_CHANGED
                  - OKUMA.Lathe_assetCount = {"dataItemId":"OKUMA.Lathe_assetCount","name":"assetCount","type":"ASSET_COUNT","timestamp":"2023-02-07T20:02:26.7671421Z","result":"UNAVAILABLE","count":0}
          - Controller
            - L2ct1
              - Events
                - EMERGENCY_STOP
                  - L2estop = {"dataItemId":"L2estop","name":"estop","type":"EMERGENCY_STOP","timestamp":"2023-02-07T20:02:26.8978653Z","result":"ARMED"}
          - Path
            - L2p1
              - Events
                - EXECUTION
                  - L2p1execution = {"dataItemId":"L2p1execution","name":"p1execution","type":"EXECUTION","timestamp":"2023-02-07T20:02:26.7671421Z","result":"UNAVAILABLE"}
              - Conditions
                - SYSTEM
                  - L2p1system = [{"level":"WARNING","dataItemId":"L2p1system","name":"p1system","type":"SYSTEM","timestamp":"2023-02-07T20:30:16.8639659Z","result":"Not Found","nativeCode":"404"},{"level":"FAULT","dataItemId":"L2p1system","name":"p1system","type":"SYSTEM","timestamp":"2023-02-07T20:30:38.9662297Z","result":"Interval Error","nativeCode":"500"}]
              - Samples
                - PATH_FEEDRATE
                  - SubTypes
                    - ACTUAL
                      - L2p1Fact = {"dataItemId":"L2p1Fact","name":"p1Fact","type":"PATH_FEEDRATE","subType":"ACTUAL","timestamp":"2023-02-07T20:02:26.7671421Z","result":"UNAVAILABLE"}
        - Assets
          - CuttingTool
            - 5.12 = {"assetId":"5.12","type":"CuttingTool","timestamp":"2023-02-07T13:36:04.7288143Z","deviceUuid":"OKUMA.Lathe.123456","serialNumber":"12345678946","toolId":"12","cuttingToolLifeCycle":{"cutterStatus":["AVAILABLE","NEW","MEASURED"],"location":{"type":"SPINDLE"},"programToolGroup":"5","programToolNumber":"12","measurements":[{"type":"FunctionalLength","value":7.6543,"units":"MILLIMETER","code":"LF"},{"type":"CuttingDiameterMax","value":0.375,"units":"MILLIMETER","code":"DC"}]}}
```

## Assets
```
MTConnect/Assets/[ASSET_TYPE]/[ASSET_ID]
```
> Note: Assets are sent to two topics. One for the "Global" assets and one for the Device that the Asset was added to

### Topic Structure

> [Node] = (Payload)

```bash
- MTConnect
   ─ Assets
      ─ [ASSET_TYPE]
        - [ASSET_ID] = (JSON)
```

### Example
```bash
- MTConnect
  - Assets
      - CuttingTool
      - 5.12 = {"assetId":"5.12","type":"CuttingTool","timestamp":"2023-02-07T13:36:04.7288143Z","deviceUuid":"OKUMA.Lathe.123456","serialNumber":"12345678946","toolId":"12","cuttingToolLifeCycle":{"cutterStatus":["AVAILABLE","NEW","MEASURED"],"location":{"type":"SPINDLE"},"programToolGroup":"5","programToolNumber":"12","measurements":[{"type":"FunctionalLength","value":7.6543,"units":"MILLIMETER","code":"LF"},{"type":"CuttingDiameterMax","value":0.375,"units":"MILLIMETER","code":"DC"}]}}
```

## More Information
More information can be found in the MQTT-Protocol document (https://github.com/TrakHound/MTConnect.NET/blob/master/docs/MQTT-Protocol.md).