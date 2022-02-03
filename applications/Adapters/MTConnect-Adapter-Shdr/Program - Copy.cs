using MTConnect.Streams;
using MTConnect.Streams.Events;
using MTConnect.Streams.Samples;
using MTConnect.Observations;
using MTConnect.Adapters.Shdr;

namespace MTConnect.Applications.Adapters.Shdr
{
    class Program
    {
        static ShdrAdapter _adapter;
        static Simulator.DeviceSimulator _simulator;

        static double xabs = 0;
        static double xpos = 123.4567;

        static async Task Main(string[] args)
        {
            var cancel = new CancellationTokenSource();

            var rnd = new Random();
            var i = 0;

            var deviceName = "M12346";

            _simulator = new Simulator.DeviceSimulator(deviceName);

            _adapter = new ShdrAdapter(deviceName);
            _adapter.Interval = 100;
            _adapter.AgentConnected += (sender, connectionId) => Console.WriteLine($"Agent Connection (ID = {connectionId}) : Agent Connected");
            _adapter.AgentDisconnected += (sender, connectionId) => Console.WriteLine($"Agent Connection (ID = {connectionId}) : Agent Disconnected");
            _adapter.PingReceived += (sender, connectionId) => Console.WriteLine($"Agent Connection (ID = {connectionId}) : Agent Ping Received");
            _adapter.PongSent += (sender, connectionId) => Console.WriteLine($"Agent Connection (ID = {connectionId}) : Pong Sent to Agent");
            _adapter.LineSent += (sender, args) => Console.WriteLine($"Agent Connection (ID = {args.ClientId}) : Line Sent : {args.Message}");


            var timer = new System.Timers.Timer();
            timer.Interval = 10;
            timer.Elapsed += (s, e) =>
            {
                UpdateController();
                UpdatePath();

                UpdateXAxis();
                UpdateYAxis();
                UpdateZAxis();
            };
            timer.Start();

            while (true)
            {
                Console.ReadLine();
                _adapter.Start();
                _simulator.Connect();

                Console.ReadLine();
                _adapter.Stop();
                _simulator.Disconnect();
            }

            Console.ReadLine();
        }

        static void UpdateController()
        {
            var dataItems = new List<ShdrDataItem>();

            // Emergency Stop
            if (_simulator.Controller.EmergencyStop) dataItems.Add(new ShdrDataItem("estop", "TRIGGERED"));
            else dataItems.Add(new ShdrDataItem("estop", "ARMED"));




            _adapter.AddDataItems(dataItems);
        }

        static void UpdatePath()
        {
            var dataItems = new List<ShdrDataItem>();

            // Execution
            switch (_simulator.Path.Execution)
            {
                case 0: dataItems.Add(new ShdrDataItem("execution", "UNAVAILABLE")); break;
                case 1: dataItems.Add(new ShdrDataItem("execution", "READY")); break;
                case 2: dataItems.Add(new ShdrDataItem("execution", "ACTIVE")); break;
            }

            _adapter.AddDataItems(dataItems);
        }

        static void UpdateXAxis()
        {
            var axis = _simulator.LinearAxes[0];
            _adapter.AddDataItem(new ShdrDataItem("Xabs", axis.MachinePosition));
            _adapter.AddDataItem(new ShdrDataItem("Xpos", axis.WorkPosition));
            _adapter.AddDataItem(new ShdrDataItem("Xfrt", axis.Feedrate));
            _adapter.AddDataItem(new ShdrDataItem("Xload", axis.Load));
            _adapter.AddDataItem(new ShdrDataItem("servotemp1", axis.Temperature));

            switch (axis.State)
            {
                case 0: _adapter.AddDataItem(new ShdrDataItem("xaxisstate", AxisState.STOPPED)); break;
                case 1: _adapter.AddDataItem(new ShdrDataItem("xaxisstate", AxisState.HOME)); break;
                case 2: _adapter.AddDataItem(new ShdrDataItem("xaxisstate", AxisState.PARKED)); break;
                case 3: _adapter.AddDataItem(new ShdrDataItem("xaxisstate", AxisState.TRAVEL)); break;
            }
           
            if (axis.Overtravel)
            {
                var condition = new ShdrCondition("Xtravel", ConditionLevel.FAULT);
                condition.Message = "X Axis Overtravel Limit Reached";
                _adapter.AddCondition(condition);
            }
            else
            {
                _adapter.AddCondition(new ShdrCondition("Xtravel", ConditionLevel.NORMAL));
            }
        }

