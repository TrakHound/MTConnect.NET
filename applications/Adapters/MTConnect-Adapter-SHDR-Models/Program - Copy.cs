using MTConnect.Streams;
using MTConnect.Streams.Events;
using MTConnect.Streams.Samples;
using MTConnect.Observations;
using MTConnect.Adapters.Shdr;
using MTConnect.Models;
using MTConnect.Streams.Events;

namespace MTConnect.Applications.Adapters.ShdrModels
{
    class Program
    {
        static ShdrAdapter _adapter;

        static async Task Main(string[] args)
        {
            var cancel = new CancellationTokenSource();

            var rnd = new Random();
            var i = 0;

            _adapter = new ShdrAdapter("OKUMA.Lathe");
            _adapter.Interval = 100;
            _adapter.AgentConnected += (sender, connectionId) => Console.WriteLine($"Agent Connection (ID = {connectionId}) : Agent Connected");
            _adapter.AgentDisconnected += (sender, connectionId) => Console.WriteLine($"Agent Connection (ID = {connectionId}) : Agent Disconnected");
            _adapter.PingReceived += (sender, connectionId) => Console.WriteLine($"Agent Connection (ID = {connectionId}) : Agent Ping Received");
            _adapter.PongSent += (sender, connectionId) => Console.WriteLine($"Agent Connection (ID = {connectionId}) : Pong Sent to Agent");
            _adapter.LineSent += (sender, args) => Console.WriteLine($"Agent Connection (ID = {args.ClientId}) : Line Sent : {args.Message}");


            var timer = new System.Timers.Timer();
            timer.Interval = 500;
            timer.Elapsed += (s, e) =>
            {
                AddDataItems();

                AddConditions();

                AddTimeSeries();

                AddDataSets();

                AddTables();

                AddCuttingTools();

                AddFiles();
            };
            timer.Start();

            while (true)
            {
                Console.ReadLine();
                _adapter.Start();

                Console.ReadLine();
                _adapter.Stop();
            }

            Console.ReadLine();
        }

        static void AddDataItems()
        {
            var deviceModel = new DeviceModel("OKUMA.Lathe");

            deviceModel.Controller.EmergencyStop = EmergencyStop.ARMED;

            _adapter.AddDataItems(deviceModel.GetObservations());




            var rnd = new Random();
            var inches = UnitSystem.INCH;
            var dataItems = new List<ShdrDataItem>();

            // Device
            dataItems.Add(new ShdrDataItem("L2avail", Availability.AVAILABLE));

            // Rotary Axis C
            dataItems.Add(new ShdrDataItem("L2S1speed", new RotaryVelocityValue(rnd.Next(0, 1000))));
            dataItems.Add(new ShdrDataItem("L2S1cmd", new RotaryVelocityValue(2500)));
            dataItems.Add(new ShdrDataItem("L2S1ovr", new RotaryVelocityOverrideValue(90)));
            dataItems.Add(new ShdrDataItem("L2S1load", new LoadValue(15)));
            dataItems.Add(new ShdrDataItem("L2S1Mode", RotaryMode.SPINDLE));
            dataItems.Add(new ShdrDataItem("L2S1ChuckState", ChuckState.OPEN));
            dataItems.Add(new ShdrDataItem("L2S1SurfaceSpeed", 125.456));

            // Rotary Axis C2
            dataItems.Add(new ShdrDataItem("L2S2speed", new RotaryVelocityValue(12000)));
            dataItems.Add(new ShdrDataItem("L2S2cmd", new RotaryVelocityValue(10000)));
            dataItems.Add(new ShdrDataItem("L2S2ovr", new RotaryVelocityOverrideValue(120)));
            dataItems.Add(new ShdrDataItem("L2S2load", new LoadValue(5)));
            dataItems.Add(new ShdrDataItem("L2S2Mode", RotaryMode.SPINDLE));
            dataItems.Add(new ShdrDataItem("L2S2ChuckState", ChuckState.CLOSED));
            dataItems.Add(new ShdrDataItem("L2S2SurfaceSpeed", 565.2));

            // Linear Axis X
            dataItems.Add(new ShdrDataItem("L2X1actm", new PositionValue(16.0025, inches)));
            dataItems.Add(new ShdrDataItem("L2X1actw", new PositionValue(12.3648, inches)));
            dataItems.Add(new ShdrDataItem("L2X1load", new LoadValue(2.5)));

            // Linear Axis X2
            dataItems.Add(new ShdrDataItem("L2X2actm", new PositionValue(0.00000, inches)));
            dataItems.Add(new ShdrDataItem("L2X2actw", new PositionValue(0.00000, inches)));
            dataItems.Add(new ShdrDataItem("L2X2load", new LoadValue(0.11)));


            // Controller
            dataItems.Add(new ShdrDataItem("L2estop", EmergencyStop.ARMED));

            // Path 1
            dataItems.Add(new ShdrDataItem("L2f1mode", FunctionalMode.PRODUCTION));
            dataItems.Add(new ShdrDataItem("L2p1mode", ControllerMode.SEMI_AUTOMATIC));
            dataItems.Add(new ShdrDataItem("L2p1program", "TEST-01.NC"));
            dataItems.Add(new ShdrDataItem("L2p1execution", Execution.READY));
            dataItems.Add(new ShdrDataItem("L2p1Fovr", new PathFeedrateOverrideValue(100)));
            dataItems.Add(new ShdrDataItem("L2p1partcount", 150));
            dataItems.Add(new ShdrDataItem("L2p1Fact", new PathFeedrateValue(50, inches)));

            _adapter.AddDataItems(dataItems);
        }

        static void AddConditions()
        {
            var condition = new ShdrCondition("L2p1system", ConditionLevel.FAULT);
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

            var timeSeries = new ShdrTimeSeries("L2p1Sensor", samples, 100);

            _adapter.AddTimeSeries(timeSeries);
        }

        static void AddDataSets()
        {
            var dataSetEntries = new List<DataSetEntry>();
            dataSetEntries.Add(new DataSetEntry("V1", 5));
            dataSetEntries.Add(new DataSetEntry("V2", 205));

            var dataSet = new ShdrDataSet("L2p1Variables", dataSetEntries);

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

            var table = new ShdrTable("L2p1ToolTable", tableEntries);

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
