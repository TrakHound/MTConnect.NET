# MTConnect HTTP Server
The HTTP interface for MTConnect Agents is provided by the MTConnectHttpServer class. This class runs an HttpListener and handles all MTConnect requests.

## Requests

### Probe
Probe requests are used to retreive MTConnect Information Model data from the underlying MTConnectAgent. The MTConnectHttpServer class supports all of the requests defined in the MTConnect standard as well as some additional parameters that are specific to MTConnect.NET.

#### Query Parameters

`version`

Sets the MTConnect Version to use for the response document. This can be used to request a document using an older version for legacy clients

`documentFormat`

Sets the Document Format (XML, JSON, etc.) to use to format the response document

`validationLevel`

Sets the Validation Level to use when formatting the response document. This is currently used to set the XSD Schema validation level. 
- 0 = Ignore : any validation errors
- 1 = Warning : The server will respond with the document but will add validation errors to the log
- 2 = Strict : The server will respond with an MTConnectError document containing validation errors (if present)

`indentOutput`

Sets the indentation of the response document. True = Indent. False = Do not Indent

`outputComments`

Sets whether comments can be output in the response document. This is currently used to output descriptive information in Xml response documents. Descriptive information includes descriptions for Component and DataItem Types

##### Example 1:
Request a Probe document using MTConnect Version 1.5

> http<nolink>://localhost:5000?version=1.5

##### Example 2:
Request a Probe document formatted in XML (default)
> http<nolink>://localhost:5000?documentFormat=XML

##### Example 3:
Request a Probe document with a validation level of 2 (Strict)
> http<nolink>://localhost:5000?validationLevel=2

##### Example 4:
Request a Probe document with the ouput indented and output comments
> http<nolink>://localhost:5000?indentOutput=true&outputComments=true
