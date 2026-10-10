![MTConnect.NET Logo](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/logo.png)

# MTConnect Haas Adapter Agent Module
This Agent Module reads data directly from a Haas CNC control over Ethernet or RS-232 using Haas Q-commands, so no separate adapter is required.

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

### Common
* `deviceKey` - The UUID or Name of the Device that observations are added to.

* `createDevice` - When `true` (default), the module adds a Device named `deviceKey` containing the DataItems listed below. Set to `false` to use a Device defined in a Devices file instead.

* `deviceUuid` - The UUID of the Device created when `createDevice` is `true`. Defaults to `deviceKey`.

* `connection` - `ethernet` (default) or `serial`.

* `readInterval` - The interval (in milliseconds) between reads of the control. Default is `1000`.

* `responseTimeout` - The maximum time (in milliseconds) to wait for a reply to a command. Default is `2000`.

* `units` - The units the control reports axis positions in: `mm` (default) or `inch` (Setting 9). Positions reported in inches are converted to millimeters.

### Ethernet
* `server` - The hostname or IP address of the Haas control.

* `port` - The TCP port of the control (Setting 143). Default is `5051`.

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

| Key | DataItem | Source |
| --- | --- | --- |
| `avail` | AVAILABILITY | `AVAILABLE` while the control replies to `?Q100`, otherwise `UNAVAILABLE` |
| `mode` | CONTROLLER_MODE | `?Q104` |
| `execution` | EXECUTION | `?Q500` (IDLE = `READY`, BUSY = `ACTIVE`, FEED HOLD = `INTERRUPTED`, ALARM ON = `STOPPED`) |
| `program` | PROGRAM | `?Q500` (empty in MDI) |
| `partCount` | PART_COUNT | `?Q500` (M30 counter) |
| `system` | SYSTEM (Condition) | `?Q500` (`FAULT` while ALARM ON) |
| `x_act` | POSITION (ACTUAL) | `?Q600 5021` (X machine position, mm) |
| `y_act` | POSITION (ACTUAL) | `?Q600 5022` (Y machine position, mm) |
| `z_act` | POSITION (ACTUAL) | `?Q600 5023` (Z machine position, mm) |
| `speed` | ROTARY_VELOCITY (ACTUAL) | `?Q600 3027` (spindle speed, RPM) |

The control only reports the program name, part count and alarm state while it is not running a program (`?Q500` returns `STATUS, BUSY` while running), so these values update when the program stops.

