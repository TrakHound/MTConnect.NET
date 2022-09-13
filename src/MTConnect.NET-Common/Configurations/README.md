## Configurations

Configurations are used set default parameters for Agents, Adapters, and Clients. Configuration files are written in JSON (other formats will be supported in the future). Agent Configurations are designed to inherity from AgentConfiguration.cs with any additional properties being set in the derived class.

#### Agent Configuration Parameters

* `ObservationBufferSize` - The maximum number of Observations the agent can hold in its buffer

* `AssetBufferSize` - The maximum number of assets the agent can hold in its buffer

* `IgnoreTimestamps` - Overwrite timestamps with the agent time. This will correct clock drift but will not give as accurate relative time since it will not take into consideration network latencies. This can be overridden on a per adapter basis.

* `DefaultVersion` - Sets the default MTConnect version to output response documents for.

* `ConvertUnits` - Sets the default for Converting Units when adding Observations

* `IgnoreObservationCase` - Sets the default for Ignoring the case of Observation values. Applicable values will be converted to uppercase

* `InputValidationLevel` - Sets the default response document validation level. 0 = Ignore, 1 = Warning, 2 = Strict
