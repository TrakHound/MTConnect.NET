![MTConnect.NET Logo](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/dev/img/mtconnect-net-03-md.png) 

# MTConnect HTTP Server Agent Module

## Description
This Agent Module implements the MTConnect REST Protocol

## Configuration
```yaml
 - http-server:
    hostname: localhost
    port: 7878
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

* `maxStreamingThreads` - The maximum number of Threads to use for the Http Stream Requests

* `allowPut` - Allow HTTP PUT or POST of data item values or assets.

* `allowPutFrom` - Allow HTTP PUT or POST from a specific host or list of hosts. 

* `indentOutput` - Sets the default response document indendation

* `outputComments` - Sets the default response document comments output. Comments contain descriptions from the MTConnect standard

* `outputValidationLevel` - Sets the default response document validation level. 0 = Ignore, 1 = Warning, 2 = Strict

* `files` - Sets the configuration for Static Files that can be served from the Http Server.


## Contribution / Feedback
- Please use the [Issues](https://github.com/TrakHound/MTConnect.NET/issues) tab to create issues for specific problems that you may encounter 
- Please feel free to use the [Pull Requests](https://github.com/TrakHound/MTConnect.NET/pulls) tab for any suggested improvements to the source code
- For any other questions or feedback, please contact TrakHound directly at **info@trakhound.com**.

## License
This application and it's source code is licensed under the [MIT License](https://choosealicense.com/licenses/mit/) and is free to use.
