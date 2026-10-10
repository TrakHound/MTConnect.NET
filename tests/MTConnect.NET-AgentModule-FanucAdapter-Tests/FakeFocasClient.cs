// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Fanuc;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.AgentModule.FanucAdapter.Tests
{
    /// <summary>
    /// A simulated Fanuc CNC implementing <see cref="IFocasClient"/>.
    /// </summary>
    public class FakeFocasClient : IFocasClient
    {
        private short _currentPath = 1;
        private bool _connected;


        /// <summary>Simulated paths, in order (path 1 first).</summary>
        public List<FakePath> Paths { get; } = new List<FakePath>();

        /// <summary>The system information.</summary>
        public FocasSystemInfo SystemInfo { get; set; } = new FocasSystemInfo { AddInfo = 2, CncType = "30", MtType = "M", Series = "G421", Version = "15.0" };

        /// <summary>When <c>true</c>, Connect fails with EW_SOCKET.</summary>
        public bool Offline { get; set; }

        /// <summary>When set, every function other than Connect returns this code (ex. EW_SOCKET to simulate a lost connection).</summary>
        public short? FailWith { get; set; }

        /// <summary>Functions that return EW_FUNC.</summary>
        public HashSet<string> Unsupported { get; } = new HashSet<string>();

        /// <summary>Timer values by type.</summary>
        public Dictionary<short, double> Timers { get; } = new Dictionary<short, double> { { 0, 3600 }, { 1, 1800.5 }, { 2, 900 }, { 3, 42.25 } };

        /// <summary>Every function called, in order.</summary>
        public ConcurrentQueue<string> Calls { get; } = new ConcurrentQueue<string>();

        /// <summary>The number of successful connections.</summary>
        public int ConnectCount { get; private set; }

        /// <inheritdoc/>
        public bool IsConnected => _connected;


        /// <summary>Returns how many times a function was called.</summary>
        /// <param name="function">The function name.</param>
        /// <returns>The number of calls.</returns>
        public int CountOf(string function) => Calls.Count(o => o == function);


        /// <summary>Creates a 3-axis machining center with a rotary A axis.</summary>
        /// <returns>A new client.</returns>
        public static FakeFocasClient CreateMill()
        {
            var client = new FakeFocasClient();
            var path = new FakePath { Spindles = new[] { "S1" } };
            path.Axes.Add(new FakeAxis { Name = "X", Machine = 1.5, Absolute = 0.5, Unit = 1, Load = 12.3 });
            path.Axes.Add(new FakeAxis { Name = "Y", Machine = -2.25, Absolute = 0.25, Unit = 1, Load = 4 });
            path.Axes.Add(new FakeAxis { Name = "Z", Machine = 0.1, Absolute = -1, Unit = 1, Load = 20 });
            path.Axes.Add(new FakeAxis { Name = "A", Machine = 90.5, Absolute = 45, Unit = 2, Load = 1 });
            client.Paths.Add(path);
            return client;
        }

        /// <summary>Creates a 2-path lathe (X1, Z1, S1 / X2, Z2, S2).</summary>
        /// <returns>A new client.</returns>
        public static FakeFocasClient CreateTwoPathLathe()
        {
            var client = new FakeFocasClient { SystemInfo = new FocasSystemInfo { AddInfo = 2, CncType = "0", MtType = "T", Series = "D4F1", Version = "30.0" } };

            var path1 = new FakePath { Spindles = new[] { "S1" }, ProgramName = "O1000", Speed = 1200 };
            path1.Axes.Add(new FakeAxis { Name = "X1", Machine = 10, Unit = 0 });
            path1.Axes.Add(new FakeAxis { Name = "Z1", Machine = -5, Unit = 0 });
            client.Paths.Add(path1);

            var path2 = new FakePath { Spindles = new[] { "S2" }, ProgramName = "O2000", Speed = 800, Status = new FocasStatus { Mode = 1, Run = 2 } };
            path2.Axes.Add(new FakeAxis { Name = "X2", Machine = 20, Unit = 0 });
            path2.Axes.Add(new FakeAxis { Name = "Z2", Machine = -7.5, Unit = 0 });
            client.Paths.Add(path2);

            return client;
        }


        /// <inheritdoc/>
        public short Connect(string host, int port, int timeoutSeconds)
        {
            Calls.Enqueue("cnc_allclibhndl3");
            if (Offline) return FocasReturnCodes.EW_SOCKET;

            _connected = true;
            _currentPath = 1;
            ConnectCount++;
            return FocasReturnCodes.EW_OK;
        }

        /// <inheritdoc/>
        public void Disconnect() => _connected = false;

        /// <inheritdoc/>
        public void Dispose() => _connected = false;

        /// <inheritdoc/>
        public short ReadSystemInfo(out FocasSystemInfo info)
        {
            info = SystemInfo;
            return Result("cnc_sysinfo");
        }

        /// <inheritdoc/>
        public short GetPath(out short path, out short maxPath)
        {
            path = _currentPath;
            maxPath = (short)Paths.Count;
            return Result("cnc_getpath");
        }

        /// <inheritdoc/>
        public short SetPath(short path)
        {
            var code = Result("cnc_setpath");
            if (code == FocasReturnCodes.EW_OK) _currentPath = path;
            return code;
        }

        /// <inheritdoc/>
        public short ReadStatus(out FocasStatus status)
        {
            status = Path.Status;
            return Result("cnc_statinfo");
        }

        /// <inheritdoc/>
        public short ReadProgramName(out string name)
        {
            name = Path.ProgramName;
            return Result("cnc_exeprgname");
        }

        /// <inheritdoc/>
        public short ReadProgramNumber(out int running, out int main)
        {
            running = int.Parse(Path.ProgramName.TrimStart('O'));
            main = running;
            return Result("cnc_rdprgnum");
        }

        /// <inheritdoc/>
        public short ReadSequenceNumber(out int number)
        {
            number = Path.SequenceNumber;
            return Result("cnc_rdseqnum");
        }

        /// <inheritdoc/>
        public short ReadParameter(short number, out int value)
        {
            value = number == 6711 ? Path.PartCount : 0;
            return Result("cnc_rdparam");
        }

        /// <inheritdoc/>
        public short ReadMacro(short number, out double? value)
        {
            double v;
            value = Path.Macros.TryGetValue(number, out v) ? v : (double?)null;
            return Result("cnc_rdmacro");
        }

        /// <inheritdoc/>
        public short ReadAxisNames(out string[] names)
        {
            names = Path.Axes.Select(o => o.Name).ToArray();
            return Result("cnc_rdaxisname");
        }

        /// <inheritdoc/>
        public short ReadPositions(out FocasPosition[] positions)
        {
            positions = Path.Axes.Select(o => new FocasPosition { Name = o.Name, Machine = o.Machine, Absolute = o.Absolute, Unit = o.Unit }).ToArray();
            return Result("cnc_rdposition");
        }

        /// <inheritdoc/>
        public short ReadServoLoads(out FocasLoad[] loads)
        {
            loads = Path.Axes.Select(o => new FocasLoad { Name = o.Name, Value = o.Load }).ToArray();
            return Result("cnc_rdsvmeter");
        }

        /// <inheritdoc/>
        public short ReadSpindleNames(out string[] names)
        {
            names = Path.Spindles;
            return Result("cnc_rdspdlname");
        }

        /// <inheritdoc/>
        public short ReadSpindleLoads(out FocasLoad[] loads)
        {
            loads = Path.Spindles.Select(o => new FocasLoad { Name = o, Value = Path.SpindleLoad }).ToArray();
            return Result("cnc_rdspmeter");
        }

        /// <inheritdoc/>
        public short ReadSpeed(out FocasSpeed speed)
        {
            speed = new FocasSpeed { Feedrate = Path.Feedrate, FeedrateUnit = Path.FeedrateUnit, SpindleSpeed = Path.Speed };
            return Result("cnc_rdspeed");
        }

        /// <inheritdoc/>
        public short ReadSpindleSpeed(out int rpm)
        {
            rpm = (int)Path.Speed;
            return Result("cnc_acts");
        }

        /// <inheritdoc/>
        public short ReadTimer(short type, out double seconds)
        {
            Timers.TryGetValue(type, out seconds);
            return Result("cnc_rdtimer");
        }

        /// <inheritdoc/>
        public short ReadAlarms(out FocasAlarm[] alarms)
        {
            alarms = Path.Alarms.ToArray();
            return Result("cnc_rdalmmsg2");
        }

        /// <inheritdoc/>
        public short ReadPmc(short addressType, ushort start, ushort end, out byte[] data)
        {
            data = new byte[end - start + 1];
            var offset = (ushort)((_currentPath - 1) * 1000);
            foreach (var signal in Path.Signals)
            {
                var index = signal.Key + offset - start;
                if (index >= 0 && index < data.Length) data[index] = signal.Value;
            }
            return Result("pmc_rdpmcrng");
        }


        private FakePath Path => Paths[_currentPath - 1];

        private short Result(string function)
        {
            Calls.Enqueue(function);
            if (!_connected) return FocasReturnCodes.EW_HANDLE;
            if (FailWith.HasValue) return FailWith.Value;
            if (Unsupported.Contains(function)) return FocasReturnCodes.EW_FUNC;
            return FocasReturnCodes.EW_OK;
        }
    }

    /// <summary>A simulated CNC path.</summary>
    public class FakePath
    {
        /// <summary>The CNC status.</summary>
        public FocasStatus Status { get; set; } = new FocasStatus { Mode = 1, Run = 3 };
        /// <summary>The axes.</summary>
        public List<FakeAxis> Axes { get; } = new List<FakeAxis>();
        /// <summary>The spindle names.</summary>
        public string[] Spindles { get; set; } = new[] { "S" };
        /// <summary>The executing program.</summary>
        public string ProgramName { get; set; } = "O1234";
        /// <summary>The executing sequence number.</summary>
        public int SequenceNumber { get; set; } = 120;
        /// <summary>The part count (parameter 6711).</summary>
        public int PartCount { get; set; } = 42;
        /// <summary>The actual feedrate.</summary>
        public double Feedrate { get; set; } = 600;
        /// <summary>The feedrate units.</summary>
        public short FeedrateUnit { get; set; }
        /// <summary>The spindle speed.</summary>
        public double Speed { get; set; } = 8000;
        /// <summary>The spindle load.</summary>
        public double SpindleLoad { get; set; } = 35.5;
        /// <summary>The active alarms.</summary>
        public List<FocasAlarm> Alarms { get; } = new List<FocasAlarm>();
        /// <summary>Macro variables.</summary>
        public Dictionary<short, double> Macros { get; } = new Dictionary<short, double> { { 4120, 5 }, { 4014, 54 } };
        /// <summary>CNC interface (G) signals by path 1 address: 100% feed, 50% rapid, 110% spindle.</summary>
        public Dictionary<ushort, byte> Signals { get; } = new Dictionary<ushort, byte> { { 12, unchecked((byte)~100) }, { 14, 1 }, { 30, 110 }, { 96, 0 } };
    }

    /// <summary>A simulated axis.</summary>
    public class FakeAxis
    {
        /// <summary>The axis name.</summary>
        public string Name { get; set; }
        /// <summary>The machine position.</summary>
        public double Machine { get; set; }
        /// <summary>The absolute position.</summary>
        public double Absolute { get; set; }
        /// <summary>The position units (0 = mm, 1 = inch, 2 = degree).</summary>
        public short Unit { get; set; }
        /// <summary>The servo load.</summary>
        public double Load { get; set; }
    }
}
