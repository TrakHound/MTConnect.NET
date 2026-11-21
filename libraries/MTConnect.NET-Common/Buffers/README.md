# Buffers
Buffers are used to store data that is written to or read from an MTConnectAgentBroker. The MTConnectAgentBroker takes two types of Buffers:
- Observation Buffer : Used to store Observations (Samples, Events, and Conditions)
- Asset Buffer : Used to store Asset data

Both are passed to the MTConnectAgentBroker constructor. If they are not specified, the in-memory MTConnectObservationBuffer and MTConnectAssetBuffer classes are used.
```c#
using MTConnect.Agents;
using MTConnect.Buffers;
using MTConnect.Configurations;

var configuration = AgentConfiguration.Read();

var observationBuffer = new MTConnectObservationFileBuffer(configuration);
observationBuffer.Load();

var assetBuffer = new MTConnectAssetFileBuffer(configuration);
assetBuffer.Load();

var agent = new MTConnectAgentBroker(configuration, observationBuffer, assetBuffer);
```

> Device Model Information (such as what is returned with a Probe request) is not stored in a Buffer. Devices are held by the MTConnectAgent class itself. The [IMTConnectDeviceBuffer](IMTConnectDeviceBuffer.cs) interface and [MTConnectDeviceBuffer](MTConnectDeviceBuffer.cs) class exist in this directory but are not used by the Agent classes.

## Observation Buffer
Observation Buffers must implement the [IMTConnectObservationBuffer](IMTConnectObservationBuffer.cs) interface. An Observation Buffer can be an in-memory circular buffer such as the [MTConnectObservationBuffer](MTConnectObservationBuffer.cs) class or a permanent storage source such as from a SQL database. The Observation Buffer is responsible for keeping up with the current Sequence numbers that are reported in the Header of an MTConnectStreams Response Document.

### Observation File Buffer
The [MTConnectObservationFileBuffer](MTConnectObservationFileBuffer.cs) class is used to save the buffer to the local File System in order to retain the buffer between application restarts.

#### Buffer Directory Structures
By default, Buffer files are stored in the **/buffer** directory located in the application's base directory (`AppDomain.CurrentDomain.BaseDirectory`). A different directory can be set using the optional `basePath` constructor parameter (a relative path is resolved against the application's base directory). Observation buffer files are then stored under the **/observations** directory. Current Observation buffer files are handled separately and are stored in the **/observations/current** directory.

##### /buffer/observations
![image](https://user-images.githubusercontent.com/13772118/191876160-f00bd982-4e35-4133-b27d-9a8a2cb0481b.png)

##### /buffer/observations/current
![image](https://user-images.githubusercontent.com/13772118/191876480-6ec6a8c2-427a-4b1e-9d4f-0d2fd07e8617.png)



## Asset Buffer
Asset Buffers must implement the [IMTConnectAssetBuffer](IMTConnectAssetBuffer.cs) interface. An Asset Buffer can be an in-memory buffer such as the [MTConnectAssetBuffer](MTConnectAssetBuffer.cs) class or a permanent storage source such as from a SQL database.

### Asset File Buffer
The [MTConnectAssetFileBuffer](MTConnectAssetFileBuffer.cs) class is used to save the buffer to the local File System in order to retain the buffer between application restarts. Asset buffer files are stored under the **/buffer/assets** directory.
