![MTConnect.NET Logo](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/logo.png)

# MTConnect Fanuc Adapter Agent Module
This Agent Module reads data directly from a Fanuc CNC using FOCAS over Ethernet, so no separate adapter is required. The paths, axes and spindles are read from the CNC when it connects and the Device is built to match.

Supported controls: Series 0i, 16i, 18i, 21i, 30i, 31i, 32i and 35i (Embedded Ethernet or Fast Ethernet with FOCAS enabled).

## FOCAS Libraries
The Fanuc FOCAS libraries are **not included** and must be obtained from Fanuc (FOCAS2 library). Copy the libraries next to the Agent executable:

* 64-bit Agent: `Fwlib64.dll` and the 64-bit series libraries (ex. `fwlib30i64.dll`, `fwlib0iD64.dll`, `fwlibe64.dll`)
* 32-bit Agent: `Fwlib32.dll` and the 32-bit series libraries (ex. `fwlib30i.dll`, `fwlib0iD.dll`, `fwlibe1.dll`)

If the libraries are missing, an error is logged and the Device stays `UNAVAILABLE`.

## CNC Setup
Enable FOCAS on the CNC's Ethernet port (Embedded Ethernet / FOCAS2 setting screen) and note the TCP port (typically `8193`).

## Configuration
```yaml
modules:
  - fanuc-adapter:
      deviceKey: M1
      server: 192.168.1.70
      port: 8193
```

* `deviceKey` - The UUID or Name of the Device that observations are added to.

* `createDevice` - When `true` (default), the module adds a Device named `deviceKey` and updates it with the model, paths, axes and spindles read from the CNC. Set to `false` to use a Device defined in a Devices file instead.

* `deviceUuid` - The UUID of the Device created when `createDevice` is `true`. Defaults to `deviceKey`.

* `server` - The IP address or hostname of the CNC.

* `port` - The FOCAS port of the CNC. Default is `8193`.

* `responseTimeout` - The FOCAS communication timeout (in milliseconds, rounded up to whole seconds). Default is `5000`.

* `readInterval` - The interval (in milliseconds) between reads of the CNC. Default is `1000`.

* `slowReadInterval` - The interval (in milliseconds) between reads of the timers and the work offset. Default is `10000`.

* `workPositions` - When `true`, absolute (work coordinate) positions are read in addition to machine positions. Default is `false`.

* `axisLoads` - When `true` (default), the servo load of each axis is read.

* `spindleLoad` - When `true` (default), the spindle load is read.

* `overrides` - When `true` (default), the feedrate, rapid and spindle overrides are read from the CNC interface signals (G0012, G0014, G0030, G0096).

* `toolNumberVariable` - The custom macro variable read for the tool number. Default is `4120` (the modal T code). Many machines store the tool in the spindle in another variable; set it here, or `0` to disable.

* `workOffset` - When `true` (default), the active work offset is read from macro variable #4014.

* `partCountParameter` - The parameter read for the part count. Default is `6711` (parts machined). Set to `0` to disable.

## DataItems
If you use your own Device (`createDevice: false`), it needs DataItems whose `id` or `name` match the keys below. Path keys are used as-is for path 1 and prefixed with `p{n}_` for other paths (ex. `p2_execution`). Axis keys use the axis name read from the CNC, including its suffix (ex. `x_act`, `x1_act`).

### Read every cycle (`readInterval`)
| Key | DataItem | Source |
| --- | --- | --- |
| `avail` | AVAILABILITY | `AVAILABLE` while connected, otherwise `UNAVAILABLE` |
| `estop` | EMERGENCY_STOP | `cnc_statinfo` |
| `mode` | CONTROLLER_MODE | `cnc_statinfo` (MEM / RMT = `AUTOMATIC`, MDI = `MANUAL_DATA_INPUT`, EDIT = `EDIT`, JOG / HANDLE / INC / REF = `MANUAL`) |
| `execution` | EXECUTION | `cnc_statinfo` (reset = `READY`, START = `ACTIVE`, HOLD = `FEED_HOLD`, STOP = `STOPPED`, MSTR = `INTERRUPTED`; `STOPPED` in emergency stop) |
| `program` | PROGRAM | `cnc_exeprgname` (or `cnc_rdprgnum` on older controls) |
| `line` | LINE_LABEL | `cnc_rdseqnum` (sequence number N) |
| `partCount` | PART_COUNT | `cnc_rdparam` 6711 |
| `tool` | TOOL_NUMBER | `cnc_rdmacro` 4120 |
| `feedrate` | PATH_FEEDRATE (ACTUAL) | `cnc_rdspeed` (mm/s; feed per revolution is converted using the spindle speed) |
| `feedOverride` | PATH_FEEDRATE_OVERRIDE (PROGRAMMED) | G0012 (%) |
| `rapidOverride` | PATH_FEEDRATE_OVERRIDE (RAPID) | G0014 / G0096 (%) |
| `system` | SYSTEM (Condition) | `cnc_rdalmmsg2`: one `FAULT` per active alarm (native code ex. `PS0010`), `WARNING` for battery / fan / other warnings |
| `speed` | ROTARY_VELOCITY (ACTUAL) | `cnc_rdspeed` (or `cnc_acts`) (RPM) |
| `spindleLoad` | LOAD | `cnc_rdspmeter` (%) |
| `spindleOverride` | ROTARY_VELOCITY_OVERRIDE | G0030 (%) |
| `{axis}_act` | POSITION / ANGLE (ACTUAL) | `cnc_rdposition` machine position (mm or degrees) |
| `{axis}_work` | POSITION / ANGLE (ACTUAL) | `cnc_rdposition` absolute position (`workPositions: true`), referencing the Device's `work` coordinate system |
| `{axis}_load` | LOAD | `cnc_rdsvmeter` (%) |

### Read every `slowReadInterval`
| Key | DataItem | Source |
| --- | --- | --- |
| `firmware` | FIRMWARE (VERSION) | `cnc_sysinfo` (series and version) |
| `powerOnTime` | EQUIPMENT_TIMER (POWERED) | `cnc_rdtimer` 0 (seconds) |
| `operatingTime` | EQUIPMENT_TIMER (OPERATING) | `cnc_rdtimer` 1 (seconds) |
| `cuttingTime` | EQUIPMENT_TIMER (WORKING) | `cnc_rdtimer` 2 (seconds) |
| `cycleTime` | PROCESS_TIMER (PROCESS) | `cnc_rdtimer` 3 (seconds) |
| `workOffset` | WORK_OFFSET | `cnc_rdmacro` 4014 (ex. `54`) |

The CNC model (ex. `30i-M`) from `cnc_sysinfo` is used for the Device Description.

## Notes
* FOCAS functions the CNC does not support (or that need an option that is not installed) are logged once and not read again until the CNC reconnects.
* The tool number, work offset and overrides depend on the custom macro option and the standard CNC interface signals. Check the values against the CNC screens when setting up a new machine.

## Contribution / Feedback
- Please use the [Issues](https://github.com/TrakHound/MTConnect.NET/issues) tab to create issues for specific problems that you may encounter
- Please feel free to use the [Pull Requests](https://github.com/TrakHound/MTConnect.NET/pulls) tab for any suggested improvements to the source code
- For any other questions or feedback, please contact TrakHound directly at **info@trakhound.com**.

## License
This application and its source code is licensed under the [MIT License](https://choosealicense.com/licenses/mit/) and is free to use. The Fanuc FOCAS libraries are licensed separately by Fanuc.
