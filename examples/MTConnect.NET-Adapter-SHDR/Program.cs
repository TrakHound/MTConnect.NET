using MTConnect.Adapters;
using MTConnect.Observations;
using MTConnect.Observations.Events;
using MTConnect.Shdr;

namespace MTConnect.Adapters.Simulator
{
    // Simulates the twin-spindle, two-path Okuma lathe described in agent/MTConnect.NET-Agent/devices/okuma.xml
    // and outputs its data using the SHDR protocol. The DataItem IDs written below match that Device model.
    //
    // The adapter listens for MTConnect Agent connections (default port 7878). The MTConnect.NET-Agent project
    // is already configured to connect to an SHDR adapter for the "Okuma" device on port 7878 (see ReadMe.md).
    //
    // Intended for testing throughput : the scan rate can be set from a few scans per second up to as fast as possible,
    // every scan is sent to the Agent(s) immediately, and throughput statistics are written to the console every second.
    //
    // Usage: MTConnect.NET-Adapter-SHDR [--port 7878] [--rate 10] [--verbose]
    //   --port    : TCP port to listen for Agent connections on (default 7878)
    //   --rate    : Scans per second (default 10). Use 0 to run as fast as possible
    //   --verbose : Write each SHDR line sent to the console (this significantly reduces throughput)

    internal class Program
    {
        static void Main(string[] args)
        {
            var port = GetArgument(args, "--port", 7878);
            var rate = GetArgument(args, "--rate", 500);
            var verbose = args.Contains("--verbose");

            // ShdrAdapter (without an interval) only sends when SendChanged() is called. Calling it after each scan
            // sends every scan as soon as it is produced instead of being limited by a background send interval.
            var adapter = new ShdrAdapter(port, heartbeat: 10000);
            var agentCount = 0;
            adapter.AgentConnected += (s, clientId) =>
            {
                Interlocked.Increment(ref agentCount);
                Console.WriteLine($"Agent Connected : {clientId}");
            };
            adapter.AgentDisconnected += (s, clientId) =>
            {
                Interlocked.Decrement(ref agentCount);
                Console.WriteLine($"Agent Disconnected : {clientId}");
            };

            // LineSent is raised once per line, per connected Agent
            long linesSent = 0;
            long bytesSent = 0;
            adapter.LineSent += (s, e) =>
            {
                Interlocked.Increment(ref linesSent);
                Interlocked.Add(ref bytesSent, e.Data.Length + 1);
                if (verbose) Console.WriteLine(e.Data);
            };
            adapter.Start();

            var rateText = rate > 0 ? $"{rate} scans/sec" : "max rate";
            Console.WriteLine($"Okuma SHDR Simulator Adapter listening on port {port} @ {rateText} (Press Ctrl+C to exit)");

            using var stop = new CancellationTokenSource();
            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                stop.Cancel();
            };

            // Path 1 (main spindle S1, X1/Z1 axes) and Path 2 (sub spindle S2, X2/Z2 axes) run independently
            var path1 = new PathSimulator(1, new[] { "O1001", "O1002", "O2050" }, seed: 1);
            var path2 = new PathSimulator(2, new[] { "O5001", "O5002" }, seed: 2);

            var period = rate > 0 ? TimeSpan.FromSeconds(1 / rate) : TimeSpan.Zero;
            var clock = System.Diagnostics.Stopwatch.StartNew();
            var lastScan = clock.Elapsed;
            var nextScan = clock.Elapsed;
            var nextStats = TimeSpan.FromSeconds(1);
            var lastStatsTime = TimeSpan.Zero;
            long scans = 0;
            long lastStatsScans = 0, lastStatsLines = 0, lastStatsBytes = 0;

