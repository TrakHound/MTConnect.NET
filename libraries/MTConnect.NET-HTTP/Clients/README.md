![MTConnect.NET Logo](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/dev/img/mtconnect-net-03-md.png) 

# MTConnect Http REST Clients
These client classes use the Http REST Api that is described in the [MTConnect Standard](https://model.mtconnect.org/#Package__8082e379-d82e-4b0e-abad-83cdf92f7fe6).

## MTConnectHttpClient
The [MTConnectHttpClient](MTConnectHttpClient.cs) class is the primary class to use when wanting to implement the full MTConnect REST protocol. This class handles an initial Probe request to gather capabilities of the Agent, a Current request to read the initial values, and a Sample request (at the specified Interval) to read successive values.

### Class Initialization
Class initialization is straightforward in that specifiying the BaseUrl (the URL of the Agent) is all that is required. Then call the **Start()** method to start the request sequence.

#### No Device Name (Get All Devices)
```c#
using MTConnect.Clients;

var baseUrl = "localhost:5000";

var client = new MTConnectHttpClient(baseUrl);
client.Interval = 500;
client.Start();
```

#### Get Specific Device by Name
```c#
using MTConnect.Clients;

var baseUrl = "localhost:5000";
var deviceName = "OKUMA-Lathe";

var client = new MTConnectHttpClient(baseUrl, deviceName);
client.Interval = 500;
client.Start();
```

#### Specfiy Hostname and Port
```c#
using MTConnect.Clients;

var hostname = "localhost";
var port = 5000;

var client = new MTConnectHttpClient(hostname, port);
client.Interval = 500;
client.Start();
```

#### Specfiy Hostname, Port, and Device
```c#
using MTConnect.Clients;

var hostname = "localhost";
var port = 5000;
var deviceName = "OKUMA.Lathe";

var client = new MTConnectHttpClient(hostname, port, deviceName);
client.Interval = 500;
client.Start();
```

### Properties

* `Id` - A unique Identifier used to indentify this instance of the MTConnectClient class

* `Authority` - The authority portion consists of the DNS name or IP address associated with an Agent

* `Device` - If present, specifies that only the Equipment Metadata for the piece of equipment represented by the name or uuid will be published

* `DocumentFormat` - Gets or Sets the Document Format to use (ex. XML, JSON, etc.)

* `Interval` - Gets or Sets the Interval in Milliseconds for the Sample Stream

* `Timeout` - Gets or Sets the connection timeout for the request

* `Heartbeat` - Gets or Sets the MTConnect Agent Heartbeat for the request

* `RetryInterval` - Gets or Sets the Interval in Milliseconds that the Client will attempt to reconnect if the connection fails

* `MaximumSampleCount` - Gets or Sets the Maximum Number of Samples returned per interval from the Sample Stream

* `ContentEncodings` - Gets or Sets the List of Encodings (ex. gzip, br, deflate) to pass to the Accept-Encoding HTTP Header

* `ContentType` - Gets or Sets the Content-type (or MIME-type) to pass to the Accept HTTP Header

* `LastInstanceId` - Gets the Last Instance ID read from the MTConnect Agent

* `LastSequence` - Gets the Last Sequence read from the MTConnect Agent

* `LastResponse` - Gets the Unix Timestamp (in Milliseconds) since the last response from the MTConnect Agent

* `CurrentOnly` - Gets or Sets whether the stream requests a Current (true) or a Sample (false)

### Handle Probe Received Event
###### (MTConnectDevices Response Document received from a Probe Request)
```c#
var baseUrl = "localhost:5000";
var deviceName = "OKUMA-Lathe";

var client = new MTConnectHttpClient(baseUrl, deviceName);
client.OnProbeReceived += (sender, document) =>
{
    foreach (var device in document.Devices)
    {
        // Device
        Console.WriteLine(device.Id);

        // All DataItems (traverse the entire Device model)
        foreach (var dataItem in device.GetDataItems())
        {
            Console.WriteLine(dataItem.IdPath);
        }

        // All Components (traverse the entire Device model)
        foreach (var component in device.GetComponents())
        {
            Console.WriteLine(component.IdPath);
        }

        // All Compositions (traverse the entire Device model)
        foreach (var composition in device.GetCompositions())
        {
            Console.WriteLine(composition.IdPath);
        }
    }
};
client.Start();
```

### Handle Current Received Event
###### (MTConnectStreams Response Document received from a Current Request)
```c#
var baseUrl = "localhost:5000";
var deviceName = "OKUMA-Lathe";

var client = new MTConnectHttpClient(baseUrl, deviceName);
client.OnCurrentReceived += (sender, document) =>
{
    foreach (var deviceStream in document.Streams)
    {
        // DeviceStream
        Console.WriteLine(deviceStream.Name);

        // Component Streams
        foreach (var componentStream in deviceStream.ComponentStreams)
        {
            Console.WriteLine(componentStream.Name);

            // DataItems (Samples, Events, and Conditions)
            foreach (var observation in componentStream.Observations)
            {
                Console.WriteLine(observation.DataItemId);
            }
        }
    }
};
client.Start();
```

### Handle Samples Received Event
###### (MTConnectStreams Response Document received from a Sample Stream)
```c#
var baseUrl = "localhost:5000";
var deviceName = "OKUMA-Lathe";

var client = new MTConnectHttpClient(baseUrl, deviceName);
client.Interval = 500;
client.OnSampleReceived += (sender, document) =>
{
    foreach (var deviceStream in document.Streams)
    {
        // DeviceStream
        Console.WriteLine(deviceStream.Name);

        // Component Streams
        foreach (var componentStream in deviceStream.ComponentStreams)
        {
            Console.WriteLine(componentStream.Name);

            // DataItems (Samples, Events, and Conditions)
            foreach (var observation in componentStream.Observations)
            {
                Console.WriteLine(observation.DataItemId);
            }
        }
    }
};
client.Start();
```

### Handle Assets Received Event
###### (MTConnectAssets Response Document received from an Asset Request)
```c#
var baseUrl = "localhost:5000";
var deviceName = "OKUMA-Lathe";

var client = new MTConnectHttpClient(baseUrl, deviceName);
client.Interval = 500;
client.OnAssetsReceived += (sender, document) =>
{
    foreach (var asset in document.Assets)
    {
        // Print AssetId to the Console
        Console.WriteLine(asset.AssetId);
    }
};
client.Start();
```


## MTConnectHttpProbeClient
The [MTConnectHttpProbeClient](MTConnectHttpProbeClient.cs) class is used to send a Probe request and return an MTConnectDevices Response Document.
```c#
var baseUrl = "localhost:5000";
var deviceName = "OKUMA.Lathe";

var client = new MTConnectHttpProbeClient(baseUrl, deviceName);
var document = client.Get();
foreach (var device in document.Devices)
{
    // Device
    Console.WriteLine(device.Id);

    // All DataItems (traverse the entire Device model)
    foreach (var dataItem in device.GetDataItems())
    {
        Console.WriteLine(dataItem.IdPath);
    }

    // All Components (traverse the entire Device model)
    foreach (var component in device.GetComponents())
    {
        Console.WriteLine(component.IdPath);
    }

    // All Compositions (traverse the entire Device model)
    foreach (var composition in device.GetCompositions())
    {
        Console.WriteLine(composition.IdPath);
    }
}
```

## MTConnectHttpCurrentClient
The [MTConnectHttpCurrentClient](MTConnectHttpCurrentClient.cs) class is used to send a Current request and return an MTConnectStreams Response Document.
```c#
var baseUrl = "localhost:5000";
var deviceName = "OKUMA.Lathe";

var client = new MTConnectHttpCurrentClient(baseUrl, deviceName);
var document = client.Get();
foreach (var deviceStream in document.Streams)
{
    // Device
    Console.WriteLine(deviceStream.Name);

    // Component Streams
    foreach (var componentStream in deviceStream.ComponentStreams)
    {
        Console.WriteLine(componentStream.Name);

        // DataItems (Samples, Events, and Conditions)
        foreach (var observation in componentStream.Observations)
        {
            Console.WriteLine(observation.DataItemId);
        }
    }
}
```

## MTConnectHttpSampleClient
The [MTConnectHttpSampleClient](MTConnectHttpSampleClient.cs) class is used to send a Sample request and return an MTConnectStreams Response Document.
```c#
var baseUrl = "localhost:5000";
var deviceName = "OKUMA.Lathe";
var fromSequence = 150;
var toSequence = 250;

var client = new MTConnectHttpSampleClient(baseUrl, deviceName, fromSequence, toSequence);
var document = client.Get();
foreach (var deviceStream in document.Streams)
{
    // Device
    Console.WriteLine(deviceStream.Name);

    // Component Streams
    foreach (var componentStream in deviceStream.ComponentStreams)
    {
        Console.WriteLine(componentStream.Name);

        // DataItems (Samples, Events, and Conditions)
        foreach (var observation in componentStream.Observations)
        {
            Console.WriteLine(observation.DataItemId);
        }
    }
}
```

## MTConnectHttpAssetClient
The [MTConnectHttpAssetClient](MTConnectHttpAssetClient.cs) class is used to send an Assets request and return an MTConnectAssets Response Document.
```c#
var baseUrl = "localhost:5000";
var deviceName = "OKUMA.Lathe";
var count = 5

var client = new MTConnectHttpAssetClient(baseUrl, deviceName, count);
var document = client.Get();
foreach (var asset in document.Assets)
{
    // Print AssetId to the Console
    Console.WriteLine(asset.AssetId);
}
```
