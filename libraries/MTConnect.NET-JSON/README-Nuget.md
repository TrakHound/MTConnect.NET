![MTConnect.NET Logo](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/mtconnect-net-03-md.png) 

# MTConnect.NET-JSON
MTConnect.NET-JSON is an extension library to MTConnect.NET that provides reading and writing as JSON formatted Response Documents

## Document Format ID
```
JSON
```
The above document format ID can be used to specify a ResponseDocumentFormatter or EntityFormatter to output using this library

## Devices
MTConnectDevicesResponse documents are formatted as shown below.

### Examples
- [Devices](Devices/Examples/Devices.json)
- [DataItem](Devices/Examples/DataItem.json)
- [Configuration](Devices/Examples/Configuration.json)
- [DataItemDefinition](Devices/Examples/DataItemDefinition.json)
- [Filter](Devices/Examples/Filter.json)
- [Constraints](Devices/Examples/Constraints.json)

### Example
```json
{
  "header": {
    "instanceId": 1675796631,
    "version": "5.0.0.0",
    "sender": "DESKTOP-HV74M4N",
    "bufferSize": 150000,
    "assetBufferSize": 1000,
    "assetCount": 2,
    "deviceModelChangeTime": "2023-02-07T19:03:51.4276271Z",
    "creationTime": "2023-02-07T20:59:56.063869Z"
  },
  "devices": [
    {
      "id": "OKUMA.Lathe",
      "type": "Device",
      "name": "OKUMA-Lathe",
      "uuid": "OKUMA.Lathe.123456",
      "description": {
        "manufacturer": "OKUMA",
        "model": "LB3000",
        "serialNumber": "3216549771",
        "value": "Okuma MT Connect Adapter - Lathe"
      },
      "dataItems": [
        {
          "category": "EVENT",
          "id": "L2avail",
          "type": "AVAILABILITY",
          "name": "avail"
        }
      ],
      "components": [
        {
          "id": "L2ct1",
          "type": "Controller",
          "name": "Controller",
          "dataItems": [
            {
              "category": "EVENT",
              "id": "L2estop",
              "type": "EMERGENCY_STOP",
              "name": "estop"
            }
          ],
          "components": [
            {
              "id": "L2p1",
              "type": "Path",
              "name": "path",
              "dataItems": [
                {
                  "category": "EVENT",
                  "id": "L2p1mode",
                  "type": "CONTROLLER_MODE",
                  "name": "p1mode"
                }
              ]
            }
          ]
        }
      ]
    }
  ]
}
```

## Observations
MTConnectStreamsResponse documents are formatted as shown below.

### Examples
- [Streams](Streams/Examples/Streams.json)
- [ValueObservation](Streams/Examples/ValueObservation.json)
- [ConditionObservation](Streams/Examples/ConditionObservation.json)
- [DataSetObservation](Streams/Examples/DataSetObservation.json)
- [TableObservation](Streams/Examples/TableObservation.json)
- [TimeSeriesObservation](Streams/Examples/TimeSeriesObservation.json)

### Example
```json
{
  "header": {
    "InstanceId": 1675796631,
    "Version": "5.0.0.0",
    "Sender": "DESKTOP-HV74M4N",
    "BufferSize": 150000,
    "FirstSequence": 1,
    "LastSequence": 138,
    "NextSequence": 139,
    "DeviceModelChangeTime": "2023-02-07T19:03:51.4276271Z",
    "CreationTime": "2023-02-07T20:54:26.7903092Z"
  },
  "streams": [
    {
      "name": "OKUMA-Lathe",
      "uuid": "OKUMA.Lathe.123456",
      "componentStream": [
        {
          "component": "Path",
          "componentId": "L2p1",
          "name": "path",
          "samples": [
            {
              "dataItemId": "L2p1Fact",
              "name": "p1Fact",
              "type": "PATH_FEEDRATE",
              "subType": "ACTUAL",
              "timestamp": "2023-02-07T19:03:51.4064826Z",
              "sequence": 38,
              "result": "UNAVAILABLE"
            },
          ],
          "events": [
            {
              "dataItemId": "L2p1mode",
              "name": "p1mode",
              "type": "CONTROLLER_MODE",
              "timestamp": "2023-02-07T20:58:08.739602Z",
              "sequence": 139,
              "result": "AUTOMATIC"
            }
          ],
          "condition": [
            {
              "level": "WARNING",
              "dataItemId": "L2p1system",
              "name": "p1system",
              "type": "SYSTEM",
              "timestamp": "2023-02-07T20:30:16.8639659Z",
              "sequence": 137,
              "result": "Not Found",
              "nativeCode": "404"
            },
            {
              "level": "FAULT",
              "dataItemId": "L2p1system",
              "name": "p1system",
              "type": "SYSTEM",
              "timestamp": "2023-02-07T20:30:38.9662297Z",
              "sequence": 138,
              "result": "Interval Error",
              "nativeCode": "500"
            }
          ]
        }
      ]
    }
  ]
{
```

