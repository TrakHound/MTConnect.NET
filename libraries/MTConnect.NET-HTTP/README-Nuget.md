![MTConnect.NET Logo](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/logo.png) 

# MTConnect.NET-HTTP
Classes to implement HTTP Clients & Servers for MTConnect.

For the HTTP server / adapter module pages and end-to-end configuration recipes, see [Modules](https://www.trakhound.com/docs/mtconnect-net/modules) and [Configure & Use](https://www.trakhound.com/docs/mtconnect-net#configure).

## Client
A client that implements the full MTConnect REST Protocol is implemented in the [MTConnectHttpClient](https://github.com/TrakHound/MTConnect.NET/blob/master/libraries/MTConnect.NET-HTTP/Clients/MTConnectHttpClient.cs) class. This client supports individual endpoint requests as well as the streaming protocol.

> **[Learn More](https://github.com/TrakHound/MTConnect.NET/blob/master/libraries/MTConnect.NET-HTTP/Clients/README.md)** : Click here to learn more about MTConnect REST HTTP Clients in MTConnect.NET

## Server
A fully MTConnect compatible HTTP web server is implemented in the [MTConnectHttpServer](https://github.com/TrakHound/MTConnect.NET/blob/master/libraries/MTConnect.NET-HTTP/Servers/MTConnectHttpServer.cs) class. This server supports all REST endpoints defined in the MTConnect standard as well as the streaming protocol.

> **[Learn More](https://github.com/TrakHound/MTConnect.NET/blob/master/libraries/MTConnect.NET-HTTP/Servers/README.md)** : Click here to learn more about MTConnect REST HTTP Servers in MTConnect.NET
