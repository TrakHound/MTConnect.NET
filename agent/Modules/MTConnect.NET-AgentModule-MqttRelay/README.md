![MTConnect.NET Logo](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/mtconnect-net-03-md.png) 

# MTConnect MQTT Relay Module
This Agent Module writes data to an **External** MQTT Broker

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
            <td>MTConnect.NET-AgentModule-MqttRelay</td>
            <td><img src="https://img.shields.io/nuget/dt/MTConnect.NET-AgentModule-MqttRelay?style=for-the-badge&logo=nuget&label=%20&color=%23333"/></td>
            <td><a href="https://www.nuget.org/packages/MTConnect.NET-AgentModule-MqttRelay">https://www.nuget.org/packages/MTConnect.NET-AgentModule-MqttRelay</a></td>
        </tr>
    </tbody>
</table>

## Configuration
```yaml
- mqtt-relay:
    server: localhost
    port: 7878
    topic: enterprise/site/area/line/cell/MTConnect
```

* `server` - The MQTT broker hostname

* `port` - The MQTT broker port number

* `timeout` - The UUID or Name of the Device to read data for

* `reconnectInterval` - The UUID or Name of the Device to read data for

* `username` - Sets the Username to use for authentication
 
* `password` - Sets the Password to use for authentication
 
* `clientId` - Sets the Client ID to use for the connection

* `qos` - Sets the Quality Of Service (QoS) to use. 0 = At Most Once, 1 = At least Once, 2 = Exactly Once

* `certificateAuthority` - The path to the Certificate Authority file

* `pemCertificatePath` - The path to the PEM Certificate (.pem) file

* `pemPrivateKey` - The path to the PEM Private Key file

* `allowUntrustedCertificates` - Sets whether to validate the certificate chain (true or false)

* `useTls` - Sets whether to use TLS or not (true or false)

* `topicPrefix` - The prefix to add to the MQTT topics that are published

* `DocumentFormat` - The Document Format ID to use to format the payload

### AWS IoT Configuration Example
```yaml
- mqtt-relay:
    server: akljadkfjdlsf-ats.iot.us-east-1.amazonaws.com
    port: 8883
    tls:
      pem:
        certificateAuthority: certs/AmazonRootCA1.pem
        certificatePath: certs/asfdslkafjdslkfjdklsdjf-certificate.pem.crt
        privateKeyPath: certs/sdlkajlksdajfldskjfdldlskfjdslkaj-private.pem.key
    documentFormat: json-cppagent
    currentInterval: 5000
    sampleInterval: 500
    topicPrefix: enterprise/site/area/line/cell/MTConnect
```

### AWS Greengrass Moquette Configuration Example
```yaml
- mqtt-relay:
    server: localhost
    port: 8883
    clientId: mtconnect-test # Set the ClientId to the AWS Thing ID
    tls:
      verifyClientCertificate: false
      pem:
        certificateAuthority: certs/AmazonRootCA1.pem
        certificatePath: certs/2316549874654321654984984158961634984794-certificate.pem.crt
        privateKeyPath: certs/2316549874654321654984984158961634984794-private.pem.key
    documentFormat: json-cppagent
    currentInterval: 5000
    sampleInterval: 500
    topicPrefix: enterprise/site/area/line/cell/MTConnect
```

### HiveMQ Configuration Example
```yaml
- mqtt-relay:
    server: 5679887d308d402888f32.s1.eu.hivemq.cloud
    port: 8883
    username: mtconnect
    password: mtconnect
    useTls: true
    documentFormat: json-cppagent
    currentInterval: 5000
    sampleInterval: 500
    topicPrefix: enterprise/site/area/line/cell/MTConnect
```

## Contribution / Feedback
- Please use the [Issues](https://github.com/TrakHound/MTConnect.NET/issues) tab to create issues for specific problems that you may encounter 
- Please feel free to use the [Pull Requests](https://github.com/TrakHound/MTConnect.NET/pulls) tab for any suggested improvements to the source code
- For any other questions or feedback, please contact TrakHound directly at **info@trakhound.com**.

## License
This application and it's source code is licensed under the [MIT License](https://choosealicense.com/licenses/mit/) and is free to use.
