# Agents
The MTConnectAgent class implements the MTConnect® Standard for processing and accessing data.

### Example (Initialization)
```c#
using MTConnect.Agents;
using MTConnect.Agents.Configuration;
using MTConnect.Buffers;

var configuration = MTConnectAgentConfiguration.Read();
if (configuration != null)
{
    // Create Device Buffer
    var deviceBuffer = new MTConnectDeviceBuffer(configuration);

    // Create Streaming Buffer
    var streamingBuffer = new MTConnectStreamingBuffer(configuration);

    // Create Asset Buffer
    var assetBuffer = new MTConnectAssetBuffer(configuration);


    // Create MTConnectAgent
    var agent = new MTConnectAgent(configuration, deviceBuffer, streamingBuffer, assetBuffer);
}
```

### Example (Get MTConnectDevices Response Document)
```c#
using MTConnect.Agents;
using MTConnect.Devices;

IMTConnectAgent agent = new MTConnectAgent(configuration);
DevicesDocument devicesDocument = agent.GetDevices();
```

### Example (Get MTConnectStreams Response Document)
```c#
using MTConnect.Agents;
using MTConnect.Streams;

IMTConnectAgent agent = new MTConnectAgent(configuration);
StreamsDocument streamsDocument = agent.GetDeviceStreams();
```
