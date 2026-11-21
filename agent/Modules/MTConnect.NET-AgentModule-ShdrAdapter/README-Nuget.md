![MTConnect.NET Logo](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/logo.png) 

# MTConnect SHDR Adapter Agent Module
This Agent Module connects to an SHDR Adapter and reads data from it using the SHDR Protocol

## Configuration
```yaml
modules:
  - shdr-adapter:
      deviceKey: M12346
      hostname: localhost
      port: 7878
```

* `deviceKey` - The UUID or Name of the Device that corresponds to the name of the Device (typically set in the Device XML file). Default is `*`, which reads from the adapter for every Device in the Agent.

* `hostname` - The host the adapter is located on. Default is `localhost`.

* `port` - The port to connect to the adapter. Default is `7878`.

* `heartbeat` - The Heartbeat interval (in milliseconds) that the TCP Connection will use to maintain a connection when no new data has been sent. Default is `5000`

* `connectionTimeout` - The amount of time (in milliseconds) an adapter can be silent before it is disconnected. Default is `600`.

* `reconnectInterval` - The amount of time (in milliseconds) between adapter reconnection attempts. Default is `10000`.

* `allowShdrDevice` - Sets whether a Device Model can be sent from an SHDR Adapter. When `true` and the Agent has no Devices, a connection to the adapter is still created so that the Device can be received. Default is `false`

* `availableOnConnection` - For devices that do not have the ability to provide available events, if TRUE, this sets the Availability to AVAILABLE upon connection. Default is `false`.

* `convertUnits` - Adapter setting for data item units conversion in the agent. When `true` (default), the agent converts the units of the values received from the adapter. Set to `false` if the adapter has already done unit conversion.

* `ignoreObservationCase` - Gets or Sets the default for Ignoring the case of Observation values. Default is `true`

* `ignoreTimestamps` - Overwrite timestamps with the agent time. This will correct clock drift but will not give as accurate relative time since it will not take into consideration network latencies. Default is `false`.

* `outputConnectionInformation` - Gets or Sets whether the Connection Information (Host / Port) is output to the Agent to be collected by a client. Default is `true`

* `ignoreHeartbeatOnChange` - Gets or Sets whether Heartbeat PING requests are not sent if data has been received within the Heartbeat period. Default is TRUE


## Contribution / Feedback
- Please use the [Issues](https://github.com/TrakHound/MTConnect.NET/issues) tab to create issues for specific problems that you may encounter 
- Please feel free to use the [Pull Requests](https://github.com/TrakHound/MTConnect.NET/pulls) tab for any suggested improvements to the source code
- For any other questions or feedback, please contact TrakHound directly at **info@trakhound.com**.

## License
This application and its source code is licensed under the [MIT License](https://choosealicense.com/licenses/mit/) and is free to use.
