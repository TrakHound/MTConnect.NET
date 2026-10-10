![MTConnect.NET Logo](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/logo.png)

# MTConnect Tormach Adapter Agent Module
This Agent Module reads data from Tormach CNC machines running PathPilot. PathPilot is based on LinuxCNC and does not provide a network interface for machine data, so the included **PathPilot status server** (`pathpilot/pathpilot_status_server.py`) runs on the controller, reads the machine status using the LinuxCNC Python API, and sends it to this module over TCP.

The status server only reads status. It never sends commands to the machine.

## PathPilot Status Server

### Install
1. Copy `pathpilot_status_server.py` (included in the `pathpilot` folder next to the Agent) to the PathPilot controller, for example using the controller's network file share or a USB drive.
2. Start it on the controller (it uses only Python and the `linuxcnc` module included with PathPilot):
   ```
   python pathpilot_status_server.py
   ```
3. Make sure the controller's network connection used for the Agent (not the machine control Ethernet port) allows TCP connections on the server port (default `7890`).

To start the server automatically, add it to the controller's startup applications. It waits for PathPilot (LinuxCNC) and reports `UNAVAILABLE` while PathPilot is not running.

> PathPilot updates may remove files and startup entries added to the controller. Check the status server is running after updating PathPilot.

### Options
* `--port` - The TCP port to listen on. Default is `7890`.
* `--bind` - The address to listen on. Default is all addresses.
* `--interval` - The interval (in milliseconds) between status messages. Default is `500`.

## Configuration
```yaml
modules:
  - tormach-adapter:
      deviceKey: PCNC1100
      server: 192.168.1.80
      port: 7890
```

* `deviceKey` - The UUID or Name of the Device that observations are added to.

* `createDevice` - When `true` (default), the module adds a Device named `deviceKey` and updates it with the machine name and axes configured in PathPilot. Set to `false` to use a Device defined in a Devices file instead.

* `deviceUuid` - The UUID of the Device created when `createDevice` is `true`. Defaults to `deviceKey`.

* `server` - The IP address or hostname of the PathPilot controller.

* `port` - The port of the PathPilot status server. Default is `7890`.

* `readInterval` - The interval (in milliseconds) between processing the status received. Default is `500`.

* `connectionTimeout` - The maximum time (in milliseconds) to wait for the connection to open. Default is `5000`.

* `dataTimeout` - The time (in milliseconds) without receiving data after which the connection is considered lost and reopened. Default is `5000`.

* `workPositions` - When `true`, work coordinate positions are reported in addition to machine positions. Default is `false`.

## DataItems
If you use your own Device (`createDevice: false`), it needs DataItems whose `id` or `name` match the keys below.

| Key | DataItem | Source (linuxcnc.stat) |
| --- | --- | --- |
| `avail` | AVAILABILITY | `AVAILABLE` while PathPilot is running and the status server is connected |
| `estop` | EMERGENCY_STOP | `estop` |
| `mode` | CONTROLLER_MODE | `task_mode` (AUTO = `AUTOMATIC`, MDI = `MANUAL_DATA_INPUT`, MANUAL = `MANUAL`) |
| `execution` | EXECUTION | `interp_state` (IDLE = `READY`, READING / WAITING = `ACTIVE`, PAUSED = `FEED_HOLD`; `STOPPED` in emergency stop or with the machine off) |
| `program` | PROGRAM | `file` (file name) |
| `line` | LINE_NUMBER (ABSOLUTE) | `current_line` |
| `tool` | TOOL_NUMBER | `tool_in_spindle` |
| `workOffset` | WORK_OFFSET | `g5x_index` (ex. `54`, `59.1`) |
| `optionalStop` | CONTROLLER_MODE_OVERRIDE (OPTIONAL_STOP) | `optional_stop` |
| `feedrate` | PATH_FEEDRATE (ACTUAL) | `current_vel` (mm/s) |
| `feedOverride` | PATH_FEEDRATE_OVERRIDE (PROGRAMMED) | `feedrate` (%) |
| `rapidOverride` | PATH_FEEDRATE_OVERRIDE (RAPID) | `rapidrate` (%) |
| `speed` | ROTARY_VELOCITY (COMMANDED) | spindle speed (RPM, 0 when the spindle is off) |
| `spindleOverride` | ROTARY_VELOCITY_OVERRIDE | spindle override (%) |
| `{axis}_act` | POSITION / ANGLE (ACTUAL) | `actual_position` (machine position, mm or degrees) |
| `{axis}_work` | POSITION / ANGLE (ACTUAL) | `actual_position` minus the work, G92 and tool offsets (`workPositions: true`), referencing the Device's `work` coordinate system |

Axes are read from the PathPilot configuration (`[TRAJ] COORDINATES`, ex. `X Y Z A`). Positions in inch machine units are converted to millimeters.

## Notes
* The spindle speed is the commanded speed; LinuxCNC does not report the measured spindle speed.
* Alarms and part counts are not reported: PathPilot errors are only available on the LinuxCNC error channel (reading it would take them away from PathPilot), and PathPilot's part counter is not part of the LinuxCNC status.
* Work positions do not include rotation of the work coordinate system (G10 L2 R).

## Contribution / Feedback
- Please use the [Issues](https://github.com/TrakHound/MTConnect.NET/issues) tab to create issues for specific problems that you may encounter
- Please feel free to use the [Pull Requests](https://github.com/TrakHound/MTConnect.NET/pulls) tab for any suggested improvements to the source code
- For any other questions or feedback, please contact TrakHound directly at **info@trakhound.com**.

## License
This application and its source code is licensed under the [MIT License](https://choosealicense.com/licenses/mit/) and is free to use.
