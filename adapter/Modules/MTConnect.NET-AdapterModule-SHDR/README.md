![MTConnect.NET Logo](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/logo.png) 

# MTConnect SHDR Adapter Module
This Adapter Module implements the SHDR Protocol to send data to an MTConnect Agent

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
            <td>MTConnect.NET-AdapterModule-SHDR</td>
            <td><img src="https://img.shields.io/nuget/dt/MTConnect.NET-AdapterModule-SHDR?style=for-the-badge&logo=nuget&label=%20&color=%23333"/></td>
            <td><a href="https://www.nuget.org/packages/MTConnect.NET-AdapterModule-SHDR">https://www.nuget.org/packages/MTConnect.NET-AdapterModule-SHDR</a></td>
        </tr>
    </tbody>
</table>

## Configuration
```yaml
modules:
  - shdr:
      port: 7878
```

* `port` - The TCP port that the adapter listens on for connections from an MTConnect Agent. Default is `7878`.

* `heartbeat` - The Heartbeat interval (in milliseconds) that is sent to the Agent in response to a PING request. Default is `10000`.

* `connectionTimeout` - The timeout (in milliseconds) used when reading from or writing to an Agent connection. Default is `5000`.

* `timeZoneOutput` - The time zone used for the timestamps that are output (ex. `ET`). Default is `Z` (UTC).

The Device that data is sent for is set using the `deviceKey` property at the top level of the adapter configuration file, not in this module.


## Contribution / Feedback
- Please use the [Issues](https://github.com/TrakHound/MTConnect.NET/issues) tab to create issues for specific problems that you may encounter 
- Please feel free to use the [Pull Requests](https://github.com/TrakHound/MTConnect.NET/pulls) tab for any suggested improvements to the source code
- For any other questions or feedback, please contact TrakHound directly at **info@trakhound.com**.

## License
This application and its source code is licensed under the [MIT License](https://choosealicense.com/licenses/mit/) and is free to use.
