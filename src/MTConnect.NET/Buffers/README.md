# Buffers
Buffers are used to store data that is written to or read from an MTConnectAgent. There are three type of Buffers:
- Device Buffer : Used to store Device Model Information such as what is returned with a Probe request
- Streaming Buffer : Used to store Observations (Samples, Events, and Conditions)
- Assets Buffer : Used to store Asset data

## Device Buffer
Device Buffers must implement the IDeviceBuffer interface. A Device Buffer can be an in-memory buffer such as the [DeviceBuffer](DeviceBuffer.cs) class or a permanent storage source such as from a SQL database.

## Streaming Buffer
Streaming Buffers must implement the IStreamingBuffer interface. A Streaming Buffer can be an in-memory circular buffer such as the [StreamingBuffer](StreamingBuffer.cs) class or a permanent storage source such as from a SQL database. The Streaming Buffer is responsible for keeping up with the current Sequence numbers that is reported in the Header of an MTConnectStreams Response Document.

## Assets Buffer
Assets Buffers must implement the IAssetsBuffer interface. An Assets Buffer can be an in-memory hash-table buffer such as the [AssetsBuffer](AssetsBuffer.cs) class or a permanent storage source such as from a SQL database.
