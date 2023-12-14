![MTConnect.NET Logo](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/mtconnect-net-03-md.png) 

# MTConnect MQTT Adapter Module
This Adapter Module sends input data to an MQTT Broker that can be read by an MTConnect Agent

>This Module is still under development. Please feel free to leave feedback or create Issues on GitHub.

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
- mqtt:
    port: 1883
    server: localhost
    topic: MTConnect/Input
```

* `port` - The port number to read from.

* `server` - The server hostname or IP address
 
* `topic` - The MQTT topic to publish to

## Payload
All Payloads are sent compressed using **gzip** and follow the format below:

### Observations
```
[TOPIC]/Observations
```

```json
[
	{
		"timestamp": "2023-12-06T23:15:01.7754921Z",
		"observations": [
			{
				"dataItemKey": "avail",
				"values": {
					"result": "AVAILABLE"
				}
			},
			{
				"dataItemKey": "estop",
				"values": {
					"result": "ARMED"
				}
			},
			{
				"dataItemKey": "system",
				"values": {
					"level": "WARNING",
					"nativeCode": "404"
				}
			},
			{
				"dataItemKey": "system",
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
				"dataItemKey": "system",
				"values": {
					"level": "NORMAL"
				}
			},
			{
				"dataItemKey": "processTimer",
				"values": {
					"result": "12",
					"resetTriggered": "SHIFT"
				}
			},
			{
				"dataItemKey": "vars",
				"values": {
					"DATASET[E100]": "12.123",
					"DATASET[E101]": "6574"
				}
			},
			{
				"dataItemKey": "toolTable",
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
This application and it's source code is licensed under the [MIT License](https://choosealicense.com/licenses/mit/) and is free to use.
