# Cookbook

Recipes for the deployment patterns that come up most often. Each recipe is task-driven — pick the goal, follow the steps, end with a working configuration.

## Available recipes

- **[Write an agent](/cookbook/write-an-agent)** — build a Device model in code, create an `MTConnectAgentBroker`, push observations, and serve `/probe`, `/current`, and `/sample` over HTTP. The [embedded agent](/cookbook/write-an-agent#embedded-agent) section covers the alternative shape: host `MTConnectAgentApplication` in your own process and feed it from an `MTConnectInputAgentModule` (the `examples/MTConnect.NET-Agent-Embedded` sample). See [Getting started](/getting-started) for the shortest path to a running agent.
- **Write your first adapter** — install `MTConnect.NET-Applications-Adapter`, pick the right `ShdrAdapter` variant for your data-flow pattern (`ShdrAdapter`, `ShdrIntervalAdapter`, `ShdrQueueAdapter`, `ShdrIntervalQueueAdapter`), feed values from your PLC reader into it. See [Configure an adapter](/configure/adapter-config).
- **Write a custom agent module** — subclass `MTConnectAgentModule` or `MTConnectInputAgentModule`, wire a config block into `agent.config.yaml`, load the module by ID. The standalone agent application picks it up automatically through the module-loader.
- **Configure the MQTT relay module** — enable `mqtt-relay` in `agent.config.yaml`, point it at an external broker, define the topic template, set the availability topic. See the [MQTT protocol overview](/wire-formats/mqtt) for the topic tree.
- **[Write a JSON-MQTT consumer](/cookbook/write-a-json-mqtt-consumer)** — subscribe to the relay's topic tree, parse each message with the `MTConnect.NET-JSON-cppagent` codec, dispatch by topic prefix. The .NET samples use MQTTnet directly or the library's `MTConnectMqttClient` (the `examples/MTConnect.NET-Client-MQTT` sample); the Python sample uses `paho-mqtt` with a hand-rolled parser following the JSON-CPPAGENT v2 envelope shape.
- **Migrate from JSON v1 to JSON-CPPAGENT v2** — the v2 codec is the cppagent-parity target; the v1 codec ships for compatibility with consumers that pre-date the spec's JSON formalization. See [Wire formats](/wire-formats/) for the envelope-level comparison.
- **Add a custom DataItem type** — define the type, the controlled-vocabulary subType (if any), and the units in your `Devices.xml`. The agent validates the declaration against the spec's controlled vocabularies and registers the DataItem at startup.
- **Persist Assets to disk** — assets outlive observations and need a storage layer. Configure the agent's asset buffer to back to the file system; the buffer survives restarts.
- **Bridge an MTConnect agent to InfluxDB or Grafana** — see the [InfluxDB integration](/configure/integrations/#influxdb) page.
- **Run the agent under a Windows service or systemd unit** — see [Run](/configure/run).

## Recipe shape

Every cookbook page follows the same structure: **goal**, **prerequisites**, **steps** (numbered, with code blocks), **verification** (the curl / log / file-system check that proves it worked), **what to read next**.

## See also

- [Configure & Use](/configure/) — the reference behind the recipes.
- [API reference](/api/) — every type the recipes mention.
- [Examples](/examples/) — runnable sample applications that mirror the patterns in these recipes.
- [Troubleshooting](/troubleshooting/) — what to do when a recipe doesn't yield the expected result.