            while (!stop.IsCancellationRequested)
            {
                // Advance the simulation by the actual elapsed time so the machine behaves the same at any scan rate
                var now = clock.Elapsed;
                var elapsed = (now - lastScan).TotalSeconds;
                lastScan = now;
                path1.Step(elapsed);
                path2.Step(elapsed);

                // Only add observations while an Agent is connected. Values added with no Agent connected are
                // not retained as "last sent", so a newly connected Agent would never receive unchanged values
                // (ex. Availability or Program) until they change.
                if (Volatile.Read(ref agentCount) > 0)
                {
                    Write(adapter, path1, path2, elapsed);
                    adapter.SendChanged();
                    scans++;
                }

                // Write throughput statistics every second
                if (now >= nextStats)
                {
                    var seconds = (now - lastStatsTime).TotalSeconds;
                    var lines = Interlocked.Read(ref linesSent);
                    var bytes = Interlocked.Read(ref bytesSent);
                    var memory = Environment.WorkingSet / 1024 / 1024;
                    Console.WriteLine($"Scans/sec : {(scans - lastStatsScans) / seconds,8:0} | Lines/sec : {(lines - lastStatsLines) / seconds,8:0} | KB/sec : {(bytes - lastStatsBytes) / 1024.0 / seconds,8:0.0} | Agents : {Volatile.Read(ref agentCount)} | Memory : {memory} MB");

                    lastStatsScans = scans;
                    lastStatsLines = lines;
                    lastStatsBytes = bytes;
                    lastStatsTime = now;
                    nextStats = now + TimeSpan.FromSeconds(1);
                }

                WaitForNextScan(clock, ref nextScan, period, stop.Token);
            }

            adapter.SetUnavailable();
            adapter.SendChanged();
            adapter.Stop();
        }

        private static void WaitForNextScan(System.Diagnostics.Stopwatch clock, ref TimeSpan nextScan, TimeSpan period, CancellationToken cancellationToken)
        {
            if (period <= TimeSpan.Zero) return;

            nextScan += period;

            // If too far behind (ex. the requested rate can't be met), resync instead of bursting to catch up
            var now = clock.Elapsed;
            if (now - nextScan > TimeSpan.FromSeconds(1)) nextScan = now;

            // Thread.Sleep has a resolution of ~15ms on Windows, so sleep while the wait is long and yield for the remainder
            while (!cancellationToken.IsCancellationRequested)
            {
                var remaining = nextScan - clock.Elapsed;
                if (remaining <= TimeSpan.Zero) break;
                else if (remaining > TimeSpan.FromMilliseconds(20)) Thread.Sleep(1);
                else Thread.Yield();
            }
        }

        private static T GetArgument<T>(string[] args, string name, T defaultValue)
        {
            var index = Array.IndexOf(args, name);
            if (index >= 0 && index + 1 < args.Length)
            {
                try { return (T)Convert.ChangeType(args[index + 1], typeof(T), System.Globalization.CultureInfo.InvariantCulture); }
                catch { }
            }
            return defaultValue;
        }

        private static void Write(ShdrAdapter adapter, PathSimulator path1, PathSimulator path2, double elapsed)
        {
            // Use a single timestamp for every observation in a scan so the SHDR output is consolidated into a single line
            var ts = DateTime.UtcNow;

            adapter.AddDataItem("L2avail", Availability.AVAILABLE, ts);
            adapter.AddDataItem("L2estop", path1.EmergencyStop || path2.EmergencyStop ? EmergencyStop.TRIGGERED : EmergencyStop.ARMED, ts);

            WritePath(adapter, path1, ts);
            WritePath(adapter, path2, ts);
            WritePath1Extras(adapter, path1, ts, elapsed);
        }

