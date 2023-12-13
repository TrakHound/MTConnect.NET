![MTConnect.NET Logo](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/mtconnect-net-03-md.png) 

# MTConnect MQTT Broker Module
This Agent Module serves data via an **Internal** MQTT Broker

## Configuration
```yaml
- mqtt-broker:
    port: 1883
    topicPrefix: MTConnect
```

* `server` - The MQTT broker hostname to bind to

* `port` - The MQTT broker port number to bind to

* `timeout` - The timeout (in milliseconds) to use for connection and read/write

* `qos` - Sets the Quality Of Service (QoS) to use. 0 = At Most Once, 1 = At least Once, 2 = Exactly Once

* `certificateAuthority` - The path to the Certificate Authority file

* `pemCertificatePath` - The path to the PEM Certificate (.pem) file

* `pemPrivateKey` - The path to the PEM Private Key file

* `allowUntrustedCertificates` - Sets whether to validate the certificate chain (true or false)

* `initialDelay` - The time (in milliseconds) to delay after initial Module start (to allow for TCP binding)

* `restartInterval` - The time (in milliseconds) to delay between server start errors

* `topicPrefix` - The prefix to add to the MQTT topics that are published

* `currentInterval` - Sets the Interval (in milliseconds) to send Current messages at

* `sampleInterval` - Sets the Interval (in milliseconds) to send Sample messages at

* `DocumentFormat` - The Document Format ID to use to format the payload

* `indentOutput` - Sets whether to indent the output in each payload

## Contribution / Feedback
- Please use the [Issues](https://github.com/TrakHound/MTConnect.NET/issues) tab to create issues for specific problems that you may encounter 
- Please feel free to use the [Pull Requests](https://github.com/TrakHound/MTConnect.NET/pulls) tab for any suggested improvements to the source code
- For any other questions or feedback, please contact TrakHound directly at **info@trakhound.com**.

## License
This application and it's source code is licensed under the [MIT License](https://choosealicense.com/licenses/mit/) and is free to use.
