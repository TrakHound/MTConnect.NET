# Cookbook

Recipes for the deployment patterns that come up most often. Each recipe is task-driven — pick the goal, follow the steps, end with a working configuration.

## Available recipes

- **Write your first agent** — install the NuGet package, scaffold a `Program.cs` with `MTConnectAgentApplication`, point a consumer at `/probe` and `/current`. See [Getting started](/getting-started) for the full walkthrough.
- **Write your first adapter** — install `MTConnect.NET-Applications-Adapter`, pick the right `ShdrAdapter` variant for your data-flow pattern (`ShdrAdapter`, `ShdrIntervalAdapter`, `ShdrQueueAdapter`, `ShdrIntervalQueueAdapter`), feed values from your PLC reader into it. See [Configure an adapter](/configure/adapter).
- **Write a custom agent module** — subclass `MTConnectAgentModule` or `MTConnectInputAgentModule`, wire a config block into `agent.config.yaml`, load the module by ID. The standalone agent application picks it up automatically through the module-loader.
- **Configure the MQTT relay module** — enable `mqtt-relay` in `agent.config.yaml`, point it at an external broker, define the topic template, set the availability topic. See the [MQTT protocol overview](/configure/integrations/mqtt-protocol) for the topic tree.
- **Write a JSON-MQTT consumer** — subscribe to the relay's topic tree, parse each message with the `MTConnect.NET-JSON-cppagent` codec, dispatch by topic prefix. .NET sample uses `MTConnectMqttClient`; Python sample uses `paho-mqtt` with a hand-rolled parser following the JSON-CPPAGENT v2 envelope shape.
- **Migrate from JSON v1 to JSON-CPPAGENT v2** — the v2 codec is the cppagent-parity target; the v1 codec ships for compatibility with consumers that pre-date the spec's JSON formalization. See [Wire formats](/wire-formats/) for the envelope-level comparison.
- **Add a custom DataItem type** — define the type, the controlled-vocabulary subType (if any), and the units in your `Devices.xml`. The agent validates the declaration against the spec's controlled vocabularies and registers the DataItem at startup.
- **Persist Assets to disk** — assets outlive observations and need a storage layer. Configure the agent's asset buffer to back to the file system; the buffer survives restarts.
- **Bridge an MTConnect agent to InfluxDB or Grafana** — see the [InfluxDB integration](/configure/integrations/influxdb) page.
- **Run the agent under a Windows service or systemd unit** — see [Run](/configure/run).

## Recipe shape

Every cookbook page follows the same structure: **goal**, **prerequisites**, **steps** (numbered, with code blocks), **verification** (the curl / log / file-system check that proves it worked), **what to read next**.

## See also

- [Configure & Use](/configure/) — the reference behind the recipes.
- [API reference](/api/) — every type the recipes mention.
- [Examples](/examples/) — runnable sample applications that mirror the patterns in these recipes.
- [Troubleshooting](/troubleshooting/) — what to do when a recipe doesn't yield the expected result.
