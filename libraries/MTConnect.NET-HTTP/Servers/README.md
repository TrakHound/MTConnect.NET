![MTConnect.NET Logo](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/logo.png) 

# MTConnect HTTP Server
The HTTP interface for MTConnect Agents is provided by the MTConnectHttpServer class. This class runs a CEEN Http Server and handles all MTConnect requests.

> The HTTP server uses the CEEN project (the source is included in this library). This allows for better control and better TLS implementation

```c#
using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Servers.Http;

var agent = new MTConnectAgentBroker();
agent.Start();

var configuration = new HttpServerConfiguration();
configuration.Port = 5000;

var server = new MTConnectHttpServer(configuration, agent);
server.Start();
```

## Requests

### Probe
Probe requests are used to retrieve MTConnect Information Model data from the underlying MTConnectAgent. The MTConnectHttpServer class supports all of the requests defined in the MTConnect standard as well as some additional parameters that are specific to MTConnect.NET.

> http<nolink>://\<HOSTNAME>:\<PORT>
  
> http<nolink>://\<HOSTNAME>:\<PORT>/probe
  
> http<nolink>://\<HOSTNAME>:\<PORT>/\<DEVICE_UUID>
  
> http<nolink>://\<HOSTNAME>:\<PORT>/\<DEVICE_NAME>
  
> http<nolink>://\<HOSTNAME>:\<PORT>/\<DEVICE_UUID>/probe
  
> http<nolink>://\<HOSTNAME>:\<PORT>/\<DEVICE_NAME>/probe

Example: http<nolink>://localhost:5000
  
Example: http<nolink>://localhost:5000/VMC-3Axis

#### Query Parameters

`deviceType`

Filters the Devices that are returned by Device Type. This is used when a specific Device is not specified in the URL. This parameter is also read by Current and Sample requests

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

The `version`, `documentFormat`, `validationLevel`, `indentOutput`, and `outputComments` parameters are read for all of the GET requests listed below (Probe, Current, Sample, Assets, and Asset).

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
Request a Probe document with the output indented and output comments
> http<nolink>://localhost:5000?indentOutput=true&outputComments=true

### Current
Current requests are used to retrieve an MTConnectStreams Response Document that contains the most recent Observations.

> http<nolink>://\<HOSTNAME>:\<PORT>/current

> http<nolink>://\<HOSTNAME>:\<PORT>/\<DEVICE_UUID or DEVICE_NAME>/current

#### Query Parameters

`path` : An XPath expression used to select the DataItems to return

`at` : The sequence number to return the Observations at. Cannot be used together with `interval`

`interval` : The interval (in milliseconds) to send documents at. When specified, the response is a stream (multipart) of documents instead of a single document

`heartbeat` : The heartbeat (in milliseconds) used for a stream. Defaults to 10000 with a minimum of 500

`deviceType` : Filters by Device Type (see Probe)

### Sample
Sample requests are used to retrieve an MTConnectStreams Response Document that contains the Observations within a range of sequence numbers.

> http<nolink>://\<HOSTNAME>:\<PORT>/sample

> http<nolink>://\<HOSTNAME>:\<PORT>/\<DEVICE_UUID or DEVICE_NAME>/sample

#### Query Parameters

`path` : An XPath expression used to select the DataItems to return

`from` : The sequence number to start from

`to` : The sequence number to end at

`count` : The maximum number of Observations to return. Defaults to 100. A negative count is not supported and returns an MTConnectError document

`interval` : The interval (in milliseconds) to send documents at. When specified, the response is a stream (multipart) of documents instead of a single document

`heartbeat` : The heartbeat (in milliseconds) used for a stream. Defaults to 10000 with a minimum of 500

`deviceType` : Filters by Device Type (see Probe)

Example: http<nolink>://localhost:5000/sample?from=150&count=200

### Assets
Assets requests are used to retrieve an MTConnectAssets Response Document.

> http<nolink>://\<HOSTNAME>:\<PORT>/assets

> http<nolink>://\<HOSTNAME>:\<PORT>/\<DEVICE_UUID or DEVICE_NAME>/assets

#### Query Parameters

`type` : The Asset Type to return (ex. CuttingTool)

`removed` : Sets whether Assets that have been marked as removed are returned

`count` : The maximum number of Assets to return. Defaults to 100

### Asset
Asset requests are used to retrieve an MTConnectAssets Response Document that contains the Asset(s) with the specified AssetId(s). Multiple AssetIds are separated using a semicolon.

> http<nolink>://\<HOSTNAME>:\<PORT>/asset/\<ASSET_ID>

> http<nolink>://\<HOSTNAME>:\<PORT>/asset/\<ASSET_ID>;\<ASSET_ID>

### PUT (Observations) and POST (Assets)
PUT and POST requests are used to write data to the Agent. Both are only accepted when the `AllowPut` configuration property is set to true (the default is false). If the `AllowPutFrom` configuration property is set, the request must also come from one of the listed addresses. Otherwise the server responds with a 403 status code.

The MTConnectHttpServer class does not write to the Agent itself. A PUT request calls the `OnObservationInput` method and a POST request calls the `OnAssetInput` method. Both return false in the MTConnectHttpServer class so a derived class must override them to add the data to the Agent.

#### PUT
The Device is read from the URL path and each query parameter is read as an Observation (the name is the DataItem key and the value is the Observation value).

> http<nolink>://\<HOSTNAME>:\<PORT>/\<DEVICE_UUID or DEVICE_NAME>?\<DATAITEM_KEY>=\<VALUE>

#### POST
The AssetId is read from the URL path and the Asset is read from the request body (XML by default or JSON when the Content-Type header is `application/json`). The request body is limited to 2 MB (a 413 status code is returned if it is larger).

> http<nolink>://\<HOSTNAME>:\<PORT>/asset/\<ASSET_ID>?device=\<DEVICE_UUID or DEVICE_NAME>&type=\<ASSET_TYPE>

Both respond with `<success/>` when the input was accepted or with an MTConnectError document (404) when it was not.
