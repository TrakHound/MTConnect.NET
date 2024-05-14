![MTConnect.NET Logo](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/mtconnect-net-03-md.png) 

# MTConnect.NET-HTTP
Classes to implement HTTP Clients & Servers for MTConnect

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
            <td>MTConnect.NET-HTTP</td>
            <td><img src="https://img.shields.io/nuget/dt/MTConnect.NET-HTTP?style=for-the-badge&logo=nuget&label=%20&color=%23333"/></td>
            <td><a href="https://www.nuget.org/packages/MTConnect.NET-HTTP">https://www.nuget.org/packages/MTConnect.NET-HTTP</a></td>
        </tr>
    </tbody>
</table>

## Client
A client that implements the full MTConnect REST Protocol is implemented in the [MTConnectHttpClient](https://github.com/TrakHound/MTConnect.NET/blob/version-6.0/libraries/MTConnect.NET-HTTP/Clients/MTConnectHttpClient.cs) class. This client supports individual endpoint requests as well the streaming protocol.

> **[Learn More](https://github.com/TrakHound/MTConnect.NET/blob/version-6.0/libraries/MTConnect.NET-HTTP/Clients/README.md)** : Click here to learn more about MTConnect REST HTTP Clients in MTConnect.NET

## Server
A fully MTConnect compatible HTTP web server is implemented in the [MTConnectHttpServer](https://github.com/TrakHound/MTConnect.NET/blob/version-6.0/libraries/MTConnect.NET-HTTP/Servers/MTConnectHttpServer.cs) class. This server supports all REST endpoints defined in the MTConnect standard as well as the streaming protocol.

> **[Learn More](https://github.com/TrakHound/MTConnect.NET/blob/version-6.0/libraries/MTConnect.NET-HTTP/Servers/README.md)** : Click here to learn more about MTConnect REST HTTP Servers in MTConnect.NET