        static void UpdateYAxis()
        {
            var axis = _simulator.LinearAxes[1];
            _adapter.AddDataItem(new ShdrDataItem("Yabs", axis.MachinePosition));
            _adapter.AddDataItem(new ShdrDataItem("Ypos", axis.WorkPosition));
            _adapter.AddDataItem(new ShdrDataItem("Yfrt", axis.Feedrate));
            _adapter.AddDataItem(new ShdrDataItem("Yload", axis.Load));
            _adapter.AddDataItem(new ShdrDataItem("servotemp2", axis.Temperature));

            switch (axis.State)
            {
                case 0: _adapter.AddDataItem(new ShdrDataItem("yaxisstate", AxisState.STOPPED)); break;
                case 1: _adapter.AddDataItem(new ShdrDataItem("yaxisstate", AxisState.HOME)); break;
                case 2: _adapter.AddDataItem(new ShdrDataItem("yaxisstate", AxisState.PARKED)); break;
                case 3: _adapter.AddDataItem(new ShdrDataItem("yaxisstate", AxisState.TRAVEL)); break;
            }

            if (axis.Overtravel)
            {
                var condition = new ShdrCondition("Ytravel", ConditionLevel.FAULT);
                condition.Message = "Y Axis Overtravel Limit Reached";
                _adapter.AddCondition(condition);
            }
            else
            {
                _adapter.AddCondition(new ShdrCondition("Ytravel", ConditionLevel.NORMAL));
            }
        }

        static void UpdateZAxis()
        {
            var axis = _simulator.LinearAxes[2];
            _adapter.AddDataItem(new ShdrDataItem("Zabs", axis.MachinePosition));
            _adapter.AddDataItem(new ShdrDataItem("Zpos", axis.WorkPosition));
            _adapter.AddDataItem(new ShdrDataItem("Zfrt", axis.Feedrate));
            _adapter.AddDataItem(new ShdrDataItem("Zload", axis.Load));
            _adapter.AddDataItem(new ShdrDataItem("servotemp3", axis.Temperature));

            switch (axis.State)
            {
                case 0: _adapter.AddDataItem(new ShdrDataItem("zaxisstate", AxisState.STOPPED)); break;
                case 1: _adapter.AddDataItem(new ShdrDataItem("zaxisstate", AxisState.HOME)); break;
                case 2: _adapter.AddDataItem(new ShdrDataItem("zaxisstate", AxisState.PARKED)); break;
                case 3: _adapter.AddDataItem(new ShdrDataItem("zaxisstate", AxisState.TRAVEL)); break;
            }

            if (axis.Overtravel)
            {
                var condition = new ShdrCondition("Ztravel", ConditionLevel.FAULT);
                condition.Message = "Z Axis Overtravel Limit Reached";
                _adapter.AddCondition(condition);
            }
            else
            {
                _adapter.AddCondition(new ShdrCondition("Ztravel", ConditionLevel.NORMAL));
            }
        }



        static void AddDataItems()
        {
            var rnd = new Random();
            var inches = UnitSystem.INCH;
            var dataItems = new List<ShdrDataItem>();

            dataItems.Add(new ShdrDataItem("avail", Availability.AVAILABLE));
            dataItems.Add(new ShdrDataItem("functionalmode", FunctionalMode.PRODUCTION));

            dataItems.Add(new ShdrDataItem("Xabs", new PositionValue(xabs++)));
            dataItems.Add(new ShdrDataItem("Xpos", new PositionValue(xpos++)));
            dataItems.Add(new ShdrDataItem("Xload", new LoadValue(42.5)));
            dataItems.Add(new ShdrDataItem("servotemp1", new TemperatureValue(35.481)));
 
            _adapter.AddDataItems(dataItems);
        }

