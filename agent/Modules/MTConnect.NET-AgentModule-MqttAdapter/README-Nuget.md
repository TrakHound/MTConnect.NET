![MTConnect.NET Logo](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/logo.png) 

# MTConnect MQTT Adapter Agent Module
This Agent Module implements an adapter to read from an MQTT broker

>**Note:** This Module is still under development and may be deprecated in the future. Please feel free to leave feedback or create Issues on GitHub.

## Configuration
```yaml
modules:
  - mqtt-adapter:
      server: localhost
      port: 1883
      topicPrefix: input
      deviceKey: M12346
```

* `server` - The MQTT broker hostname. Default is `localhost`

* `port` - The MQTT broker port number. Default is `1883`

* `timeout` - The timeout (in milliseconds) to use for connection and read/write. Default is `5000`

* `reconnectInterval` - The interval (in milliseconds) to delay between disconnections. Default is `10000`

* `username` - Sets the Username to use for authentication
 
* `password` - Sets the Password to use for authentication
 
* `clientId` - Sets the Client ID to use for the connection. If not set, a Client ID is generated from the Agent UUID and `deviceKey`

* `cleanSession` - Sets the CleanSession flag (true or false). Default is `true`

* `qos` - Sets the Quality Of Service (Qos) to use. 0 = At Most Once, 1 = At least Once, 2 = Exactly Once. Default is `1`

* `certificateAuthority` - The path to the Certificate Authority file. Only used when `pemCertificate` and `pemPrivateKey` are both set

* `pemCertificate` - The path to the PEM Certificate (.pem) file

* `pemPrivateKey` - The path to the PEM Private Key file

* `allowUntrustedCertificates` - Sets whether untrusted certificates are accepted (true or false). When `true`, certificate chain and revocation errors are ignored. Only used when `pemCertificate` and `pemPrivateKey` are both set. Default is `false`

* `useTls` - Sets whether to use TLS or not (true or false)

* `topicPrefix` - The MQTT topic prefix to subscribe to (required). The module subscribes to `[TOPIC_PREFIX]/#`

* `deviceKey` - The UUID or Name of the Device to read data for

* `documentFormat` - The Document Format ID used to read the input data. Default is `json`

## Input Topics

### Observations
```
[TOPIC_PREFIX]/observations
```

### Asset
```
[TOPIC_PREFIX]/assets
```

### Device
```
[TOPIC_PREFIX]/device
```

## Contribution / Feedback
- Please use the [Issues](https://github.com/TrakHound/MTConnect.NET/issues) tab to create issues for specific problems that you may encounter 
- Please feel free to use the [Pull Requests](https://github.com/TrakHound/MTConnect.NET/pulls) tab for any suggested improvements to the source code
- For any other questions or feedback, please contact TrakHound directly at **info@trakhound.com**.

## License
This application and its source code is licensed under the [MIT License](https://choosealicense.com/licenses/mit/) and is free to use.