## Assets
MTConnectAssetsResponse documents are formatted as shown below.

### Examples
- [CuttingTool](Assets/Examples/CuttingTool.json)
- [File](Assets/Examples/File.json)

### Example
```json
{
  "header": {
    "InstanceId": 1675796631,
    "Version": "5.0.0.0",
    "Sender": "DESKTOP-HV74M4N",
    "AssetBufferSize": 1000,
    "AssetCount": 2,
    "DeviceModelChangeTime": "2023-02-07T19:03:51.4276271Z",
    "CreationTime": "2023-02-07T21:07:46.4658671Z"
  },
  "assets": [
    {
      "assetId": "tool.2",
      "type": "CuttingTool",
      "timestamp": "2023-02-07T19:04:19.8594137Z",
      "deviceUuid": "OKUMA.Lathe.123456",
      "serialNumber": "1",
      "toolId": "KSSP300R4SD43L240",
      "manufacturers": "KMT,Parlec",
      "cuttingToolLifeCycle": {
        "cutterStatus": [
          "NEW"
        ],
        "toolLife": {
          "type": "PART_COUNT",
          "countDirection": "UP",
          "limit": 10
        },
        "programToolNumber": "1",
        "connectionCodeMachineSide": "CV50",
        "measurements": [
          {
            "type": "BodyDiameterMax",
            "value": 73.25,
            "units": "MILLIMETER",
            "code": "BDX"
          },
          {
            "type": "OverallToolLength",
            "value": 323.86,
            "units": "MILLIMETER",
            "code": "OAL",
            "maximum": 324.104,
            "minimum": 323.596,
            "nominal": 323.85
          },
          {
            "type": "UsableLengthMax",
            "value": 82.55,
            "units": "MILLIMETER",
            "code": "LUX",
            "nominal": 82.55
          },
          {
            "type": "CuttingDiameterMax",
            "value": 76.262,
            "units": "MILLIMETER",
            "code": "DC",
            "maximum": 76.213,
            "minimum": 76.187,
            "nominal": 76.2
          },
          {
            "type": "BodyLengthMax",
            "value": 222.259,
            "units": "MILLIMETER",
            "code": "LBX",
            "maximum": 222.504,
            "minimum": 222.004,
            "nominal": 222.25
          },
          {
            "type": "DepthOfCutMax",
            "value": 62.383,
            "units": "MILLIMETER",
            "code": "APMX",
            "nominal": 60.96
          }
        ]
      }
    },
    {
      "assetId": "file.patrick2",
      "type": "File",
      "timestamp": "2023-02-07T21:11:21.3576018Z",
      "deviceUuid": "OKUMA.Lathe.123456",
      "name": "file-123.txt",
      "mediaType": "text/plain",
      "applicationCategory": "DEVICE",
      "applicationType": "DATA",
      "size": 123456,
      "versionId": "test-v1",
      "state": "PRODUCTION",
      "fileLocation": {
        "href": "C:\\temp\\file-123.txt"
      },
      "creationTime": "2022-06-16T14:23:46.4347597Z"
    }
  ]
}
```