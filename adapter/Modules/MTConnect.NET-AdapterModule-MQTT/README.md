![MTConnect.NET Logo](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/logo.png) 

# MTConnect MQTT Adapter Module
This Adapter Module sends input data to an MQTT Broker that can be read by an MTConnect Agent

>**Note:** This Module is still under development and may be deprecated in the future. Please feel free to leave feedback or create Issues on GitHub.

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
            <td>MTConnect.NET-AdapterModule-MQTT</td>
            <td><img src="https://img.shields.io/nuget/dt/MTConnect.NET-AdapterModule-MQTT?style=for-the-badge&logo=nuget&label=%20&color=%23333"/></td>
            <td><a href="https://www.nuget.org/packages/MTConnect.NET-AdapterModule-MQTT">https://www.nuget.org/packages/MTConnect.NET-AdapterModule-MQTT</a></td>
        </tr>
    </tbody>
</table>

## Configuration
```yaml
modules:
  - mqtt:
      port: 1883
      server: localhost
      topic: MTConnect/Input
```

* `port` - The MQTT broker port number to publish to. Default is `7878`, so set this to the port of your broker (ex. `1883`).

* `server` - The MQTT broker hostname or IP address. Default is `localhost`
 
* `topic` - The MQTT topic prefix to publish to. There is no default, so this should always be set

* `username` - Sets the Username to use for authentication

* `password` - Sets the Password to use for authentication

* `clientId` - Sets the Client ID to use for the connection

* `qos` - Sets the Quality Of Service (Qos) to use. 0 = At Most Once, 1 = At least Once, 2 = Exactly Once. Default is `0`

* `useTls` - Sets whether to use TLS or not (true or false)

* `pemCertificate` - The path to the PEM Certificate (.pem) file

* `pemPrivateKey` - The path to the PEM Private Key file

* `certificateAuthority` - The path to the Certificate Authority file. Only used when `pemCertificate` and `pemPrivateKey` are both set

* `allowUntrustedCertificates` - Sets whether untrusted certificates are accepted (true or false). When `true`, certificate chain and revocation errors are ignored. Only used when `pemCertificate` and `pemPrivateKey` are both set. Default is `false`

* `reconnectInterval` - The interval (in milliseconds) to delay between disconnections. Default is `10000`

* `documentFormat` - The Document Format ID to use to format the payload. Default is `json`

## Payload
Payloads are formatted using the configured `documentFormat` and are not compressed. All messages are retained. The topics below are used:

### Assets
```
[TOPIC]/assets
```

### Device
```
[TOPIC]/device
```

### Observations
```
[TOPIC]/observations
```

Observations using the `json` Document Format follow the format below:

```json
[
	{
		"timestamp": "2023-12-06T23:15:01.7754921Z",
		"observations": [
			{
				"dataItem": "avail",
				"values": {
					"result": "AVAILABLE"
				}
			},
			{
				"dataItem": "estop",
				"values": {
					"result": "ARMED"
				}
			},
			{
				"dataItem": "system",
				"values": {
					"level": "WARNING",
					"nativeCode": "404"
				}
			},
			{
				"dataItem": "system",
				"values": {
					"level": "WARNING",
					"nativeCode": "405"
				}
			}
		]
	},
	{
		"timestamp": "2023-12-06T23:16:45.456725Z",
		"observations": [
			{
				"dataItem": "system",
				"values": {
					"level": "NORMAL"
				}
			},
			{
				"dataItem": "processTimer",
				"values": {
					"result": "12",
					"resetTriggered": "SHIFT"
				}
			},
			{
				"dataItem": "vars",
				"values": {
					"DATASET[E100]": "12.123",
					"DATASET[E101]": "6574"
				}
			},
			{
				"dataItem": "toolTable",
				"values": {
					"TABLE[T1][LENGTH]": "142.654",
					"TABLE[T1][DIAMETER]": "12.496",
					"TABLE[T2][LENGTH]": "135.611",
					"TABLE[T2][DIAMETER]": "5.980"
				}
			}
		]
	}
]
```


## Contribution / Feedback
- Please use the [Issues](https://github.com/TrakHound/MTConnect.NET/issues) tab to create issues for specific problems that you may encounter 
- Please feel free to use the [Pull Requests](https://github.com/TrakHound/MTConnect.NET/pulls) tab for any suggested improvements to the source code
- For any other questions or feedback, please contact TrakHound directly at **info@trakhound.com**.

## License
This application and its source code is licensed under the [MIT License](https://choosealicense.com/licenses/mit/) and is free to use.
