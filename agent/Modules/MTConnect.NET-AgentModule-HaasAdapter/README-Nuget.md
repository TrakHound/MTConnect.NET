![MTConnect.NET Logo](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/logo.png)

# MTConnect Haas Adapter Agent Module
This Agent Module reads data directly from a Haas CNC control (including the Haas Next Generation Control) over Ethernet or RS-232 using Haas Machine Data Collection (Q-commands), so no separate adapter is required.

> Haas NGC controls also include a built-in MTConnect Agent on port 8082 (software 100.20.000.1200 or higher recommended), which can be read using the `http-adapter` module.

## Control Setup
Enable Machine Data Collection by setting **Setting 143** to the TCP port to use (ex. 5051). Do not use ports 8082 or 9090 - 9999, which are used by the control.

## Configuration

### Ethernet
```yaml
modules:
  - haas-adapter:
      deviceKey: VF2
      server: 192.168.1.60
      port: 5051
```

### RS-232
```yaml
modules:
  - haas-adapter:
      deviceKey: VF2
      connection: serial
      comPort: COM1
      baudRate: 19200
      parity: None
      dataBits: 7
      stopBits: 1
```

### 5-Axis with Work Positions
```yaml
modules:
  - haas-adapter:
      deviceKey: UMC-750
      server: 192.168.1.61
      units: inch
      axes: [X, Y, Z, A, C]
      workPositions: true
```

### Common
* `deviceKey` - The UUID or Name of the Device that observations are added to.

* `createDevice` - When `true` (default), the module adds a Device named `deviceKey` containing the DataItems listed below. The Device Description is updated with the model and serial number read from the control. Set to `false` to use a Device defined in a Devices file instead.

* `deviceUuid` - The UUID of the Device created when `createDevice` is `true`. Defaults to `deviceKey`.

* `connection` - `ethernet` (default) or `serial`.

* `readInterval` - The interval (in milliseconds) between reads of the control. Default is `1000`.

* `slowReadInterval` - The interval (in milliseconds) between reads of slowly changing values (timers, counters, software version, work offset, single block / optional stop). Default is `10000`.

* `responseTimeout` - The maximum time (in milliseconds) to wait for a reply to a command. Default is `2000`.

* `units` - The units the control reports linear positions in: `mm` (default) or `inch` (Setting 9). Positions reported in inches are converted to millimeters.

* `axes` - The axes to read positions for: any of `X`, `Y`, `Z`, `A`, `B`, `C`. Default is `[X, Y, Z]`.

* `workPositions` - When `true`, work coordinate positions are read in addition to machine positions. Default is `false`.

* `spindleLoad` - When `true` (default), the spindle load is read (machines with a Haas vector drive).

* `coolantLevel` - When `true`, the coolant level is read (machines with a coolant level sensor). Default is `false`.

### Ethernet
* `server` - The hostname or IP address of the Haas control.

* `port` - The TCP port set in Setting 143. Default is `5051`.

* `connectionTimeout` - The maximum time (in milliseconds) to wait for the TCP connection to open. Default is `5000`.

### RS-232
These must match the RS-232 settings on the control (Settings 11-15).

* `comPort` - The serial port name (ex. `COM1` or `/dev/ttyUSB0`). Default is `COM1`.

* `baudRate` - Default is `19200`.

* `parity` - `None`, `Even`, `Odd`, `Mark` or `Space`. Default is `None`.

* `dataBits` - Default is `7`.

* `stopBits` - `1` or `2`. Default is `1`.

## DataItems
If you use your own Device (`createDevice: false`), it needs DataItems whose `id` or `name` match the keys below.

### Read every cycle (`readInterval`)
| Key | DataItem | Source |
| --- | --- | --- |
| `avail` | AVAILABILITY | `AVAILABLE` while the control replies to `?Q100`, otherwise `UNAVAILABLE` |
| `mode` | CONTROLLER_MODE | `?Q104` |
| `execution` | EXECUTION | `?Q500` (IDLE = `READY`, BUSY = `ACTIVE`, FEED HOLD = `INTERRUPTED`, ALARM ON = `STOPPED`) |
| `program` | PROGRAM | `?Q500` (empty in MDI) |
| `partCount` | PART_COUNT | `?Q500` (M30 counter #1) |
| `system` | SYSTEM (Condition) | `?Q500` (`FAULT` while ALARM ON) |
| `tool` | TOOL_NUMBER | `?Q201` |
| `x_act`, `y_act`, `z_act` | POSITION (ACTUAL) | `?Q600 5021` - `5023` (machine position, mm) |
| `a_act`, `b_act`, `c_act` | ANGLE (ACTUAL) | `?Q600 5024` - `5026` (machine position, degrees) |
| `x_work` ... `c_work` | POSITION / ANGLE (ACTUAL) | `?Q600 5041` - `5046` (`workPositions: true`), referencing the Device's `work` coordinate system |
| `speed` | ROTARY_VELOCITY (ACTUAL) | `?Q600 3027` (spindle speed, RPM) |
| `spindleLoad` | LOAD | `?Q600 1098` (spindle load, %) (`spindleLoad: true`) |

### Read every `slowReadInterval`
| Key | DataItem | Source |
| --- | --- | --- |
| `firmware` | FIRMWARE (VERSION) | `?Q101` (control software version) |
| `powerOnTime` | EQUIPMENT_TIMER (POWERED) | `?Q300` (seconds) |
| `cycleStartTime` | EQUIPMENT_TIMER (OPERATING) | `?Q301` (total cycle start time, seconds) |
| `lastCycleTime` | PROCESS_TIMER (PROCESS) | `?Q303` (seconds) |
| `previousCycleTime` | PROCESS_TIMER (PROCESS) | `?Q304` (seconds) |
| `partCount2` | PART_COUNT | `?Q403` (M30 counter #2) |
| `workOffset` | WORK_OFFSET | `?Q600 4132` (active work offset number, ex. `54`) |
| `singleBlock` | CONTROLLER_MODE_OVERRIDE (SINGLE_BLOCK) | `?Q600 3030` |
| `optionalStop` | CONTROLLER_MODE_OVERRIDE (OPTIONAL_STOP) | `?Q600 3033` |
| `coolantLevel` | FILL_LEVEL | `?Q600 13013` (`coolantLevel: true`) |

The model (`?Q102`) and serial number (`?Q100`) are used for the Device Description.

## Notes
* The control only reports the program name, part count and alarm state while it is not running a program (`?Q500` returns `STATUS, BUSY` while running), so these values update when the program stops.
* Commands the control does not recognize (it replies `?, ?Qxxx`) are not read again until the control reconnects.
* The control sends every reply to every connected client. Replies to other clients' commands are ignored when they can be identified by their label, but macro variable replies (`MACRO, value`) do not identify the variable, so avoid other clients reading macro variables over Machine Data Collection at the same time.
* Feed, rapid and spindle override percentages, current axis loads and active alarm numbers are not available through Machine Data Collection.

