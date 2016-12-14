![MTConnect.NET Logo] (http://trakhound.com/images/mtconnect-net-logo.png)

[![Travis branch](https://img.shields.io/travis/TrakHound/MTConnect.NET.svg?style=flat-square)](https://travis-ci.org/TrakHound/MTConnect.NET) [![NuGet](https://img.shields.io/nuget/v/MTConnect.Net.svg?style=flat-square)](https://www.nuget.org/packages/MTConnect.NET/) [![Hex.pm](https://img.shields.io/hexpm/l/plug.svg?style=flat-square)](https://www.apache.org/licenses/LICENSE-2.0)

MTConnect.NET is a .NET library for the [MTConnect®](http://www.mtconnect.org) protocol for machine tool data collection. Uses the .NET XmlSerializer to parse and easy to use functions for requesting data from MTConnect Agents. Supports up to MTConnect v1.3.

# Features
- Easy to use client classes
- Full MTConnect document responses as class objects
- Intellisense using text directly from the MTConnect Standard
- Simple parsing using built in XmlSerializer

# Installation

## Nuget
**PM> Install-Package MTConnect.NET**

http://www.nuget.org/packages/MTConnect.NET/

# Examples

## MTConnectClient
The MTConnectClient class handles the entire request structure for a typical data collection application using MTConnect. First a Probe request is made, then a Current request, then a stream is opened for any new Sample data. The class will continue to run until the Stop() method is called and will handle errors internally.

```c#
using MTConnectDevices = MTConnect.MTConnectDevices;
using MTConnectStreams = MTConnect.MTConnectStreams;
using MTConnect.Client;

MTConnectClient client;

void Start()
{
  // The base address for the MTConnect Agent
  string baseUrl = "http://agent.mtconnect.org";

  // Create a new MTConnectClient using the baseUrl
  client = new MTConnectClient(baseUrl);

  // Subscribe to the Event handlers to receive the MTConnect documents
  client.ProbeReceived += Probe_Successful;
  client.CurrentReceived += Current_Successful;
  client.SampleReceived += Current_Successful;

  // Start the MTConnectClient
  client.Start();
}

void Stop()
{
  client.Stop();
}

// --- Event Handlers ---

void DevicesSuccessful(MTConnectDevices.Document document)
{
  foreach (var device in document.Devices)
  {
     var dataItems = device.GetDataItems();
     foreach (var dataItem in dataItems) Console.WriteLine(dataItem.Id + " : " + dataItem.Name);
  }
}

void StreamsSuccessful(MTConnectStreams.Document document)
{
  foreach (var deviceStream in document.DeviceStreams)
  {
     foreach (var dataItem in deviceStream.DataItems) Console.WriteLine(dataItem.DataItemId + " = " + dataItem.CDATA);
  }
}

```

## MTConnect Requests
The Probe, Current, and Sample requests return the appropriate MTConnect document object. Requests can be sent either synchronously or asynchronously and errors are returned as an MTConnectError document. Request constructors have multiple overloads to make configuration easier and to keep your code clean.

### Probe

```c#
// The base address for the MTConnect Agent
string baseUrl = "http://agent.mtconnect.org";

// Execute the Probe request and get an MTConnectDevices.Document object back
var probe = new Probe(baseUrl).Execute();

// Execute the Probe request asynchronously and return the MTConnectDevices.Document using the event handler
var probe = new Probe(baseUrl);
probe.Successful += ProbeSuccess;
probe.ExecuteAsync();
```

### Current

```c#
// The base address for the MTConnect Agent
string baseUrl = "http://74.203.109.245:5001";

// Execute the Current request and get an MTConnectStreams.Document object back
var current = new Current(baseUrl).Execute();
```

### Sample

```c#
// The base address for the MTConnect Agent
string baseUrl = "http://agent.mtconnect.org";

// Execute the Sample request from sequence 200 to 500 and get an MTConnectStreams.Document object back
var sample = new Sample(baseUrl, 200, 500).Execute();
```

## Documents

### MTConnectDevices.Document
The MTConnectDevices.Document class uses the same structure as laid out in the raw MTConnect XML file and allows you to easily list or search the DataItems by Component or DataItem ID using Linq.

```c#
// Find DataItem by ID
var avail = document.Devices[0].GetDataItems().Find(o => o.Id == "avail");

// Find DataItem by Type
var estop = document.Devices[0].GetDataItems().Find(o => o.Type == "EMERGENCY_STOP");

// Find the first Controller component
var controller = document.Devices[0].GetComponents().Find(o => o.GetType() == typeof(MTConnectDevices.Components.Controller));

// Find DataItem by Type in the first Controller/Path component
var program = document.Devices[0].GetComponents().Find(o => o.GetType() == typeof(MTConnectDevices.Components.Path)).DataItems.Find(o => o.Type == "PROGRAM");

// Find all of the Line Types in each Controller/Path component
foreach (var path in document.Devices[0].GetComponents().FindAll(o => o.GetType() == typeof(MTConnectDevices.Components.Path)))
{
  var line = path.DataItems.Find(o => o.Type == "LINE");
}
```

### MTConnectStreams.Document
The MTConnectStreams.Document class uses the same structure as laid out in the raw MTConnect XML file and allows you to easily list or search the DataItems by Component or DataItem ID using Linq.

```c#
// Find DataItem by ID and write value to console
var avail = document.DeviceStreams[0].DataItems.Find(o => o.DataItemId == "avail");
if (avail != null) Console.WriteLine(avail.CDATA);

// Find DataItem by Type
var estop = document.DeviceStreams[0].DataItems.Find(o => o.Type == "EMERGENCY_STOP");

// Find the a component by the ComponentId
var cn1 = document.DeviceStreams[0].ComponentStreams.Find(o => o.ComponentId == "cn1");

// Find a Condition by Id and write both the Condition and CDATA to console
var htemp = document.DeviceStreams[0].Conditions.Find(o => o.DataItemId == "htemp");
if (htemp != null) Console.WriteLine(htemp.ConditionValue + " : " + htemp.CDATA);
```

## License
This library is licensed under the Apache 2.0 License
