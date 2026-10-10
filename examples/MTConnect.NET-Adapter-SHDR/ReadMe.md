# SHDR Adapter Simulator Example
This is an example of an MTConnect Adapter that outputs simulated data using the SHDR protocol. It simulates the twin-spindle, two-path Okuma lathe described by the [okuma.xml](../../agent/MTConnect.NET-Agent/devices/okuma.xml) Device model in the MTConnect.NET-Agent project. Each path repeatedly runs turning programs, with tool changes, chuck open/close between parts, occasional alarms, and spindle load warnings.

This application is intended for testing throughput. The scan rate can be set from a few scans per second up to as fast as possible, every scan is sent to the connected Agent(s) immediately, and throughput statistics are written to the console every second.

The adapter uses the `ShdrAdapter` class from the [MTConnect.NET-SHDR](https://www.nuget.org/packages/MTConnect.NET-SHDR) package, which listens for MTConnect Agent connections. `SendChanged()` is called after each scan so that the send rate is not limited by a background send interval (the interval based adapters wait using `Task.Delay`, which has a resolution of ~15ms on Windows).

## Run the Adapter
Use a Release build when measuring throughput:
```
dotnet run --project examples/MTConnect.NET-Adapter-SHDR -c Release -- [--port 7878] [--rate 10] [--verbose]
```

| Option | Default | Description |
| ------ | ------- | ----------- |
| --port | 7878 | TCP port to listen for Agent connections on |
| --rate | 10 | Scans per second. Use `0` to run as fast as possible |
| --verbose | | Write each SHDR line sent to the console (this significantly reduces throughput) |

The simulation advances by the actual time elapsed between scans, so the machine behaves the same at any rate; a higher rate produces finer-grained samples (positions, loads, spindle speed, etc.). Rates above ~50 scans/sec keep one CPU core busy waiting between scans to maintain accurate timing.

Throughput statistics are written every second (lines and bytes are counted per connected Agent):
```
Scans/sec :     1000 | Lines/sec :     2000 | KB/sec :    349.3 | Agents : 1 | Memory : 85 MB
```

With `--verbose`, each SHDR line is also written, for example:
```
2026-10-03T14:21:07.1230000Z|L2p1execution|ACTIVE|L2p1line|56|L2p1Fact|400|L2S1speed|1596|L2X1actw|65|L2Z1actw|-33.6933
2026-10-03T14:21:07.1230000Z|L2p1system|WARNING|S100||HIGH|Spindle load high
```

## Connect an Agent
The [MTConnect.NET-Agent](../../agent/MTConnect.NET-Agent) project is already configured for this: its `agent.config.yaml` loads `devices/okuma.xml` and has an `shdr-adapter` module for the `Okuma` device on port 7878:
```yaml
devices: devices

modules:

- http-server:
    port: 5000

- shdr-adapter:
    deviceKey: Okuma
    hostname: localhost
    port: 7878
```

Start the Agent and this adapter, then browse to http://localhost:5000/Okuma/current to see the simulated data.

## Simulated Data
DataItems are written by their `id` in okuma.xml. `p1`/`S1`/`X1`/`Z1` belong to Path 1 (main spindle) and `p2`/`S2`/`X2`/`Z2` to Path 2 (sub spindle); both paths run independently.

| DataItem Id | Type | Description |
| ----------- | ---- | ----------- |
| L2avail | AVAILABILITY | AVAILABLE while running (UNAVAILABLE on shutdown) |
| L2estop | EMERGENCY_STOP | TRIGGERED during a servo overload alarm |
| L2f*n*mode | FUNCTIONAL_MODE | PRODUCTION |
| L2p*n*mode | CONTROLLER_MODE | AUTOMATIC (MANUAL while recovering from an alarm) |
| L2p*n*execution | EXECUTION | READY, ACTIVE, PROGRAM_COMPLETED, STOPPED |
| L2p*n*program / ProgramHeader | PROGRAM / PROGRAM_HEADER | Current part program (changes occasionally) |
| L2p*n*block / line | BLOCK / LINE | Current NC block and line |
| L2p*n*partcount | PART_COUNT | Incremented each completed cycle |
| L2p*n*Fact / Fcmd / Fovr | PATH_FEEDRATE / PATH_FEEDRATE_OVERRIDE | Feedrate (mm/min native) and override |
| L2p*n*LPathPos | PATH_POSITION | Tool tip position in work coordinates |
| L2p*n*CurrentTool / ToolAssetId | TOOL_NUMBER / TOOL_ASSET_ID | Roughing (T1) then finishing (T3) tool |
| L2p*n*...Time | ACCUMULATED_TIME | Operating, running, cutting and spindle run times (seconds) |
| L2p*n*system | SYSTEM (Condition) | FAULT during a random alarm, WARNING when spindle load exceeds 85% |
| L2S*n*speed / cmd / ovr / load | ROTARY_VELOCITY / OVERRIDE / LOAD | Spindle speed, commanded speed, override and load |
| L2S*n*Mode / ChuckState | ROTARY_MODE / CHUCK_STATE | SPINDLE; chuck opens between parts |
| L2S1SurfaceSpeed | SURFACE_SPEED | Calculated from diameter and spindle speed |
| L2X*n*actm / actw / load | POSITION / LOAD | X axis machine and work position, load |
| L2Z*n*actm / actw / load | POSITION / LOAD | Z axis machine and work position, load |
| L2p1Sensor | VARIABLE (TIME_SERIES) | Simulated vibration sampled at 1 kHz (samples since the previous scan) |
| L2p1Variables | TOOL_OFFSET (DATA_SET) | Tool wear offsets that grow with part count |
| L2p1ToolTable | WORK_OFFSET (TABLE) | G54/G55 work offsets |

## Notes
Observations are only added while an Agent is connected. Values added with no Agent connected are not kept as the last sent values, so an Agent that connects later would not receive values that have not changed since (ex. Availability or Program).
