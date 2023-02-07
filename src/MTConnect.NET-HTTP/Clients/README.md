# MTConnect Http REST Clients
These client classes use the Http REST Api that is described in [MTConnect Standard](https://model.mtconnect.org/#Package__8082e379-d82e-4b0e-abad-83cdf92f7fe6).

## MTConnectHttpClient
The [MTConnectHttpClient](MTConnectHttpClient.cs) class is the primary class to use when wanting to implement the full MTConnect REST protocol. This class handles an initial Probe request to gather capabilities of the Agent, a Current request to read the initial values, and a Sample request (at the specified Interval) to read successive values.

### Class Initialization
Class initialization is straightforward in that specifiying the BaseUrl (the URL of the Agent) is all that is required. Then call the **Start()** method to start the request sequence.

#### No Device Name (Get All Devices)
```c#
using MTConnect.Clients;

var baseUrl = "localhost:5006";

var client = new MTConnectHttpClient(baseUrl);
client.Interval = 500;
client.Start();
```

#### Get Specific Device by Name
```c#
using MTConnect.Clients;

var deviceName = "OKUMA.Lathe";
var baseUrl = "localhost:5000";

var client = new MTConnectHttpClient(baseUrl, deviceName);
client.Interval = 500;
client.Start();
```

### Handle Probe Received Event
###### (MTConnectDevices Response Document received from a Probe Request)
```c#
var client = new MTConnectHttpClient(baseUrl, deviceName);
client.OnProbeReceived += (sender, document) =>
{
    foreach (var device in document.Devices)
    {
        // Device
        Console.WriteLine(device.Id);

        // DataItems
        foreach (var dataItem in device.DataItems)
        {
            Console.WriteLine(dataItem.Id);
        }

        // Components
        foreach (var component in device.Components)
        {
            Console.WriteLine(component.Id);
        }
    }
};
client.Start();
```

### Handle Current Received Event
###### (MTConnectStreams Response Document received from a Current Request)
```c#
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
            foreach (var dataItem in componentStream.DataItems)
            {
                Console.WriteLine(dataItem.DataItemId);
            }
        }
    }
};
client.Start();
```

### Handle Samples Received Event
###### (MTConnectStreams Response Document received from a Sample Stream)
```c#
var client = new MTConnectHttpClient(baseUrl, deviceName);
client.Interval = 500;
client.OnSampleReceived += (sender, document) =>
{
    foreach (var deviceStream in document.Streams)
    {
        // Device
        Console.WriteLine(deviceStream.Name);

        // Component Streams
        foreach (var componentStream in deviceStream.ComponentStreams)
        {
            Console.WriteLine(componentStream.Name);

            // DataItems
            foreach (var dataItem in componentStream.DataItems)
            {
                Console.WriteLine(dataItem.DataItemId);
            }
        }
    }
};
client.Start();
```

## MTConnectHttpProbeClient
The [MTConnectHttpProbeClient](MTConnectHttpProbeClient.cs) class is used to send a Probe request and return an MTConnectDevices Response Document.
```c#
using MTConnect.Clients;

var deviceName = "OKUMA.Lathe";
var baseUrl = "localhost:5000";

var client = new MTConnectHttpProbeClient(baseUrl, deviceName);
var document = client.Get();
foreach (var device in document.Devices)
{
    // Device
    Console.WriteLine(device.Id);

    // DataItems
    foreach (var dataItem in device.DataItems)
    {
        Console.WriteLine(dataItem.Id);
    }

    // Components
    foreach (var component in device.Components)
    {
        Console.WriteLine(component.Id);
    }
}
```

## MTConnectHttpCurrentClient
The [MTConnectHttpCurrentClient](MTConnectHttpCurrentClient.cs) class is used to send a Current request and return an MTConnectStreams Response Document.
```c#
using MTConnect.Clients;

var deviceName = "OKUMA.Lathe";
var baseUrl = "localhost:5006";

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

        // DataItems
        foreach (var dataItem in componentStream.DataItems)
        {
            Console.WriteLine(dataItem.DataItemId);
        }
    }
}
```

## MTConnectHttpSampleClient
The [MTConnectHttpSampleClient](MTConnectHttpSampleClient.cs) class is used to send a Sample request and return an MTConnectStreams Response Document.
```c#
using MTConnect.Clients;

var deviceName = "OKUMA.Lathe";
var baseUrl = "localhost:5006";
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

        // DataItems
        foreach (var dataItem in componentStream.DataItems)
        {
            Console.WriteLine(dataItem.DataItemId);
        }
    }
}
```