        static void AddConditions()
        {
            var condition = new ShdrCondition("L2p1system", ConditionLevel.FAULT, DateTime.UtcNow);
            condition.NativeCode = "404";
            condition.NativeSeverity = "100";
            condition.Qualifier = "LOW";
            condition.Message = "Testing from new adapter";

            _adapter.AddCondition(condition);


            var condition1 = new ShdrCondition("L2p2system", ConditionLevel.WARNING);
            condition1.NativeCode = "113";
            condition1.Qualifier = "LOW";
            condition1.Message = "Coolant Low";

            _adapter.AddCondition(condition1);
        }

        static void AddTimeSeries()
        {
            var samples = new List<double>();
            samples.Add(12);
            samples.Add(15);
            samples.Add(14);
            samples.Add(18);
            samples.Add(25);
            samples.Add(30);

            var timeSeries = new ShdrTimeSeries("L2p1Sensor", samples, 100, DateTime.UtcNow);

            _adapter.AddTimeSeries(timeSeries);
        }

        static void AddDataSets()
        {
            var dataSetEntries = new List<DataSetEntry>();
            dataSetEntries.Add(new DataSetEntry("V1", 5));
            dataSetEntries.Add(new DataSetEntry("V2", 205));

            var dataSet = new ShdrDataSet("L2p1Variables", dataSetEntries, DateTime.UtcNow);

            _adapter.AddDataSet(dataSet);
        }

        static void AddTables()
        {
            var tableEntries = new List<TableEntry>();

            // Tool 1
            var t1Cells = new List<TableCell>();
            t1Cells.Add(new TableCell("LENGTH", 7.123));
            t1Cells.Add(new TableCell("DIAMETER", 0.494));
            t1Cells.Add(new TableCell("TOOL_LIFE", 0.35));
            tableEntries.Add(new TableEntry("T1", t1Cells));

            // Tool 2
            var t2Cells = new List<TableCell>();
            t2Cells.Add(new TableCell("LENGTH", 10.456));
            t2Cells.Add(new TableCell("DIAMETER", 0.125));
            t2Cells.Add(new TableCell("TOOL_LIFE", 1));
            tableEntries.Add(new TableEntry("T2", t2Cells));

            // Tool 3
            var t3Cells = new List<TableCell>();
            t3Cells.Add(new TableCell("LENGTH", 6.251));
            t3Cells.Add(new TableCell("DIAMETER", 1.249));
            t3Cells.Add(new TableCell("TOOL_LIFE", 0.93));
            tableEntries.Add(new TableEntry("T3", t3Cells));

            var table = new ShdrTable("L2p1ToolTable", tableEntries, DateTime.UtcNow);

            _adapter.AddTable(table);
        }

        static void AddCuttingTools()
        {
            var tool = new Assets.CuttingTools.CuttingToolAsset();
            tool.Description = new Devices.Description
            {
                Manufacturer = "Sandvik",
                Model = "B5632",
                SerialNumber = "12345678946"
            };
            tool.AssetId = "5.12";
            tool.ToolId = "12";
            tool.CuttingToolLifeCycle = new Assets.CuttingTools.CuttingToolLifeCycle
            {
                Location = new Assets.CuttingTools.Location { Type = Assets.CuttingTools.LocationType.SPINDLE },
                ProgramToolNumber = "12",
                ProgramToolGroup = "5"
            };
            tool.CuttingToolLifeCycle.Measurements.Add(new Assets.CuttingTools.Measurements.FunctionalLengthMeasurement(7.6543));
            tool.CuttingToolLifeCycle.Measurements.Add(new Assets.CuttingTools.Measurements.Assembly.CuttingDiameterMaxMeasurement(0.375));
            tool.CuttingToolLifeCycle.CuttingItems.Add(new Assets.CuttingTools.CuttingItem
            {
                ItemId = "12.1",
                Locus = Assets.CuttingTools.CuttingItemLocas.FLUTE.ToString()
            });
            tool.CuttingToolLifeCycle.CutterStatus.Add(Assets.CuttingTools.CutterStatus.AVAILABLE);
            tool.CuttingToolLifeCycle.CutterStatus.Add(Assets.CuttingTools.CutterStatus.NEW);
            tool.CuttingToolLifeCycle.CutterStatus.Add(Assets.CuttingTools.CutterStatus.MEASURED);
            tool.Timestamp = DateTime.Now;

            _adapter.AddAsset(tool);
        }

