![MTConnect.NET Logo](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/logo.png) 

# MTConnect Web UI Agent Module
This Agent Module hosts a browser user interface for the MTConnect.NET Agent it runs in. It can be used to:

* View Agent information, buffer state, and the Agent's Devices
* Browse Devices / Components and their current Observations (with auto refresh)
* View the recent history of any Data Item
* Browse Assets and view them as MTConnectAssets XML
* View the validation problems the Agent has reported
* Edit the Agent configuration file (`agent.config.yaml`) and the Device files

## Configuration
```yaml
modules:
  - web-ui:
      server: localhost
      port: 5080
      allowConfigurationEdit: true
```

* `server` - The hostname to bind to. Defaults to `localhost` so the UI is only reachable from the Agent machine. Set to `*` to listen on all interfaces (on Windows this requires administrator rights or a `netsh http add urlacl` reservation).

* `port` - The port the UI listens on. Default is `5080`.

* `allowConfigurationEdit` - Sets whether the Agent configuration file and the Device files can be saved from the UI. Default is `true`. When `false` they are shown read-only.

Open `http://localhost:5080/` in a browser once the Agent is running.

The page and its fonts (Roboto and Roboto Mono, licensed under the SIL Open Font License 1.1) are embedded in the module assembly, so the UI makes no requests outside the Agent and works on networks without internet access.

## Pages

| Page | Description |
| --- | --- |
| **Overview** | The Agent's UUID, instance ID, sender and versions, the buffer's size and sequences, each Device with its number of Data Items and Observations since the Agent started, and a summary of validation problems by Device |
| **Data** | A tree of Devices and Components with the current Observations of the selected one (sortable, filterable, with auto refresh). Select an Observation to view its history |
| **Assets** | The Agent's Assets, filtered by Device. Select an Asset to view it as MTConnectAssets XML |
| **Validation** | The problems the Agent has reported since it started, filtered by Device, kind, and text |
| **Configuration** | Editors for the Agent configuration file and the Device files |

When the Agent has an `http-server` module, the pages also link to its `probe`, `current`, `sample`, and `assets` response documents.

### History
History is read from the Agent's buffer (up to 1000 Observations of a Data Item), so older Observations are lost as new ones arrive.

### Validation
The Validation page lists invalid Devices, Components, Compositions, Data Items, Assets, and Observation values. Repeats of the same problem are counted on one row, and up to 1000 problems are kept. They are cleared when the Agent restarts.

Observation values are only checked when `enableValidation: true` is set in the Agent configuration. The Data page then also flags invalid and deprecated values.

## Editing the configuration
The Configuration page has three editors:

* **Settings** - A form for the Agent's settings and for each Module and Processor entry. Entries can be added and removed, and **Use default** removes a setting so its default is used.
* **Device files** - A tree of each Device file (the `devices` setting), to add, move, remove, and edit Components, Compositions, and Data Items. A new Device file can be created when `devices` is a folder.
* **YAML** - The whole configuration file as text.

Only a YAML configuration file can be edited. A configuration file that can't be shown as a form is opened in the YAML editor.

### Applying changes
Saving validates the file, keeps a copy of the previous file next to it with `.bak` added to its name (ex. `agent.config.yaml.bak`), and writes the new file. A save from the Settings form or a Device file is refused if the file was changed on disk after it was loaded in the UI.

When `monitorConfigurationFiles` is enabled (the default) the Agent detects the change and restarts itself to apply it; otherwise restart the Agent manually. A new Device file is always read on the next restart of the Agent.

> **Security** - The UI has no authentication. Anyone who can reach it can change the Agent configuration and the Device files. Keep `server: localhost` or set `allowConfigurationEdit: false` when exposing it on a network.

## More than one Agent
The Web UI shows the Agent it runs in, and only the Observations still in that Agent's buffer. To serve the Devices of many Agents from a single endpoint, or to store their Observations in SQL Server and publish them to MQTT, see [TrakHound MTConnectServer](https://www.trakhound.com/mtconnect-server).
