# Agents
The MTConnectAgent class implements the MTConnect Standard for processing MTConnect Response Documents.

### Example (Initialization)
```c#
using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Buffers;

var configuration = AgentConfiguration.Read();
if (configuration != null)
{
    // Create Device Buffer
    var deviceBuffer = new MTConnectDeviceBuffer();

    // Create Observation Buffer
    var observationBuffer = new MTConnectObservationBuffer(configuration);

    // Create Asset Buffer
    var assetBuffer = new MTConnectAssetBuffer(configuration);


    // Create MTConnectAgent
    var agent = new MTConnectAgentBroker(configuration, deviceBuffer, observationBuffer, assetBuffer);
}
```

### Example (Get MTConnectDevices Response Document)
```c#
using MTConnect.Agents;
using MTConnect.Configurations;

var configuration = AgentConfiguration.Read();
var agent = new MTConnectAgentBroker(configuration);
var devicesDocument = agent.GetDevices();
```

### Example (Get MTConnectStreams Response Document)
```c#
using MTConnect.Agents;
using MTConnect.Configurations;

var configuration = AgentConfiguration.Read();
var agent = new MTConnectAgentBroker(configuration);
var streamsDocument = agent.GetDeviceStreams();
```

### Example (Get MTConnectAssets Response Document)
```c#
using MTConnect.Agents;
using MTConnect.Configurations;

var configuration = AgentConfiguration.Read();
var agent = new MTConnectAgentBroker(configuration);
var assetsDocument = agent.GetAssets();
```
