# MQTT to HiveMQ

## Overview
- HiveMQ Setup
  - [Getting Started](https://www.hivemq.com/docs/hivemq/4.12/user-guide/getting-started.html#get-started)
- MQTT Explorer
  - [Download](https://mqtt-explorer.com/) - Download the MQTT Explorer app to browse the MQTT broker
  - [GitHub](https://github.com/thomasnordquist/MQTT-Explorer)

## Relay Agent Module
- [GitHub](https://github.com/TrakHound/MTConnect.NET/tree/master/applications/Agents/MTConnect-Agent-MQTT-Relay)

### Example Configuration (agent.config.yaml)
```yaml
modules:
  - mqtt-relay:

    # The hostname of the MQTT broker to publish messages to
    server: 56497654654651654654987.s1.eu.hivemq.cloud
    
    # The port number of the MQTT broker to publish messages to
    port: 8883

    username: exampleuser
    password: examplepassword
    useTls: true
```
