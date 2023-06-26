# MQTT Protocol for MTConnect

## Overview
This is a protocol for accessing MTConnect data using MQTT that mimics the functionality of the MTConnect HTTP REST protocol. 
The main difference between the HTTP and MQTT protocols is that the MQTT protocol deals with the individual MTConnect entities directly (ex. Device, Observation, Asset).

#### All Devices
![All_Devices](../img/mtconnect-mqtt-protocol-all-01.png)


#### By Device
![All_Devices](../img/mtconnect-mqtt-protocol-by-device-01.png)


### Multiple Configurable Observation Intervals
This protocol enables multiple intervals that Observations can be received. 
This interval essentially implements a window that a maximum of one Observation per DataItem will be published. 
This can be used by client applications such as Dashboards. An interval of "0" will indicate that all observation changes are published.

#### Observation Topics
The interval (in milliseconds) is specified in the brackets suffix for an Observations topic following the pattern below:

##### Observation Interval 1000ms
```
MTConnect/Devices/ff8fc5c5-206f-4d94-96a8-a03c70b682b8/Observations[1000]/#
```

##### Observation Interval 100ms
```
MTConnect/Devices/ff8fc5c5-206f-4d94-96a8-a03c70b682b8/Observations[100]/#
```

##### Observation Interval 0 ("Realtime")
```
MTConnect/Devices/ff8fc5c5-206f-4d94-96a8-a03c70b682b8/Observations/#
```


### Agent Connection Heartbeat
A connection heartbeat that represents the connection between the MTConnect Agent and the MQTT Broker can be read from the topic below:
```
MTConnect/Agents/[AGENT_UUID]/HeartbeatTimestamp
```

The payload is a simple 64 bit Integer representing the last timestamp (in Unix milliseconds) that the Agent sent a heartbeat.

Using this timestamp in combination with the HeartbeatInterval property found in the **MTConnect/Agents/[AGENT_UUID]/Information** topic, 
the connection status of the MTConnect Agent can be determined. 
Typically waiting for 3 failed heartbeats allows for temporary connection interruptions and follows a similar pattern of other MTConnect related heartbeat protocols.


### Entity level Agent InstanceId support
Each MTConnect entity contains an InstanceId property which can be compared to the **InstanceId** property found in the **MTConnect/Agents/[AGENT_UUID]/Information**.
If the entity's InstanceId property differs, then it can be assumed that the Agent was restarted, reconfigured, etc. and the protocol should be restarted.
This is similar to the MTConnect HTTP protocol.

## MQTT Topics

### Agents
Each MTConnect Agent upon connection to the MQTT Broker publishes to the 2 topics listed below:

#### Information
Topic Format:
```
MTConnect/Agents/[AGENT_UUID]/Information
```

Example 1:
```
MTConnect/Agents/d7e169c5-14bb-48a3-bf9f-521152df2c84/Information
```

The Information topic contains information about the MTConnect Agent that would typically be contained in the Header node in an MTConnect XML Response using HTTP.
In addition to the standard MTConnect information, the Information payload also contains the configured "heartbeatInterval", "observationIntervals", and a list of the Device UUIDs that are published by this Agent.

Example Payload:
```json
{
  "uuid": "d7e169c5-14bb-48a3-bf9f-521152df2c84",
  "instanceId": 1687791189,
  "sender": "DESKTOP-HV74M4N",
  "version": "5.4.0.0",
  "deviceModelChangeTime": "2023-06-26T14:53:11.1293232Z",
  "heartbeatInterval": 1000,
  "observationIntervals": [
    0,
    1000
  ],
  "devices": [
    "d7e169c5-14bb-48a3-bf9f-521152df2c84",
    "5fd88408-7811-3c6b-5400-11f4026b6890",
    "OKUMA.Lathe.123456"
  ]
}
```

#### Heartbeat
Topic Format:
```
MTConnect/Agents/[AGENT_UUID]/Heartbeat
```

Example 1:
```
MTConnect/Agents/d7e169c5-14bb-48a3-bf9f-521152df2c84/Heartbeat
```

The Heartbeat topic contains the Unix Timestamp (in milliseconds) of the last time the MTConnect Agent sent a heartbeat message to the MQTT Broker.
This is used together with the "heartbeatInterval" in the Information topic to detect if the MTConnect Agent is still connected to the MQTT Broker.

Example Payload:
```
1687796168175
```

### Devices
The Devices topics contain the data sent by the MTConnect Agent for each Device that the Agent is configured for. Each Device uses its configured UUID as the identifier.

Topic Format:
```
MTConnect/Devices/[DEVICE_UUID]
```

Example 1:
```
MTConnect/Devices/5fd88408-7811-3c6b-5400-11f4026b6890
```

