![MTConnect.NET Logo](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/mtconnect-net-03-md.png) 

# DeviceFinder

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
            <td>MTConnect.NET-DeviceFinder</td>
            <td><img src="https://img.shields.io/nuget/dt/MTConnect.NET-DeviceFinder?style=for-the-badge&logo=nuget&label=%20&color=%23333"/></td>
            <td><a href="https://www.nuget.org/packages/MTConnect.NET-DeviceFinder">https://www.nuget.org/packages/MTConnect.NET-DeviceFinder</a></td>
        </tr>
    </tbody>
</table>


## Overview
The MTConnect.NET-DeviceFinder library is used to search a network to find MTConnect Devices. The MTConnectDeviceFinder class contains events for when a new MTConnect Device has been found as well as events for what IP addresses were reachable and what ports were open on those addresses.

The sequence of operations is describe below:

- Ping IP Addresses : The IP Addresses within the specified AddressRange will be sent a Ping request. Each successful Pinged address is added a to list of reachable addresses
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
finder.ProbeError += Finder_ProbeError;
finder.ProbeSuccessful += Finder_ProbeSuccessful;
finder.DeviceFound += Finder_DeviceFound;

finder.Start();
```
