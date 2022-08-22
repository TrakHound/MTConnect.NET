# Buffers
Buffers are used to store data that is written to or read from an MTConnectAgent. There are three type of Buffers:
- Device Buffer : Used to store Device Model Information such as what is returned with a Probe request
- Observation Buffer : Used to store Observations (Samples, Events, and Conditions)
- Assets Buffer : Used to store Asset data

## Device Buffer
Device Buffers must implement the IDeviceBuffer interface. A Device Buffer can be an in-memory buffer such as the [MTConnectDeviceBuffer](MTConnectDeviceBuffer.cs) class or a permanent storage source such as from a SQL database.

## Observation Buffer
Observation Buffers must implement the IObservationBuffer interface. An Observation Buffer can be an in-memory circular buffer such as the [MTConnectObservationBuffer](MTConnectObservationBuffer.cs) class or a permanent storage source such as from a SQL database. The Observation Buffer is responsible for keeping up with the current Sequence numbers that is reported in the Header of an MTConnectStreams Response Document.

## Asset Buffer
Asset Buffers must implement the IAssetsBuffer interface. An Asset Buffer can be an in-memory hash-table buffer such as the [MTConnectAssetBuffer](MTConnectAssetBuffer.cs) class or a permanent storage source such as from a SQL database.