The topics under each Device are listed below:

#### AgentUuid
The AgentUuid topic is used to contain the UUID of the Agent that is publishing for this Device. This creates a link back to the Agent in order to retrieve information about the Agent.

Topic Format:
```
MTConnect/Devices/[DEVICE_UUID]/AgentUuid
```

Example 1:
```
MTConnect/Devices/5fd88408-7811-3c6b-5400-11f4026b6890/AgentUuid
```

Example Payload:
```
d7e169c5-14bb-48a3-bf9f-521152df2c84
```

#### Device
The Device topic contains a JSON representation of the MTConnect Device Metadata that is typically returned from an HTTP Probe request.

Topic Format:
```
MTConnect/Devices/[DEVICE_UUID]/Device
```

Example 1:
```
MTConnect/Devices/5fd88408-7811-3c6b-5400-11f4026b6890/Device
```

Example Payload:
```
{
  "id": "d1",
  "type": "Device",
  "name": "M12346",
  "uuid": "5fd88408-7811-3c6b-5400-11f4026b6890",
  "instanceId": 1687791189,
  "description": {
    "manufacturer": "Mazak_Corporation",
    "serialNumber": "304141",
    "value": "Mill w/SMooth-G"
  },
  "dataItems": [
    {
      "category": "EVENT",
      "id": "avail",
      "type": "AVAILABILITY"
    },
    
    ...
```
> More information about the JSON representation can be found (https://github.com/TrakHound/MTConnect.NET/tree/master/src/MTConnect.NET-JSON)

#### Observations
The Observations topic(s) contain the MTConnect Observation data typically returned from either an HTTP Current or Sample request.
Observation topics may have a "[xxxx]" suffix that denotes the interval (in milliseconds) that the topics are updated at. This is optional and is used to help filter high-frequency data based on the client requirements for the data.
These topics map directly to the "observationIntervals" in the MTConnect/Agents/[AGENT_UUID/Information topic. This functionality is used to mimic the MTConnect HTTP protocol's Current request using the "Interval" query parameter.

If an Interval of 0 is specified, the topic does not contain the "[xxxx]" suffix and is updated as fast as the Agent is receiving new data. This should be used when a client needs to collect All changes and mimics the functionality of the MTConnect HTTP protocol's Sample request using the "Interval = 0" query parameter.

> More information about the topic structure of Observations can be found at (https://github.com/TrakHound/MTConnect.NET/tree/master/src/MTConnect.NET-MQTT)

Topic Format:
```
MTConnect/Devices/[DEVICE_UUID]/Observations[INTERVAL]
```

Example 1: Observations with no interval specified
```
MTConnect/Devices/5fd88408-7811-3c6b-5400-11f4026b6890/Observations
```

Example 2: Observations at an 1000ms interval
```
MTConnect/Devices/5fd88408-7811-3c6b-5400-11f4026b6890/Observations[1000]
```

Example 2: Observations at an 250ms interval
```
MTConnect/Devices/5fd88408-7811-3c6b-5400-11f4026b6890/Observations[250]
```

Example 2: Observations at an 5000ms interval
```
MTConnect/Devices/5fd88408-7811-3c6b-5400-11f4026b6890/Observations[5000]
```

#### Assets
The Assets topic(s) contain the MTConnect Asset data typically retured from an HTTP Assets request for a specific Device.

Topic Format:
```
MTConnect/Devices/[DEVICE_UUID]/Assets/[ASSET_TYPE]/[ASSET_ID]
```

Example 1:
```
MTConnect/Devices/5fd88408-7811-3c6b-5400-11f4026b6890/Assets/CuttingTool/15010006.324
```

Example Payload:
```
{
  "assetId": "15010006.324",
  "type": "CuttingTool",
  "timestamp": "2023-06-26T16:44:20.6839267Z",
  "instanceId": 1687791189,
  "deviceUuid": "5fd88408-7811-3c6b-5400-11f4026b6890",
  "serialNumber": "324",
  "toolId": "15010006",
  "cuttingToolLifeCycle": {
    "cutterStatus": [
      "AVAILABLE",
      "NEW",
      "MEASURED"
    ],
    "location": {
      "type": "SPINDLE"
    },
    "programToolGroup": "5",
    "programToolNumber": "12",
    "measurements": [
      {
        "type": "FunctionalLength",
        "value": 7.6543,
        "units": "MILLIMETER",
        "code": "LF"
      },
      {
        "type": "CuttingDiameterMax",
        "value": 0.375,
        "units": "MILLIMETER",
        "code": "DC"
      }
    ]
  }
}
```

> More information about the JSON representation can be found (https://github.com/TrakHound/MTConnect.NET/tree/master/src/MTConnect.NET-JSON)

