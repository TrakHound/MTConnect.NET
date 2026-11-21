# Agents
There are two Agent classes:
- [MTConnectAgent](MTConnectAgent.cs) : Handles MTConnect Entities (Devices, Observations, and Assets), Unit Conversion, Filtering, etc. Its `GetDevices()` and `GetAssets()` methods return entities (`IEnumerable<IDevice>` and `IEnumerable<IAsset>`), not Response Documents.
- [MTConnectAgentBroker](MTConnectAgentBroker.cs) : Derives from MTConnectAgent and adds the Observation and Asset Buffers. This is the class that implements the MTConnect Standard for processing MTConnect Response Documents (MTConnectDevices, MTConnectStreams, MTConnectAssets, and MTConnectError).

### Example (Initialization)
```c#
using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Buffers;

var configuration = AgentConfiguration.Read();
if (configuration != null)
{
    // Create Observation Buffer
    var observationBuffer = new MTConnectObservationBuffer(configuration);

    // Create Asset Buffer
    var assetBuffer = new MTConnectAssetBuffer(configuration);


    // Create MTConnectAgentBroker
    var agent = new MTConnectAgentBroker(configuration, observationBuffer, assetBuffer);
    agent.Start();
}
```

If the buffers are not specified, in-memory buffers are created from the configuration:
```c#
var agent = new MTConnectAgentBroker(configuration);
```

### Example (Get MTConnectDevices Response Document)
```c#
using MTConnect.Agents;
using MTConnect.Configurations;

var configuration = AgentConfiguration.Read();
var agent = new MTConnectAgentBroker(configuration);
var devicesDocument = agent.GetDevicesResponseDocument();
```

### Example (Get MTConnectStreams Response Document)
```c#
using MTConnect.Agents;
using MTConnect.Configurations;

var configuration = AgentConfiguration.Read();
var agent = new MTConnectAgentBroker(configuration);
var streamsDocument = agent.GetDeviceStreamsResponseDocument();
```

### Example (Get MTConnectAssets Response Document)
```c#
using MTConnect.Agents;
using MTConnect.Configurations;

var configuration = AgentConfiguration.Read();
var agent = new MTConnectAgentBroker(configuration);
var assetsDocument = agent.GetAssetsResponseDocument();
```
