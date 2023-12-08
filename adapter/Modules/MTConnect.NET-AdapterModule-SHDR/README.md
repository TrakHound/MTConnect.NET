![MTConnect.NET Logo](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/mtconnect-net-03-md.png) 

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
- shdr:
    port: 7878
    deviceKey: M12346
```

* `port` - The port number to read from.

* `deviceKey` - The UUID or Name of the Device to send data for.


## Contribution / Feedback
- Please use the [Issues](https://github.com/TrakHound/MTConnect.NET/issues) tab to create issues for specific problems that you may encounter 
- Please feel free to use the [Pull Requests](https://github.com/TrakHound/MTConnect.NET/pulls) tab for any suggested improvements to the source code
- For any other questions or feedback, please contact TrakHound directly at **info@trakhound.com**.

## License
This application and it's source code is licensed under the [MIT License](https://choosealicense.com/licenses/mit/) and is free to use.
