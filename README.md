# TH_MTConnect
MTConnect® Tools used in the [TrakHound](http:github.com/TrakHound/TrakHound/) MDC Software Package

## About
This .NET library contains tools for retrieving and processing data using the MTConnect® communication protocol for CNC and PLC industrial equipment.

## Nuget
**PM> Install-Package TH_MTConnect**
<br>
http://www.nuget.org/packages/TH_MTConnect/1.0.0

<br>

### Components.Requests Example

```c#
// Create Request URL for Probe Request
string url = TH_MTConnect.HTTP.GetUrl("127.0.0.1", 5000, "VMC-3Axis") + "probe";

// Get ReturnData object back that contains a hierarchical object with the retrieved Current data 
var returnData = TH_MTConnect.Components.Requests.Get(url, 2000, 1);
```

### Streams.Requests Example

```c#
// Create Request URL for Current Request
string url = TH_MTConnect.HTTP.GetUrl("127.0.0.1", 5000, "VMC-3Axis") + "current";

// Get ReturnData object back that contains a hierarchical object with the retrieved Probe data 
var returnData = TH_MTConnect.Streams.Requests.Get(url, 2000, 1);
```

## License
This library is licensed under the Apache 2.0 License
