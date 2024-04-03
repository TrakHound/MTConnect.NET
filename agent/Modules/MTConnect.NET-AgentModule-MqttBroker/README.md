![MTConnect.NET Logo](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/mtconnect-net-03-md.png) 

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
- mqtt-broker:
    port: 1883
    topicPrefix: MTConnect
    topicStructure: Document
```

* `server` - The MQTT broker hostname to bind to

* `port` - The MQTT broker port number to bind to

* `timeout` - The timeout (in milliseconds) to use for connection and read/write

* `qos` - Sets the Quality Of Service (QoS) to use. 0 = At Most Once, 1 = At least Once, 2 = Exactly Once

* `initialDelay` - The time (in milliseconds) to delay after initial Module start (to allow for TCP binding)

* `restartInterval` - The time (in milliseconds) to delay between server start errors

* `topicPrefix` - The prefix to add to the MQTT topics that are published
 
* `topicStructure` - (Document or Entity) Sets how MQTT topics and messages are stuctured

* `currentInterval` - Sets the Interval (in milliseconds) to send Current messages at

* `sampleInterval` - Sets the Interval (in milliseconds) to send Sample messages at

* `documentFormat` - The Document Format ID to use to format the payload

* `indentOutput` - Sets whether to indent the output in each payload

* `tls` - Sets the TLS settings

    * `pfx` - The PFX certificate settings
        * `certificatePath` - The path to the (.pfx) file
        * `certificatePassword` - The certificate password

    * `pem` - The PEM certificate settings
        * `certificatePath` - The path to the (.pem) file
        * `privateKeyPath` - The path to the key containing the private key
        * `privateKeyPassword` - The certificate password
        * `certificateAuthority` - The path to the (.pem) file containing the Certificate Authority

    * `verifyClientCertificate` - Toggles whether Client Certificate chains are verified ("true" or "false")

## Contribution / Feedback
- Please use the [Issues](https://github.com/TrakHound/MTConnect.NET/issues) tab to create issues for specific problems that you may encounter 
- Please feel free to use the [Pull Requests](https://github.com/TrakHound/MTConnect.NET/pulls) tab for any suggested improvements to the source code
- For any other questions or feedback, please contact TrakHound directly at **info@trakhound.com**.

## License
This application and it's source code is licensed under the [MIT License](https://choosealicense.com/licenses/mit/) and is free to use.
