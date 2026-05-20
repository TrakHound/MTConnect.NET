# AWS Greengrass MQTT Bridge

## Overview
- AWS Greengrass Core Setup
  - [Getting Started](https://docs.aws.amazon.com/greengrass/v1/developerguide/install-ggc.html)
- AWS Greengrass MQTT Bridge Setup
  - [MQTT bridge](https://docs.aws.amazon.com/greengrass/v2/developerguide/mqtt-bridge-component.html)

## AWS Greengrass Moquette Configuration
Under the Deployment configuration for the `aws.greengrass.clientdevices.mqtt.Bridge` component
```json
"mqttTopicMapping": {
  "MTConnectIotCoreMapping": {
    "topic": "MTConnect/#",
    "targetTopicPrefix": "trakhound-test/",
    "source": "LocalMqtt",
    "target": "IotCore"
  }
}
```

## Setup AWS IoT Core
https://github.com/TrakHound/MTConnect.NET/blob/master/docs/MQTT-AWS-IoT.md

## Setup AWS Greengrass Moquette MQTT Broker
https://github.com/TrakHound/MTConnect.NET/blob/master/docs/MQTT-AWS-Greengrass-Moquette.md
