![MTConnect.NET Logo](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/logo.png) 

# MTConnect MQTT Broker Module
This Agent Module serves data via an **Internal** MQTT Broker

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
            <td>MTConnect.NET-AgentModule-MqttBroker</td>
            <td><img src="https://img.shields.io/nuget/dt/MTConnect.NET-AgentModule-MqttBroker?style=for-the-badge&logo=nuget&label=%20&color=%23333"/></td>
            <td><a href="https://www.nuget.org/packages/MTConnect.NET-AgentModule-MqttBroker">https://www.nuget.org/packages/MTConnect.NET-AgentModule-MqttBroker</a></td>
        </tr>
    </tbody>
</table>

## Configuration
```yaml
modules:
  - mqtt-broker:
      port: 1883
      topicPrefix: MTConnect
      topicStructure: Document
```

* `server` - The MQTT broker hostname to bind to. If not set, the broker listens on all addresses

* `port` - The MQTT broker port number to bind to. Default is `1883`

* `timeout` - The timeout (in milliseconds) to use for connection and read/write. Default is `5000`

* `qos` - Sets the Quality Of Service (Qos) to use. 0 = At Most Once, 1 = At least Once, 2 = Exactly Once. Only applies when `topicStructure` is `Entity`. When `topicStructure` is `Document`, messages are published with QoS 1 (At least Once)

* `initialDelay` - The maximum time (in milliseconds) to wait for the broker to start when the Module starts (to allow for TCP binding). Default is `500`

* `restartInterval` - The time (in milliseconds) to delay between server start errors. Default is `5000`

* `topicPrefix` - The prefix to add to the MQTT topics that are published. Default is `MTConnect`
 
* `topicStructure` - (Document or Entity) Sets how MQTT topics and messages are structured. Default is `Document`

* `currentInterval` - Sets the Interval (in milliseconds) to send Current messages at. Default is `5000`

* `sampleInterval` - Sets the Interval (in milliseconds) to send Sample messages at. Default is `500`

* `documentFormat` - The Document Format ID to use to format the payload. Default is `json-cppagent`

* `indentOutput` - Sets whether to indent the output in Asset payloads. Default is `false`

* `tls` - Sets the TLS settings. When set, the broker listens for TLS connections on `port` using the certificate below

    * `pfx` - The PFX certificate settings
        * `certificatePath` - The path to the (.pfx) file
        * `certificatePassword` - The certificate password

    * `pem` - The PEM certificate settings
        * `certificatePath` - The path to the (.pem) file
        * `privateKeyPath` - The path to the key containing the private key
        * `privateKeyPassword` - The certificate password

## Topics
When `topicStructure` is `Document`, the following topics are published. All messages are retained.

```
[TOPIC_PREFIX]/Probe/[DEVICE_UUID]
[TOPIC_PREFIX]/Current/[DEVICE_UUID]
[TOPIC_PREFIX]/Sample/[DEVICE_UUID]
[TOPIC_PREFIX]/Asset/[DEVICE_UUID]/[ASSET_ID]
```

## Contribution / Feedback
- Please use the [Issues](https://github.com/TrakHound/MTConnect.NET/issues) tab to create issues for specific problems that you may encounter 
- Please feel free to use the [Pull Requests](https://github.com/TrakHound/MTConnect.NET/pulls) tab for any suggested improvements to the source code
- For any other questions or feedback, please contact TrakHound directly at **info@trakhound.com**.

## License
This application and its source code is licensed under the [MIT License](https://choosealicense.com/licenses/mit/) and is free to use.
