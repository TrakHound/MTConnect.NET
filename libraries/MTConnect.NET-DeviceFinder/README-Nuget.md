![MTConnect.NET Logo](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/logo.png) 

# DeviceFinder

## Overview
The MTConnect.NET-DeviceFinder library is used to search a network to find MTConnect Devices. The MTConnectDeviceFinder class contains events for when a new MTConnect Device has been found as well as events for what IP addresses were reachable and what ports were open on those addresses.

The sequence of operations is described below:

- Ping IP Addresses : The IP Addresses within the specified AddressRange will be sent a Ping request. Each successfully Pinged address is added to a list of reachable addresses
- Test Ports : For each IP address that was successfully Pinged, a TCP connection is established based on the specified PortRange to test if the port is open and if there is an application listening on the port
- Request MTConnect Probe : For each of the Ports that were found to be open in the previous test, an MTConnect Probe request is sent to attempt to connect to an MTConnect Agent. If any devices are found, each MTConnect Device is returned in a separate DeviceFound event that contains information about the MTConnect Device and the Network connection.

## Usage
```c#
using MTConnect.DeviceFinder;

var finder = new MTConnectDeviceFinder();
finder.ScanInterval = 60000; // Interval in milliseconds
finder.Addresses = new AddressRange("192.168.1.0", "192.168.1.255");
finder.Ports = new PortRange(5000, 5020);

finder.PingSent += Finder_PingSent;
finder.PingReceived += Finder_PingReceived;

finder.PortOpened += Finder_PortOpened;
finder.PortClosed += Finder_PortClosed;

finder.ProbeSent += Finder_ProbeSent;
finder.ProbeSuccessful += Finder_ProbeSuccessful;
finder.DeviceFound += Finder_DeviceFound;

finder.SearchCompleted += Finder_SearchCompleted;

finder.Start();
```

```c#
void Finder_DeviceFound(MTConnectDeviceFinder sender, MTConnectDevice device)
{
    Console.WriteLine($"{device.IpAddress}:{device.Port} : {device.Name} ({device.Uuid})");
}

void Finder_SearchCompleted(MTConnectDeviceFinder sender, long milliseconds)
{
    Console.WriteLine($"Search Completed in {milliseconds}ms");
}
```

### Properties

* `Addresses` - The range of IP Addresses to search. If not set, the addresses of the network that the host is connected to are used

* `Ports` - The range of Ports to test on each reachable address. If not set, ports 5000 through 5019 are used

* `ScanInterval` - The Interval in Milliseconds to repeat the search at. If not set (0), the search is run once

* `Timeout` - The timeout in Milliseconds used for the Port tests and the Probe requests. The default is 500

* `SubsequentPingDelay` - The delay in Milliseconds between ping requests. The default is 25

### Events

* `PingSent` / `PingReceived` - Raised for each Ping request that is sent and for each reply

* `PortOpened` / `PortClosed` - Raised with the result of each Port test

* `ProbeSent` / `ProbeSuccessful` - Raised for each Probe request that is sent and for each one that returned an MTConnectDevices Response Document

* `DeviceFound` - Raised for each MTConnect Device that is found

* `SearchCompleted` - Raised when a search has finished. Contains the duration of the search in Milliseconds

> The `ProbeError` event is declared but is not currently raised.

### Methods

* `Start()` - Starts the search on a background thread. Use `Start(false)` to block the calling thread until the DeviceFinder is stopped

* `Stop()` - Stops the DeviceFinder
