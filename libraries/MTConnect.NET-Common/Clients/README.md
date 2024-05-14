# MTConnect Clients

#### Client Interfaces
- [IMTConnectClient](https://github.com/TrakHound/MTConnect.NET/blob/master/libraries/MTConnect.NET-Common/Clients/IMTConnectClient.cs) : Interface used to read MTConnect response documents (Probe, Current, Sample, and Assets)
- [IMTConnectEntityClient](https://github.com/TrakHound/MTConnect.NET/blob/master/libraries/MTConnect.NET-Common/Clients/IMTConnectEntityClient.cs) : Interface used to read MTConnect entities (Device, Observation, Asset)

#### Client Classes
- [MTConnectHttpClient](https://github.com/TrakHound/MTConnect.NET/blob/master/libraries/MTConnect.NET-HTTP/Clients/MTConnectHttpClient.cs) : Reads from MTConnect Agents using the MTConnect HTTP REST Api. Supports both polling and streaming. Supports compression. Supports XML & JSON.
- [MTConnectMqttClient](https://github.com/TrakHound/MTConnect.NET/blob/master/libraries/MTConnect.NET-MQTT/Clients/MTConnectMqttClient.cs) : Reads MTConnect data from an MQTT Broker. Supports the latest MTConnect MQTT Protocol.