        static void AddFiles()
        {
            var programFile = new Assets.Files.FileAsset();
            programFile.AssetId = "114905-105-30";
            programFile.Timestamp = DateTime.UtcNow;
            programFile.Size = 1234514;
            programFile.FileLocation = new Assets.Files.FileLocation(@"\\server\114905 - 105 - 30.NC");

            _adapter.AddAsset(programFile);
        }

        static async Task AddRawMaterials()
        {
            var rawMaterial = new Assets.RawMaterials.RawMaterialAsset();
            rawMaterial.AssetId = "789456-A2";
            rawMaterial.Timestamp = DateTime.UtcNow;
            rawMaterial.Name = "6061 Aluminum";
            rawMaterial.SerialNumber = "789456-A2";
            rawMaterial.Material = new Assets.RawMaterials.Material
            {
                Id = "m-al-123456",
                Name = "6061 Aluminum",
                Lot = "B-45",
                Type = "Aluminum"
            };

            _adapter.AddAsset(rawMaterial);
        }

        //static async Task AddPallets()
        //{
        //    // Pallet 5
        //    var pallet5 = new Pallet();
        //    pallet5.PalletId = "5";
        //    pallet5.AssetId = $"{pallet5.PalletId}.1";
        //    pallet5.Timestamp = DateTime.UtcNow;


        //    // Position 1
        //    var position1_5 = new PalletPosition();
        //    position1_5.Id = "1";

        //    var part1_5 = new Part();
        //    part1_5.Id = "114724-106-30";
        //    part1_5.Status = "RAW_MATERIAL";

        //    var part1Program_5 = new Assets.Pallets.Program();
        //    part1Program_5.Location = @"\\server\114905-103-30.NC";
        //    part1Program_5.Comment = "PART=ABCDEFG MOLD=9";
        //    part1_5.Program = part1Program_5;

        //    var position1Fixture_5 = new Fixture();
        //    position1Fixture_5.Id = "SEG_OP30";
        //    position1Fixture_5.Group = "30";

        //    position1_5.Part = part1_5;
        //    position1_5.Fixture = position1Fixture_5;
        //    pallet5.PalletPositions.Add(position1_5);


        //    // Position 2
        //    var position2_5 = new PalletPosition();
        //    position2_5.Id = "2";

        //    var part2_5 = new Part();
        //    part2_5.Id = "114905-105-30";
        //    part2_5.Status = "COMPLETE";

        //    var part2Program_5 = new Assets.Pallets.Program();
        //    part2Program_5.Location = @"\\server\114905-105-30.NC";
        //    part2Program_5.Comment = "PART=ABCDEFG MOLD=9";
        //    part2_5.Program = part2Program_5;

        //    var position2Fixture_5 = new Fixture();
        //    position2Fixture_5.Id = "SEG_OP50";
        //    position2Fixture_5.Group = "50";

        //    position2_5.Part = part2_5;
        //    position2_5.Fixture = position2Fixture_5;
        //    pallet5.PalletPositions.Add(position2_5);


        //    // Position 3
        //    var position3_5 = new PalletPosition();
        //    position3_5.Id = "3";

        //    var position3Fixture_5 = new Fixture();
        //    position3Fixture_5.Id = "SEG_OP50";
        //    position3Fixture_5.Group = "50";

        //    position3_5.Fixture = position3Fixture_5;
        //    pallet5.PalletPositions.Add(position3_5);

        //    _adapter.AddAsset(pallet5);
        //}
    }
}
