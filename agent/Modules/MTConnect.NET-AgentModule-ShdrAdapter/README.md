![MTConnect.NET Logo](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/dev/img/mtconnect-net-03-md.png) 

# MTConnect SHDR Adapter Agent Module

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

## Description
This Agent Module implements the MTConnect REST Protocol

## Configuration
```yaml
- shdr-adapter:
    deviceKey: M12346
    hostname: localhost
    port: 7878
```

* `allowShdrDevice` - Sets whether a Device Model can be sent from an SHDR Adapter

* `preserveUuid` - Do not overwrite the UUID with the UUID from the adapter, preserve the UUID for the Device. This can be overridden on a per adapter basis.

* `suppressIpAddress` - Suppress the Adapter IP Address and port when creating the Agent Device ids and names for 1.7. This applies to all adapters.

* `timeout` - The amount of time (in milliseconds) an adapter can be silent before it is disconnected.

* `reconnectInterval` - The amount of time (in milliseconds) between adapter reconnection attempts.


## Contribution / Feedback
- Please use the [Issues](https://github.com/TrakHound/MTConnect.NET/issues) tab to create issues for specific problems that you may encounter 
- Please feel free to use the [Pull Requests](https://github.com/TrakHound/MTConnect.NET/pulls) tab for any suggested improvements to the source code
- For any other questions or feedback, please contact TrakHound directly at **info@trakhound.com**.

## License
This application and it's source code is licensed under the [MIT License](https://choosealicense.com/licenses/mit/) and is free to use.
