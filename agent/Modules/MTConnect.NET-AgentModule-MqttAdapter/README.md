# Payload
```json
[
  {
    "timestamp": "2023-11-18T20:00:00Z",
    "dataItems": {
      "L2avail": "AVAILABLE",
      "L2estop": "ARMED",
      "L2X1load": 12.32
    },
    "messages": {
      "L2message": {
        "nativeCode": "0222",
        "message": "PLC Initializing"
      }
    },
    "conditions": {
      "L2p1system": [
          {
            "level": "WARNING",
            "nativeCode": "404",
            "message": "Not Found"
          }
        ]
    },
    "dataSets": {
      "L2vars": {
        "E100": 123.456,
        "E101": 321
      }
    },
    "tables": {
      "L2toolOffsets": {
        "T1": {
            "Length": 100.12,
            "Diameter": 12.501
          }
      }
    }
  }
]
```