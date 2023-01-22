# MTConnect.NET Agent Applications

### Recommended (General purpose Windows or Linux)

- [MTConnect HTTP Agent](https://github.com/TrakHound/MTConnect.NET/tree/master/applications/Agents/MTConnect-Agent-Http) : This MTConnect Agent application is fully compatible with the latest Version 2.1 of the MTConnect Standard. It uses the SHDR protocol to receive data from Adapters, an in-memory buffer with an optional durable file system based buffer, and an Http REST interface for retrieving data.

- [MTConnect HTTP Gateway Agent](https://github.com/TrakHound/MTConnect.NET/tree/master/applications/Agents/MTConnect-Agent-Http-Gateway) : This MTConnect Agent application is fully compatible with the latest Version 2.1 of the MTConnect Standard. It receives data from other MTConnect Agents using HTTP, an in-memory buffer with an optional durable file system based buffer, and an Http REST interface for retrieving data.

- [MTConnect MQTT Relay Agent](https://github.com/TrakHound/MTConnect.NET/tree/master/applications/Agents/MTConnect-Agent-MQTT-Relay) : This MTConnect Agent application is fully compatible with the latest Version 2.1 of the MTConnect Standard. It uses the SHDR protocol to receive data from Adapters, an in-memory buffer with an optional durable file system based buffer, and an MQTT client to publish messages to an external MQTT Broker.

- [MTConnect MQTT Broker Agent](https://github.com/TrakHound/MTConnect.NET/tree/master/applications/Agents/MTConnect-Agent-MQTT-Broker) : This MTConnect Agent application is fully compatible with the latest Version 2.1 of the MTConnect Standard. It uses the SHDR protocol to receive data from Adapters, an in-memory buffer with an optional durable file system based buffer, and a built-in MQTT broker.

### Specialized (For use with IIS)

- [MTConnect HTTP Agent - AspNetCore](https://github.com/TrakHound/MTConnect.NET/tree/master/applications/Agents/MTConnect-Agent-Http-AspNetCore) : Similar to the MTConnect Agent application but uses either the built-in Kestrel server or can be setup through IIS (Internet Information Services). This allows the agent to be used with all of the features available through ASP.NET and IIS such as security, permissions, monitoring, etc.

- [MTConnect HTTP Gateway Agent - AspNetCore](https://github.com/TrakHound/MTConnect.NET/tree/master/applications/Agents/MTConnect-Agent-Http-Gateway-AspNetCore) : An Agent that runs mulitple MTConnectClients on the backend and passes that data to an MTConnectAgent. This can be used to access MTConnect data on a central server. Uses either the built-in Kestrel server or can be setup through IIS (Internet Information Services). This allows the agent to be used with all of the features available through ASP.NET and IIS such as security, permissions, monitoring, etc.