        private static void WritePath(ShdrAdapter adapter, PathSimulator path, DateTime ts)
        {
            var p = $"L2p{path.Number}";  // Path DataItems
            var s = $"L2S{path.Number}";  // Spindle DataItems
            var x = $"L2X{path.Number}";  // X Axis DataItems
            var z = $"L2Z{path.Number}";  // Z Axis DataItems

            adapter.AddDataItem($"L2f{path.Number}mode", FunctionalMode.PRODUCTION, ts);
            adapter.AddDataItem($"{p}mode", path.Mode, ts);
            adapter.AddDataItem($"{p}execution", path.Execution, ts);
            adapter.AddDataItem($"{p}program", path.Program, ts);
            adapter.AddDataItem($"{p}ProgramHeader", path.ProgramHeader, ts);
            adapter.AddDataItem($"{p}block", path.Block, ts);
            adapter.AddDataItem($"{p}line", path.Line, ts);
            adapter.AddDataItem($"{p}partcount", path.PartCount, ts);
            adapter.AddDataItem($"{p}Fact", Math.Round(path.FeedrateActual, 1), ts);
            adapter.AddDataItem($"{p}Fcmd", Math.Round(path.FeedrateCommanded, 1), ts);
            adapter.AddDataItem($"{p}Fovr", path.FeedrateOverride, ts);
            adapter.AddDataItem($"{p}LPathPos", $"{path.XWork:0.000} 0.000 {path.ZWork:0.000}", ts);
            adapter.AddDataItem($"{p}CurrentTool", path.ToolNumber, ts);
            adapter.AddDataItem($"{p}ToolAssetId", path.ToolAssetId, ts);

            adapter.AddDataItem($"{p}TotalOperatingTime", Math.Round(path.TotalOperatingTime), ts);
            adapter.AddDataItem($"{p}TotalRunningTime", Math.Round(path.TotalRunningTime), ts);
            adapter.AddDataItem($"{p}TotalCuttingTime", Math.Round(path.TotalCuttingTime), ts);
            adapter.AddDataItem($"{p}TotalSpindleRunTime", Math.Round(path.TotalSpindleRunTime), ts);
            adapter.AddDataItem($"{p}OperatingTime", Math.Round(path.OperatingTime), ts);
            adapter.AddDataItem($"{p}RunningTime", Math.Round(path.RunningTime), ts);
            adapter.AddDataItem($"{p}CuttingTime", Math.Round(path.CuttingTime), ts);
            adapter.AddDataItem($"{p}SpindleRunTime", Math.Round(path.SpindleRunTime), ts);

            adapter.AddDataItem($"{s}speed", Math.Round(path.SpindleSpeed), ts);
            adapter.AddDataItem($"{s}cmd", Math.Round(path.SpindleCommanded), ts);
            adapter.AddDataItem($"{s}ovr", path.SpindleOverride, ts);
            adapter.AddDataItem($"{s}load", Math.Round(path.SpindleLoad, 1), ts);
            adapter.AddDataItem($"{s}Mode", RotaryMode.SPINDLE, ts);
            adapter.AddDataItem($"{s}ChuckState", path.ChuckState, ts);

            adapter.AddDataItem($"{x}actm", Math.Round(path.XMachine, 4), ts);
            adapter.AddDataItem($"{x}actw", Math.Round(path.XWork, 4), ts);
            adapter.AddDataItem($"{x}load", Math.Round(path.XLoad, 1), ts);
            adapter.AddDataItem($"{z}actm", Math.Round(path.ZMachine, 4), ts);
            adapter.AddDataItem($"{z}actw", Math.Round(path.ZWork, 4), ts);
            adapter.AddDataItem($"{z}load", Math.Round(path.ZLoad, 1), ts);

            // Condition : send NORMAL to clear, otherwise send the active FAULT or WARNING
            var system = new ShdrCondition($"{p}system", ConditionLevel.NORMAL, ts);
            if (path.Alarm != null)
            {
                system = new ShdrCondition($"{p}system");
                system.AddFault(ts, path.Alarm.Value.Text, path.Alarm.Value.Code);
            }
            else if (path.SpindleLoad > 85)
            {
                system = new ShdrCondition($"{p}system");
                system.AddWarning(ts, "Spindle load high", "S100", qualifier: ConditionQualifier.HIGH);
            }
            adapter.AddCondition(system);
        }

