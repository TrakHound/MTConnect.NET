![MTConnect.NET Logo](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/mtconnect-net-03-md.png) 

# MTConnect HTTP Server Agent Module
This Agent Module implements a MTConnect REST Protocol Http server

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
            <td>MTConnect.NET-AgentModule-HttpServer</td>
            <td><img src="https://img.shields.io/nuget/dt/MTConnect.NET-AgentModule-HttpServer?style=for-the-badge&logo=nuget&label=%20&color=%23333"/></td>
            <td><a href="https://www.nuget.org/packages/MTConnect.NET-AgentModule-HttpServer">https://www.nuget.org/packages/MTConnect.NET-AgentModule-HttpServer</a></td>
        </tr>
    </tbody>
</table>

## Configuration
```yaml
 - http-server:
    hostname: localhost
    port: 5000
    allowPut: true
    indentOutput: true
    documentFormat: xml
    responseCompression:
    - gzip
    - br
    files:
    - path: schemas
      location: schemas
    - path: styles
      location: styles
    - path: styles/favicon.ico
      location: favicon.ico
```

* `hostname` - The server IP Address or Hostname to bind to.

* `port` - The port number the agent binds to for requests.

* `responseCompression` - Sets the List of Encodings (ex. gzip, br, deflate) to pass to the Accept-Encoding HTTP Header

* `version` - Sets the MTConnect Version to use for the response document. This can be used to request a document using an older version for legacy clients

* `allowPut` - Allow HTTP PUT or POST of data item values or assets.

* `allowPutFrom` - Allow HTTP PUT or POST from a specific host or list of hosts. 
* `indentOutput` - Sets the indentation of the response document. True = Indent. False = Do not Indent

* `outputComments` - Sets the default response document comments output. Comments contain descriptions from the MTConnect standard

* `outputValidationLevel` - Sets the default response document validation level. 0 = Ignore, 1 = Warning, 2 = Strict

* `files` - Sets the configuration for Static Files that can be served from the Http Server.
    * `path` - The location of the files on the server (where the Agent is running)
    * `location` - The path to match in the requested URL

* `tls` - Sets the TLS settings

    * `pfx` - The PFX certificate settings
        * `certificatePath` - The path to the (.pfx) file
        * `certificatePassword` - The certificate password

    * `pem` - The PEM certificate settings
        * `certificatePath` - The path to the (.pem) file
        * `privateKeyPath` - The path to the key containing the private key
        * `privateKeyPassword` - The certificate password

    * `verifyClientCertificate` - Toggles whether Client Certificate chains are verified ("true" or "false")

### Example 1
Use defualt configuration
```yaml
 - http-server:
```

### Example 2
Specify the port
```yaml
 - http-server:
    port: 5001
```

### Example 3
Specify the port and hostname to an IP Address
```yaml
 - http-server:
    hostname: 192.168.1.145
    port: 5001
```

### Example 4
Specify the port and hostname
```yaml
 - http-server:
    hostname: DESKTOP-HV74M4N
    port: 5001
```

### Example 4
Specify the port and hostname with TLS (PFX Certificate)
```yaml
 - http-server:
    hostname: DESKTOP-HV74M4N
    port: 5001
    tls:
      pfx:
        certificatePath: c:\certs\mtconnect-testing.pfx
        certificatePassword: mtconnect
```


## Contribution / Feedback
- Please use the [Issues](https://github.com/TrakHound/MTConnect.NET/issues) tab to create issues for specific problems that you may encounter 
- Please feel free to use the [Pull Requests](https://github.com/TrakHound/MTConnect.NET/pulls) tab for any suggested improvements to the source code
- For any other questions or feedback, please contact TrakHound directly at **info@trakhound.com**.

## License
This application and it's source code is licensed under the [MIT License](https://choosealicense.com/licenses/mit/) and is free to use.
