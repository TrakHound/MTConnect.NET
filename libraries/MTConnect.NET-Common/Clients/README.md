# MTConnect Clients
This directory contains the client interfaces only. The client classes that implement them are in the MTConnect.NET-HTTP and MTConnect.NET-MQTT libraries.

#### Client Interfaces
- [IMTConnectClient](https://github.com/TrakHound/MTConnect.NET/blob/master/libraries/MTConnect.NET-Common/Clients/IMTConnectClient.cs) : Interface used to read MTConnect response documents (Probe, Current, Sample, and Assets)
- [IMTConnectEntityClient](https://github.com/TrakHound/MTConnect.NET/blob/master/libraries/MTConnect.NET-Common/Clients/IMTConnectEntityClient.cs) : Interface used to read MTConnect entities (Device, Observation, Asset)
- [IMTConnectProbeClient](https://github.com/TrakHound/MTConnect.NET/blob/master/libraries/MTConnect.NET-Common/Clients/IMTConnectProbeClient.cs) : Interface used to perform a single Probe request
- [IMTConnectCurrentClient](https://github.com/TrakHound/MTConnect.NET/blob/master/libraries/MTConnect.NET-Common/Clients/IMTConnectCurrentClient.cs) : Interface used to perform a single Current request
- [IMTConnectSampleClient](https://github.com/TrakHound/MTConnect.NET/blob/master/libraries/MTConnect.NET-Common/Clients/IMTConnectSampleClient.cs) : Interface used to perform a single Sample request
- [IMTConnectAssetClient](https://github.com/TrakHound/MTConnect.NET/blob/master/libraries/MTConnect.NET-Common/Clients/IMTConnectAssetClient.cs) : Interface used to perform a single Assets request

#### Client Classes
- [MTConnectHttpClient](https://github.com/TrakHound/MTConnect.NET/blob/master/libraries/MTConnect.NET-HTTP/Clients/MTConnectHttpClient.cs) : Reads from MTConnect Agents using the MTConnect HTTP REST Api. Supports both polling and streaming. Supports compression. Supports XML & JSON. Implements IMTConnectClient and IMTConnectEntityClient.
- [MTConnectMqttClient](https://github.com/TrakHound/MTConnect.NET/blob/master/libraries/MTConnect.NET-MQTT/Clients/MTConnectMqttClient.cs) : Reads MTConnect data from an MQTT Broker. Supports the latest MTConnect MQTT Protocol. Implements IMTConnectClient and IMTConnectEntityClient.
- [MTConnectMqttExpandedClient](https://github.com/TrakHound/MTConnect.NET/blob/master/libraries/MTConnect.NET-MQTT/Clients/MTConnectMqttExpandedClient.cs) : Reads MTConnect entities from an MQTT Broker. Implements IMTConnectEntityClient.

#### Usage
Code that handles the client events can be written against the interfaces so that it works with either transport.
```c#
using MTConnect.Clients;

void Subscribe(IMTConnectClient client)
{
    client.ProbeReceived += (sender, document) => Console.WriteLine("Probe Received");
    client.CurrentReceived += (sender, document) => Console.WriteLine("Current Received");
    client.SampleReceived += (sender, document) => Console.WriteLine("Sample Received");
    client.AssetsReceived += (sender, document) => Console.WriteLine("Assets Received");
    client.Start();
}
```
