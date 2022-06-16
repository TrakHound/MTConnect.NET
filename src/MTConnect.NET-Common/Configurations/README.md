## Configurations

Configurations are used set default parameters for Agents, Adapters, and Clients. Configuration files are written in JSON (other formats will be supported in the future). Agent Configurations are designed to inherity from AgentConfiguration.cs with any additional properties being set in the derived class.

#### Agent Configuration Parameters

* `ObservationBufferSize` - The maximum number of Observations the agent can hold in its buffer

* `AssetBufferSize` - The maximum number of assets the agent can hold in its buffer

* `Devices` - The XML file to load that specifies the devices and is supplied as the result of a probe request.

* `ServiceName` - Changes the service name when installing or removing the service. This allows multiple agents to run as services on the same machine.

* `ServiceAutoStart` - Sets the Service Start Type. True = Auto | False = Manual

* `IgnoreTimestamps` - Overwrite timestamps with the agent time. This will correct clock drift but will not give as accurate relative time since it will not take into consideration network latencies. This can be overridden on a per adapter basis.

* `DefaultVersion` - Sets the default MTConnect version to output response documents for.

* `ConvertUnits` - Sets the default for Converting Units when adding Observations

* `IgnoreObservationCase` - Sets the default for Ignoring the case of Observation values. Applicable values will be converted to uppercase

* `ValidationLevel` - Sets the default response document validation level. 0 = Ignore, 1 = Warning, 2 = Strict

* `IndentOutput` - Sets the default response document indendation

* `OutputComments` - Sets the default response document comments output. Comments contain descriptions from the MTConnect standard

* `MonitorConfigurationFiles` - Sets whether Configuration files are monitored. If enabled and a configuration file is changed, the Agent will restart

* `ConfigurationFileRestartInterval` - Gets or Sets the minimum time (in seconds) between Agent restarts when MonitorConfigurationFiles is enabled
