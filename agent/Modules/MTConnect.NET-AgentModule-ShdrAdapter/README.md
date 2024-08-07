![MTConnect.NET Logo](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/mtconnect-net-03-md.png) 

# MTConnect SHDR Adapter Agent Module
This Agent Module implements the MTConnect REST Protocol

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
            <td>MTConnect.NET-AgentModule-ShdrAdapter</td>
            <td><img src="https://img.shields.io/nuget/dt/MTConnect.NET-AgentModule-ShdrAdapter?style=for-the-badge&logo=nuget&label=%20&color=%23333"/></td>
            <td><a href="https://www.nuget.org/packages/MTConnect.NET-AgentModule-ShdrAdapter">https://www.nuget.org/packages/MTConnect.NET-AgentModule-ShdrAdapter</a></td>
        </tr>
    </tbody>
</table>

## Configuration
```yaml
modules:
  - shdr-adapter:
      deviceKey: M12346
      hostname: localhost
      port: 7878
```

* `deviceKey` - The UUID or Name of the Device that corresponds to the name of the Device (typically set in the Device XML file).

* `hostname` - The host the adapter is located on.

* `port` - The port to connect to the adapter.

* `heartbeat` - The Heartbeat interval (in milliseconds) that the TCP Connection will use to maintain a connection when no new data has been sent

* `connectionTimeout` - The amount of time (in milliseconds) an adapter can be silent before it is disconnected.

* `reconnectInterval` - The amount of time (in milliseconds) between adapter reconnection attempts.

* `allowShdrDevice` - Sets whether a Device Model can be sent from an SHDR Adapter

* `availableOnConnection` - For devices that do not have the ability to provide available events, if TRUE, this sets the Availability to AVAILABLE upon connection.

* `convertUnits` - Adapter setting for data item units conversion in the agent. Assumes the adapter has already done unit conversion. Defaults to global.

* `ignoreObservationCase` - Gets or Sets the default for Ignoring the case of Observation values

* `ignoreTimestamps` - Overwrite timestamps with the agent time. This will correct clock drift but will not give as accurate relative time since it will not take into consideration network latencies. This can be overridden on a per adapter basis.

* `outputConnectionInformation` - Gets or Sets whether the Connection Information (Host / Port) is output to the Agent to be collected by a client

* `ignoreHeartbeatOnChange` - Gets or Sets whether Heartbeat PING requests are not sent if data has been received within the Heartbeat period. Default is TRUE


## Contribution / Feedback
- Please use the [Issues](https://github.com/TrakHound/MTConnect.NET/issues) tab to create issues for specific problems that you may encounter 
- Please feel free to use the [Pull Requests](https://github.com/TrakHound/MTConnect.NET/pulls) tab for any suggested improvements to the source code
- For any other questions or feedback, please contact TrakHound directly at **info@trakhound.com**.

## License
This application and it's source code is licensed under the [MIT License](https://choosealicense.com/licenses/mit/) and is free to use.
