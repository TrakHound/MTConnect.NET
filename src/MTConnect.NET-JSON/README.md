# MTConnect.NET-JSON
MTConnect.NET-JSON is an extension library to MTConnect.NET that provides reading and writing as JSON formatted Response Documents

## Devices (MTConnectDevices)
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

## Observations (MTConnectStreams)
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