        private static void WritePath1Extras(ShdrAdapter adapter, PathSimulator path, DateTime ts, double elapsed)
        {
            // Surface speed (mm/s) = PI * Diameter (mm) * RPM / 60
            adapter.AddDataItem("L2S1SurfaceSpeed", Math.Round(Math.PI * path.XWork * path.SpindleSpeed / 60, 1), ts);

            // TIME_SERIES : vibration sensor sampled at 1 kHz over the time since the last scan
            var sampleRate = 1000.0;
            var sampleCount = Math.Clamp((int)(sampleRate * elapsed), 1, 1000);
            adapter.AddTimeSeries(new ShdrTimeSeries("L2p1Sensor", path.SampleVibration(sampleCount, sampleRate), sampleRate, ts));

            // DATA_SET : tool offsets (wear grows as parts are cut)
            var toolOffsets = new List<IDataSetEntry>
            {
                new DataSetEntry("T1_X", Math.Round(path.ToolWear(1), 4)),
                new DataSetEntry("T1_Z", Math.Round(path.ToolWear(1) / 2, 4)),
                new DataSetEntry("T3_X", Math.Round(path.ToolWear(3), 4)),
                new DataSetEntry("T3_Z", Math.Round(path.ToolWear(3) / 2, 4))
            };
            adapter.AddDataSet(new ShdrDataSet("L2p1Variables", toolOffsets, ts));

            // TABLE : work offsets
            var workOffsets = new List<ITableEntry>
            {
                new TableEntry("G54", new ITableCell[] { new TableCell("X", PathSimulator.WorkOffsetX), new TableCell("Z", PathSimulator.WorkOffsetZ) }),
                new TableEntry("G55", new ITableCell[] { new TableCell("X", 0.0), new TableCell("Z", -250.0) })
            };
            adapter.AddTable(new ShdrTable("L2p1ToolTable", workOffsets, ts));
        }
    }


    /// <summary>
    /// A simple state machine that produces believable data for one path of a CNC lathe running turning programs.
    /// </summary>
    internal class PathSimulator
    {
        public const double WorkOffsetX = 0.0;
        public const double WorkOffsetZ = -320.0;

        private const int Passes = 6;
        private const double StockDiameter = 80;
        private const double FinishDiameter = 50;
        private const double CutLength = 100;
        private const double HomeX = 200;
        private const double HomeZ = 50;

        private readonly Random _rnd;
        private readonly string[] _programs;
        private double _cycleTime;
        private double _cycleDuration;
        private double _idleRemaining;
        private double _alarmRemaining;


        public int Number { get; }

        public bool EmergencyStop { get; private set; }
        public ControllerMode Mode { get; private set; } = ControllerMode.AUTOMATIC;
        public Execution Execution { get; private set; } = Execution.READY;
        public string Program { get; private set; }
        public string ProgramHeader => $"({Program} - TURN SHAFT D{FinishDiameter} L{CutLength})";
        public string Block { get; private set; } = "";
        public int Line { get; private set; }
        public int PartCount { get; private set; }
        public double FeedrateActual { get; private set; }
        public double FeedrateCommanded { get; private set; }
        public int FeedrateOverride { get; private set; } = 100;
        public int ToolNumber { get; private set; } = 1;
        public string ToolAssetId => ToolNumber == 1 ? $"P{Number}-T01-ROUGH" : $"P{Number}-T03-FINISH";

        public double SpindleSpeed { get; private set; }
        public double SpindleCommanded { get; private set; }
        public int SpindleOverride { get; private set; } = 100;
        public double SpindleLoad { get; private set; }
        public ChuckState ChuckState { get; private set; } = ChuckState.CLOSED;

        public double XWork { get; private set; } = HomeX;
        public double ZWork { get; private set; } = HomeZ;
        public double XMachine => XWork + WorkOffsetX;
        public double ZMachine => ZWork + WorkOffsetZ;
        public double XLoad { get; private set; }
        public double ZLoad { get; private set; }

        // Accumulated times in seconds
        public double TotalOperatingTime { get; private set; }
        public double TotalRunningTime { get; private set; }
        public double TotalCuttingTime { get; private set; }
        public double TotalSpindleRunTime { get; private set; }
        public double OperatingTime { get; private set; }
        public double RunningTime { get; private set; }
        public double CuttingTime { get; private set; }
        public double SpindleRunTime { get; private set; }

        public (string Code, string Text)? Alarm { get; private set; }


        public PathSimulator(int number, string[] programs, int seed)
        {
            Number = number;
            _programs = programs;
            _rnd = new Random(seed);
            Program = programs[0];
            _idleRemaining = 2 + number;

            // Start the lifetime totals at believable values
            TotalOperatingTime = 3_600_000 * (4 + number);
            TotalRunningTime = TotalOperatingTime * 0.6;
            TotalCuttingTime = TotalOperatingTime * 0.4;
            TotalSpindleRunTime = TotalOperatingTime * 0.45;
        }


        public void Step(double seconds)
        {
            if (Alarm != null) StepAlarm(seconds);
            else if (Execution == Execution.ACTIVE) StepCycle(seconds);
            else StepIdle(seconds);

            TotalOperatingTime += seconds;
            OperatingTime += seconds;
            if (Execution == Execution.ACTIVE)
            {
                TotalRunningTime += seconds;
                RunningTime += seconds;
            }
            if (FeedrateActual > 0)
            {
                TotalCuttingTime += seconds;
                CuttingTime += seconds;
            }
            if (SpindleSpeed > 0)
            {
                TotalSpindleRunTime += seconds;
                SpindleRunTime += seconds;
            }
        }

        public double ToolWear(int tool) => (tool == 1 ? 0.0004 : 0.0001) * PartCount;

        public IEnumerable<double> SampleVibration(int count, double sampleRate)
        {
            var amplitude = SpindleSpeed > 0 ? 0.2 + SpindleLoad / 100 : 0.01;
            var frequency = SpindleSpeed / 60;
            var samples = new double[count];
            for (var i = 0; i < count; i++)
            {
                var t = _cycleTime + i / sampleRate;
                samples[i] = Math.Round(amplitude * Math.Sin(2 * Math.PI * frequency * t) + (_rnd.NextDouble() - 0.5) * 0.05, 4);
            }
            return samples;
        }


        private void StepCycle(double seconds)
        {
            _cycleTime += seconds;
            var progress = _cycleTime / _cycleDuration;

            if (progress >= 1)
            {
                // Cycle complete : retract to home and stop the spindle
                PartCount++;
                Execution = Execution.PROGRAM_COMPLETED;
                Block = "M30";
                Line = 0;
                StopMotion();
                _idleRemaining = 4 + _rnd.NextDouble() * 4;
                return;
            }

            // Randomly trigger an alarm (~1 per 300 seconds of cutting)
            if (_rnd.NextDouble() < 0.0033 * seconds)
            {
                var alarms = new[] { ("1012", "Servo overload"), ("2033", "Coolant level low"), ("4021", "Tool breakage detected") };
                Alarm = alarms[_rnd.Next(alarms.Length)];
                Execution = Execution.STOPPED;
                EmergencyStop = Alarm.Value.Code == "1012";
                StopMotion();
                _alarmRemaining = 10 + _rnd.NextDouble() * 10;
                return;
            }

            // Turning passes : each pass feeds Z along the part at a smaller diameter
            var passPosition = progress * Passes;
            var pass = (int)passPosition;
            var passProgress = passPosition - pass;
            var finishing = pass >= Passes - 2;

            ToolNumber = finishing ? 3 : 1;
            FeedrateOverride = finishing ? 100 : (_cycleTime % 40 < 20 ? 100 : 110);
            SpindleOverride = 100;

            XWork = StockDiameter - (StockDiameter - FinishDiameter) * (pass + 1) / Passes;
            ZWork = -CutLength * passProgress;

            SpindleCommanded = finishing ? 2200 : 1600;
            SpindleSpeed = SpindleCommanded * SpindleOverride / 100.0 + (_rnd.NextDouble() - 0.5) * 10;
            SpindleLoad = (finishing ? 30 : 55) + Math.Sin(_cycleTime / 2) * 15 + _rnd.NextDouble() * 20;

            // Feedrate in mm/min (feed per rev * RPM) to match the nativeUnits in the Device model
            FeedrateCommanded = (finishing ? 0.1 : 0.25) * SpindleCommanded;
            FeedrateActual = FeedrateCommanded * FeedrateOverride / 100.0;

            XLoad = (finishing ? 10 : 25) + _rnd.NextDouble() * 8;
            ZLoad = (finishing ? 15 : 35) + _rnd.NextDouble() * 10;

            Line = 10 + pass * 20 + (int)(passProgress * 20);
            Block = $"G01 X{XWork:0.000} Z{-CutLength:0.000} F{(finishing ? 0.1 : 0.25):0.00}";
        }

        private void StepIdle(double seconds)
        {
            _idleRemaining -= seconds;

            // Open the chuck to unload/load a part between cycles
            if (Execution == Execution.PROGRAM_COMPLETED)
            {
                ChuckState = _idleRemaining > 1.5 ? ChuckState.OPEN : ChuckState.CLOSED;
            }

            if (_idleRemaining > 0) return;

            // Start a new cycle (occasionally switch programs)
            if (Execution == Execution.PROGRAM_COMPLETED && _rnd.NextDouble() < 0.2)
            {
                Program = _programs[_rnd.Next(_programs.Length)];
            }

            ChuckState = ChuckState.CLOSED;
            Mode = ControllerMode.AUTOMATIC;
            Execution = Execution.ACTIVE;
            _cycleTime = 0;
            _cycleDuration = 40 + _rnd.NextDouble() * 20;
        }

        private void StepAlarm(double seconds)
        {
            _alarmRemaining -= seconds;

            // Operator switches to manual to recover from the alarm
            Mode = ControllerMode.MANUAL;

            if (_alarmRemaining > 0) return;

            Alarm = null;
            EmergencyStop = false;
            Mode = ControllerMode.AUTOMATIC;
            Execution = Execution.READY;
            Block = "";
            Line = 0;
            _idleRemaining = 5;
        }

        private void StopMotion()
        {
            FeedrateActual = 0;
            FeedrateCommanded = 0;
            SpindleSpeed = 0;
            SpindleCommanded = 0;
            SpindleLoad = 0;
            XLoad = 0;
            ZLoad = 0;
            XWork = HomeX;
            ZWork = HomeZ;
        }
    }
}
