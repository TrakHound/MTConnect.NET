![MTConnect.NET Logo](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/logo.png) 

# MTConnect HTTP Server Agent Module
This Agent Module implements a MTConnect REST Protocol Http server

## Configuration
```yaml
modules:
  - http-server:
      server: localhost
      port: 5000
      allowPut: true
      indentOutput: true
      documentFormat: xml
      accept:
        text/xml: xml
        application/json: json-cppagent
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

* `server` - The server IP Address or Hostname to bind to. If not set, the server listens on all network interfaces.

* `port` - The port number the agent binds to for requests. Default is `5000`.

* `documentFormat` - Sets the Document Format ID (ex. XML, JSON, JSON-cppagent) to use for the response document when it is not set by the request. Default is `XML`.

* `accept` - Maps HTTP Accept Headers to the corresponding Document Format ID. By default `text/xml` and `application/xml` are mapped to `XML` and `application/json` is mapped to `JSON`.

* `responseCompression` - Sets the List of Encodings (ex. gzip, br, deflate) that can be used to compress a response when requested by the client in the Accept-Encoding HTTP Header. If not set, responses are not compressed.

* `defaultVersion` - Sets the MTConnect Version to use for the response document when it is not set by the request. This can be used to output documents using an older version for legacy clients. If not set, the MTConnect Version of the Agent is used.

* `allowPut` - Allow HTTP PUT or POST of data item values or assets. Default is `false`.

* `allowPutFrom` - Allow HTTP PUT or POST from a specific IP Address or list of IP Addresses (compared to the IP Address of the client). If not set, any client is allowed when `allowPut` is enabled.

* `indentOutput` - Sets the indentation of the response document. True = Indent. False = Do not Indent. Default is `true`.

* `outputComments` - Sets the default response document comments output. Comments contain descriptions from the MTConnect standard. Default is `false`.

* `outputValidationLevel` - Sets the default response document validation level. 0 = Ignore, 1 = Warning, 2 = Strict. Default is `0` (Ignore).

* `files` - Sets the configuration for Static Files that can be served from the Http Server.
    * `path` - The location of the files on the server (where the Agent is running)
    * `location` - The path to match in the requested URL

* `devicesStyle`, `streamsStyle`, `assetsStyle`, `errorStyle` - Sets the XSL Stylesheet to reference in the corresponding XML response documents (MTConnectDevices, MTConnectStreams, MTConnectAssets, MTConnectError).
    * `location` - The URL of the Stylesheet that is written to the response document

* `streamsNamespaces` - Sets a list of additional XML Namespaces to add to MTConnectStreams XML response documents.
    * `alias` - The Namespace prefix
    * `urn` - The Namespace URN

* `tls` - Sets the TLS settings

    * `pfx` - The PFX certificate settings
        * `certificatePath` - The path to the (.pfx) file
        * `certificatePassword` - The certificate password

    * `pem` - The PEM certificate settings
        * `certificatePath` - The path to the (.pem) file
        * `privateKeyPath` - The path to the file containing the private key
        * `privateKeyPassword` - The private key password

### Example 1
Use default configuration
```yaml
modules:
  - http-server:
```

### Example 2
Specify the port
```yaml
modules:
  - http-server:
      port: 5001
```

### Example 3
Specify the port and server to an IP Address
```yaml
modules:
  - http-server:
      server: 192.168.1.145
      port: 5001
```

### Example 4
Specify the port and server to a Hostname
```yaml
modules:
  - http-server:
      server: DESKTOP-HV74M4N
      port: 5001
```

### Example 5
Specify the port and server with TLS (PFX Certificate)
```yaml
modules:
  - http-server:
      server: DESKTOP-HV74M4N
      port: 5001
      tls:
        pfx:
          certificatePath: c:\certs\mtconnect-testing.pfx
          certificatePassword: mtconnect
```

### Example 6
Specify the port and server with TLS (PEM Certificate)
```yaml
modules:
  - http-server:
      server: DESKTOP-HV74M4N
      port: 5001
      tls:
        pem:
          certificatePath: c:\certs\mtconnect-testing.crt
          privateKeyPath: c:\certs\mtconnect-testing.key
          privateKeyPassword: mtconnect
```

### Example 7
Specify with custom Accept headers
```yaml
modules:
  - http-server:
      accept:
        text/xml: xml
        application/json: json-cppagent
```


## Contribution / Feedback
- Please use the [Issues](https://github.com/TrakHound/MTConnect.NET/issues) tab to create issues for specific problems that you may encounter 
- Please feel free to use the [Pull Requests](https://github.com/TrakHound/MTConnect.NET/pulls) tab for any suggested improvements to the source code
- For any other questions or feedback, please contact TrakHound directly at **info@trakhound.com**.

## License
This application and its source code is licensed under the [MIT License](https://choosealicense.com/licenses/mit/) and is free to use.
