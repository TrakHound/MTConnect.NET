# Buffers
Buffers are used to store data that is written to or read from an MTConnectAgent. There are three type of Buffers:
- Device Buffer : Used to store Device Model Information such as what is returned with a Probe request
- Observation Buffer : Used to store Observations (Samples, Events, and Conditions)
- Assets Buffer : Used to store Asset data

## Device Buffer
Device Buffers must implement the IDeviceBuffer interface. A Device Buffer can be an in-memory buffer such as the [MTConnectDeviceBuffer](MTConnectDeviceBuffer.cs) class or a permanent storage source such as from a SQL database.

## Observation Buffer
Observation Buffers must implement the IObservationBuffer interface. An Observation Buffer can be an in-memory circular buffer such as the [MTConnectObservationBuffer](MTConnectObservationBuffer.cs) class or a permanent storage source such as from a SQL database. The Observation Buffer is responsible for keeping up with the current Sequence numbers that is reported in the Header of an MTConnectStreams Response Document.

### Observation File Buffer
The [MTConnectObservationFileBuffer](MTConnectObservationFileBuffer.cs) class is used to save the buffer to the local File System in order to retain the buffer between application restarts.

#### Buffer Directory Structures
Buffer files are stored in the **/buffer** directory located in the same directory as the executing assembly. Observation buffer files are then stored under the **/observations** directory. Current Observation buffer files are handled separately and are stored in the **/current** directory.

##### /buffer/observations
![image](https://user-images.githubusercontent.com/13772118/191876160-f00bd982-4e35-4133-b27d-9a8a2cb0481b.png)

##### /buffer/observations/current
![image](https://user-images.githubusercontent.com/13772118/191876480-6ec6a8c2-427a-4b1e-9d4f-0d2fd07e8617.png)



## Asset Buffer
Asset Buffers must implement the IAssetsBuffer interface. An Asset Buffer can be an in-memory hash-table buffer such as the [MTConnectAssetBuffer](MTConnectAssetBuffer.cs) class or a permanent storage source such as from a SQL database.

### Asset File Buffer
The [MTConnectAssetFileBuffer](MTConnectAssetFileBuffer.cs) class is used to save the buffer to the local File System in order to retain the buffer between application restarts.
