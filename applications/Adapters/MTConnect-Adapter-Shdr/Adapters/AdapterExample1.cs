using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTConnect.Devices;
using MTConnect.Devices.Components;
using MTConnect.Devices.DataItems;
//using MTConnect.Observations;
using MTConnect.Observations.Events;
using MTConnect.Observations.Samples.Values;
using MTConnect.Observations;
using MTConnect.Adapters.Shdr;
using MTConnect.Shdr;

namespace MTConnect.Applications.Adapters.Shdr.Adapters
{
    internal class AdapterExample1
    {
        CancellationTokenSource _stop;

        System.Timers.Timer _updateTimer;
        string _deviceName;

        ShdrAdapter _adapter;
        Simulator.DeviceSimulator _simulator;

        double xabs = 0;
        double xpos = 123.4567;


        public AdapterExample1(string deviceName, int port = 7878)
        {
            _deviceName = deviceName;
            _simulator = new Simulator.DeviceSimulator(deviceName, 100);

            //_adapter = new ShdrAdapter();
            _adapter = new ShdrAdapter(deviceName);
            _adapter.Interval = 0;
            _adapter.MultilineAssets = true;
            _adapter.MultilineDevices = true;
            _adapter.AgentConnected += (sender, connectionId) => Console.WriteLine($"Agent Connection (ID = {connectionId}) : Agent Connected");
            _adapter.AgentDisconnected += (sender, connectionId) => Console.WriteLine($"Agent Connection (ID = {connectionId}) : Agent Disconnected");
            _adapter.PingReceived += (sender, connectionId) => Console.WriteLine($"Agent Connection (ID = {connectionId}) : Agent Ping Received");
            _adapter.PongSent += (sender, connectionId) => Console.WriteLine($"Agent Connection (ID = {connectionId}) : Pong Sent to Agent");
            //_adapter.LineSent += (sender, args) => Console.WriteLine($"Agent Connection (ID = {args.ClientId}) : Line Sent : {args.Message}");
        }

        //private void Demo()
        //{
        //    var adapter = new ShdrAdapter("device1");
        //    adapter.Start();

        //    var temp = new ShdrDataItem("servotemp1", 120.5);
        //    _adapter.AddDataItem(temp);




        //}

        public void Start()
        {
            _stop = new CancellationTokenSource();

            _simulator.Connect();
            _adapter.Start();

            //_ = Task.Run(async () =>
            // {
            //     while (!_stop.IsCancellationRequested)
            //     {
            //         UpdateTest();

            //         await Task.Delay(100);
            //     }
            // });

            //_updateTimer = new System.Timers.Timer();
            //_updateTimer.Interval = 100;
            //_updateTimer.Elapsed += (s, e) =>
            //{
            //    UpdateTest();

            //    //UpdateController();
            //    //UpdatePath();

            //    //UpdateXAxis();
            //    //UpdateYAxis();
            //    //UpdateZAxis();
            //};
            //_updateTimer.Start();
        }

        public void Stop()
        {
            if (_stop != null) _stop.Cancel();

            if (_updateTimer != null) _updateTimer.Stop();
            if (_adapter != null) _adapter.Stop();
            if (_simulator != null) _simulator.Disconnect();
        }

        public void SetUnavailable()
        {
            _adapter.SetUnavailable();
            //_adapter.SetUnavailable(UnixDateTime.Now);
        }

        private int j = 1;
        private int k = 1;
        private int l = 1;


        public void AddDevice()
        {
            var devices = Configurations.DeviceConfiguration.FromFile(@"D:\TrakHound\Source-Code\MTConnect.NET\applications\Adapters\MTConnect-Adapter-Shdr\bin\Debug\net6.0\device-mazak.xml", "XML");
            if (devices != null)
            {
                var device = devices.FirstOrDefault();
                if (device != null)
                {
                    var description = new Description();
                    description.Manufacturer = "Mazak";
                    description.Model = "Integrex";
                    description.SerialNumber = "00123456789";
                    description.Value = "Patrick Test";
                    device.Description = description;

                    _adapter.AddDevice(device);
                }
            }



            //var device = new Device();
            //device.Id = "testing-123";
            //device.Name = "testing";
            //device.Uuid = "testing123";

            //var controller = new ControllerComponent();
            //controller.Id = "cont";

            //var execution = new Devices.DataItems.Events.ExecutionDataItem();
            //execution.Id = "exec";
            //controller.AddDataItem(execution);

            //device.AddComponent(controller);

            //_adapter.AddDevice(device);
        }


        public void RemoveAsset()
        {
            _adapter.RemoveAsset("file.patrick1");
        }

        public void RemoveAllAssets()
        {
            _adapter.RemoveAllAssets("File");
            //_adapter.RemoveAllAssets("CuttingTool");
        }


        public void UpdateValue()
        {
            var now = UnixDateTime.Now;
            var axisXDataItem = new ShdrDataItem("Xload", j++, now);
            var axisYDataItem = new ShdrDataItem("Yload", j++, now);

            //var axisXDataItem = new ShdrDataItem("X1load", j++, now);
            //var axisYDataItem = new ShdrDataItem("X2load", j++, now);
            //axisXDataItem.ResetTriggered = ResetTriggered.DAY;
            //var axisYDataItem = new ShdrDataItem("Ypos", k++);
            //var axisZDataItem = new ShdrDataItem("Zpos", l++);
            _adapter.AddDataItem(axisXDataItem);
            _adapter.AddDataItem(axisYDataItem);
            //_adapter.AddDataItem(axisZDataItem);

            //var axisXDataItem = new ShdrDataItem("Xpos", j++);
            //var axisYDataItem = new ShdrDataItem("Ypos", k++);
            //var axisZDataItem = new ShdrDataItem("Zpos", l++);
            //_adapter.AddDataItem(axisXDataItem);
            //_adapter.AddDataItem(axisYDataItem);
            //_adapter.AddDataItem(axisZDataItem);
        }

        public void UpdateUnavaiableTest()
        {
            var axesDataItem = new ShdrDataItem("f_sim_p1_axes", "X Y Z S");
            _adapter.AddDataItem(axesDataItem);


            var message1 = new ShdrMessage("message", "Change Inserts", "CHG_INSRT");
            _adapter.AddMessage(message1);


            var system = new ShdrCondition("p1system");
            system.AddWarning("Bad Request", "400");
            system.AddFault("Not Found", "404");
            system.AddWarning("Method Not Found", "405");
            _adapter.AddCondition(system);


            var timeValues = new List<double> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var time = new ShdrTimeSeries("p1Sensor", timeValues, 100);
            _adapter.AddTimeSeries(time);


            var dataSet = new ShdrDataSet("p1Variables");
            dataSet.Entries = new List<DataSetEntry>
            {
                new DataSetEntry("T1", 12.3456),
                new DataSetEntry("T2", 23.4567),
                new DataSetEntry("T3", 34.5678),
                new DataSetEntry("T4", 45.6789)
            };

            _adapter.AddDataSet(dataSet);


            var table = new ShdrTable("p1ToolTable");

            var entries = new List<TableEntry>();

            var cells1 = new List<TableCell>
            {
                new TableCell("LENGTH", 4.1234),
                new TableCell("DIAMETER", 0.498),
            };
            entries.Add(new TableEntry("T1", cells1));

            var cells2 = new List<TableCell>
            {
                new TableCell("LENGTH", 8.7421),
                new TableCell("DIAMETER", 0.374),
            };
            entries.Add(new TableEntry("T2", cells2));

            table.Entries = entries;

            _adapter.AddTable(table);
        }

        public void Update()
        {
            var message1 = new ShdrMessage("message", "Change Inserts", "CHG_INSRT");
            _adapter.AddMessage(message1);

            //var message2 = new ShdrMessage("message2");
            //message2.CDATA = "Change Inserts";
            //message2.NativeCode = "CHG_INSRT";
            //_adapter.AddMessage(message2);
        }

        public void UpdateTemperature1()
        {
            var temp = new ShdrDataItem("servotemp1", new TemperatureValue(120.5));
            _adapter.AddDataItem(temp);

            var abs = new ShdrDataItem("Xabs", new PositionValue(325.7894));
            _adapter.AddDataItem(abs);

            var pos = new ShdrDataItem("Xpos", new PositionValue(325.7894));
            _adapter.AddDataItem(pos);

            var pathPos = new ShdrDataItem("pathpos", new PathPositionValue(0, 0, 0));
            _adapter.AddDataItem(pathPos);
        }

        public void UpdateTemperature2()
        {
            var temp = new ShdrDataItem("servotemp1", 78.456);
            _adapter.AddDataItem(temp);

            var abs = new ShdrDataItem("Xabs", 45.987);
            _adapter.AddDataItem(abs);

            var pos = new ShdrDataItem("Xpos", 45.987);
            _adapter.AddDataItem(pos);

            var pathPos = new ShdrDataItem("pathpos", new PathPositionValue(12.123, 45.456, 7.894));
            _adapter.AddDataItem(pathPos);
        }


        public void UpdateTest1()
        {
            var system = new ShdrCondition("p1system");
            system.AddWarning("Bad Request", "400");
            system.AddFault("Not Found", "404");
            system.AddWarning("Method Not Found", "405");
            _adapter.AddCondition(system);
        }

        public void UpdateTest2()
        {
            var accessDenied = new ShdrCondition("p1system", ConditionLevel.WARNING);
            accessDenied.AddWarning("Access Denied", "401");
            _adapter.AddCondition(accessDenied);
        }

        public void UpdateTest3()
        {
            var condition = new ShdrCondition("p1system", ConditionLevel.NORMAL);
            _adapter.AddCondition(condition);
        }

        public void UpdateDataSet1()
        {
            var dataSet = new ShdrDataSet("p1Variables");
            dataSet.Entries = new List<DataSetEntry>
            {
                new DataSetEntry("T1", 12.3456),
                new DataSetEntry("T2", 23.4567),
                new DataSetEntry("T3", 34.5678),
                new DataSetEntry("T4", 45.6789)
            };

            _adapter.AddDataSet(dataSet);
        }

        public void UpdateDataSet2()
        {
            var dataSet = new ShdrDataSet("p1Variables");
            dataSet.Entries = new List<DataSetEntry>
            {
                new DataSetEntry("T5", 12.3456),
                new DataSetEntry("T6", 23.4567)
            };

            _adapter.AddDataSet(dataSet);
        }

        public void UpdateDataSet3()
        {
            var dataSet = new ShdrDataSet("p1Variables");
            dataSet.Entries = new List<DataSetEntry>
            {
                new DataSetEntry("T3", true),
                new DataSetEntry("T5", "TESTING")
            };

            _adapter.AddDataSet(dataSet);
        }

        public void UpdateDataSet4()
        {
            var dataSet = new ShdrDataSet("p1Variables");
            dataSet.ResetTriggered = ResetTriggered.MANUAL;
            dataSet.Entries = new List<DataSetEntry>
            {
                new DataSetEntry("T1", 9.1234),
                new DataSetEntry("T2", 7.0000)
            };
            _adapter.AddDataSet(dataSet);
        }


        public void UpdateTestTable1()
        {
            var table = new ShdrTable("p1ToolTable");

            var entries = new List<TableEntry>();

            var cells1 = new List<TableCell> 
            { 
                new TableCell("LENGTH", 4.1234),
                new TableCell("DIAMETER", 0.498),
            };
            entries.Add(new TableEntry("T1", cells1));

            var cells2 = new List<TableCell>
            {
                new TableCell("LENGTH", 8.7421),
                new TableCell("DIAMETER", 0.374),
            };
            entries.Add(new TableEntry("T2", cells2));

            table.Entries = entries;

            _adapter.AddTable(table);
        }

        public void UpdateTestTable2()
        {
            var table = new ShdrTable("p1ToolTable");

            var entries = new List<TableEntry>();

            var cells3 = new List<TableCell>
            {
                new TableCell("LENGTH", 10.345),
                new TableCell("DIAMETER", 0.125),
            };
            entries.Add(new TableEntry("T3", cells3));

            table.Entries = entries;

            _adapter.AddTable(table);
        }

        public void UpdateTestTable3()
        {
            var table = new ShdrTable("p1ToolTable");

            var entries = new List<ShdrTableEntry>();

            entries.Add(new ShdrTableEntry("T2",  true));

            table.Entries = entries;

            _adapter.AddTable(table);
        }



        public void TestConditionUnavailable()
        {
            var condition = new ShdrCondition("system_cond");
            condition.Unavailable();
            _adapter.AddCondition(condition);
        }

        public void TestConditionNormal()
        {
            var condition = new ShdrCondition("system_cond");
            condition.Normal();
            _adapter.AddCondition(condition);
        }

        public void TestConditionWarning()
        {
            var condition = new ShdrCondition("system_cond");
            condition.AddWarning("Access Denied", "401");
            _adapter.AddCondition(condition);
        }

        public void TestConditionFault()
        {
            var condition = new ShdrCondition("system_cond");
            condition.AddWarning("Bad Request", "400");
            condition.AddFault("Not Found", "404");
            condition.AddFault("Method Not Found", "405");
            _adapter.AddCondition(condition);
        }

        //public void UpdateTest1()
        //{
        //    var condition = new ShdrCondition("system_cond");
        //    condition.AddWarning("Bad Request", "400");
        //    condition.AddFault("Not Found", "404");
        //    condition.AddFault("Method Not Found", "405");
        //    _adapter.AddCondition(condition);
        //}





        //void UpdateController()
        //{
        //    // Emergency Stop
        //    if (_simulator.Controller.EmergencyStop) _adapter.AddDataItem(new ShdrDataItem("estop", "TRIGGERED"));
        //    else _adapter.AddDataItem(new ShdrDataItem("estop", "ARMED"));

        //    // Communications Alarm
        //    if (_simulator.Controller.CommunicationsAlarm != null)
        //    {
        //        var condition = new ShdrCondition("comms_cond");
        //        condition.Fault("A Communications Error Has Occurred");
        //        _adapter.AddCondition(condition);
        //    }
        //    else
        //    {
        //        var condition = new ShdrCondition("comms_cond");
        //        condition.Normal();
        //        _adapter.AddCondition(condition);
        //    }

        //    // Logic Alarm
        //    if (_simulator.Controller.LogicAlarm != null)
        //    {
        //        var condition = new ShdrCondition("logic_cond");
        //        //condition.AddFault("Not Found", "404");
        //        //condition.AddFault("Bad Request", "405");
        //        _adapter.AddCondition(condition);

        //        //var condition = new ShdrCondition("logic_cond", ConditionLevel.FAULT);
        //        //condition.Message = "A Logic Error Has Occurred";
        //        //_adapter.AddCondition(condition);
        //    }
        //    else
        //    {
        //        var condition = new ShdrCondition("logic_cond");
        //        condition.Normal();
        //        _adapter.AddCondition(condition);
        //        //_adapter.AddCondition(new ShdrCondition("logic_cond", ConditionLevel.NORMAL));
        //    }

        //    // System Alarm
        //    if (_simulator.Controller.SystemAlarm != null)
        //    {
        //        var condition = new ShdrCondition("system_cond");
        //        condition.AddFault("Not Found", "404");
        //        condition.AddFault("Bad Request", "405");
        //        _adapter.AddCondition(condition);

        //        //var condition = new ShdrCondition("system_cond", ConditionLevel.FAULT);
        //        //condition.Message = "A System Error Has Occurred";
        //        //_adapter.AddCondition(condition);
        //    }
        //    else
        //    {
        //        var condition = new ShdrCondition("system_cond");
        //        condition.Normal();
        //        _adapter.AddCondition(condition);
        //        //_adapter.AddCondition(new ShdrCondition("system_cond", ConditionLevel.NORMAL));
        //    }

        //    // Pallet ID
        //    _adapter.AddDataItem(new ShdrDataItem("pallet_num", _simulator.Controller.PalletId));
        //}

        //void UpdatePath()
        //{
        //    // Execution
        //    switch (_simulator.Path.Execution)
        //    {
        //        case 0: _adapter.AddDataItem(new ShdrDataItem("execution", "UNAVAILABLE")); break;
        //        case 1: _adapter.AddDataItem(new ShdrDataItem("execution", "READY")); break;
        //        case 2: _adapter.AddDataItem(new ShdrDataItem("execution", "ACTIVE")); break;
        //    }

        //    // Wait State
        //    switch (_simulator.Path.WaitState)
        //    {
        //        case 0: _adapter.AddDataItem(new ShdrDataItem("waitstate", "")); break;
        //        case 1: _adapter.AddDataItem(new ShdrDataItem("waitstate", "POWERING_UP")); break;
        //        case 2: _adapter.AddDataItem(new ShdrDataItem("waitstate", "PART_LOAD")); break;
        //        case 3: _adapter.AddDataItem(new ShdrDataItem("waitstate", "TOOL_LOAD")); break;
        //    }

        //    // Controller Mode
        //    switch (_simulator.Path.ControllerMode)
        //    {
        //        case 0: _adapter.AddDataItem(new ShdrDataItem("mode", "MANUAL")); break;
        //        case 1: _adapter.AddDataItem(new ShdrDataItem("mode", "MANUAL_DATA_INPUT")); break;
        //        case 2: _adapter.AddDataItem(new ShdrDataItem("mode", "SEMI_AUTOMATIC")); break;
        //        case 3: _adapter.AddDataItem(new ShdrDataItem("mode", "AUTOMATIC")); break;
        //        case 4: _adapter.AddDataItem(new ShdrDataItem("mode", "EDIT")); break;
        //    }

        //    // Machine Axis Lock
        //    if (_simulator.Path.MachineAxisLock) _adapter.AddDataItem(new ShdrDataItem("cmomachineaxislock", "ON"));
        //    else _adapter.AddDataItem(new ShdrDataItem("cmomachineaxislock", "OFF"));

        //    // Single Block
        //    if (_simulator.Path.SingleBlock) _adapter.AddDataItem(new ShdrDataItem("cmosingleblock", "ON"));
        //    else _adapter.AddDataItem(new ShdrDataItem("cmosingleblock", "OFF"));

        //    // Dry Run
        //    if (_simulator.Path.DryRun) _adapter.AddDataItem(new ShdrDataItem("cmodryrun", "ON"));
        //    else _adapter.AddDataItem(new ShdrDataItem("cmodryrun", "OFF"));

        //    // Motion Alarm
        //    if (_simulator.Controller.LogicAlarm != null)
        //    {
        //        var condition = new ShdrCondition("motion_cond");
        //        condition.Fault("A Path Motion Error Has Occurred");
        //        _adapter.AddCondition(condition);

        //        //var condition = new ShdrCondition("motion_cond", ConditionLevel.FAULT);
        //        //condition.Message = "A Path Motion Error Has Occurred";
        //        //_adapter.AddCondition(condition);
        //    }
        //    else
        //    {
        //        var condition = new ShdrCondition("motion_cond");
        //        condition.Normal();
        //        _adapter.AddCondition(condition);
        //        //_adapter.AddCondition(new ShdrCondition("motion_cond", ConditionLevel.NORMAL));
        //    }

        //    // System Alarm
        //    if (_simulator.Controller.SystemAlarm != null)
        //    {
        //        var condition = new ShdrCondition("path_system");
        //        condition.Fault("A Path System Error Has Occurred");
        //        _adapter.AddCondition(condition);

        //        //var condition = new ShdrCondition("path_system", ConditionLevel.FAULT);
        //        //condition.Message = "A Path System Error Has Occurred";
        //        //_adapter.AddCondition(condition);
        //    }
        //    else
        //    {
        //        var condition = new ShdrCondition("path_system");
        //        condition.Normal();
        //        _adapter.AddCondition(condition);
        //        //_adapter.AddCondition(new ShdrCondition("path_system", ConditionLevel.NORMAL));
        //    }

        //    // Feedrate
        //    var feedrate = new ShdrDataItem("Fact", _simulator.Path.Feedrate);
        //    feedrate.Duration = 3600;
        //    _adapter.AddDataItem(feedrate);

        //    // Cutting Speed
        //    _adapter.AddDataItem(new ShdrDataItem("cspeed", _simulator.Path.CuttingSpeed));

        //    // Feedrate Override
        //    _adapter.AddDataItem(new ShdrDataItem("Fovr", _simulator.Path.FeedrateOverride));

        //    // Rapid Override
        //    _adapter.AddDataItem(new ShdrDataItem("Frapidovr", _simulator.Path.RapidOverride));

        //    // Speed Override
        //    _adapter.AddDataItem(new ShdrDataItem("Sovr", _simulator.Path.SpindleOverride));

        //    // Main Program
        //    _adapter.AddDataItem(new ShdrDataItem("program", _simulator.Path.MainProgram));
        //    _adapter.AddDataItem(new ShdrDataItem("program_cmt", _simulator.Path.MainProgramComment));

        //    // Active Program
        //    _adapter.AddDataItem(new ShdrDataItem("activeprog", _simulator.Path.ActiveProgram));
        //    _adapter.AddDataItem(new ShdrDataItem("activeprogram_cmt", _simulator.Path.ActiveProgramComment));

        //    // Program Edit
        //    switch (_simulator.Path.ProgramEdit)
        //    {
        //        case -1: _adapter.AddDataItem(new ShdrDataItem("peditmode", "NOT_READY")); break;
        //        case 0: _adapter.AddDataItem(new ShdrDataItem("peditmode", "READY")); break;
        //        case 1: _adapter.AddDataItem(new ShdrDataItem("peditmode", "ACTIVE")); break;
        //    }

        //    _adapter.AddDataItem(new ShdrDataItem("peditname", _simulator.Path.ProgramEditName));

        //    _adapter.AddDataItem(new ShdrDataItem("linelabel", _simulator.Path.LineLabel));
        //    _adapter.AddDataItem(new ShdrDataItem("linenumber", _simulator.Path.LineNumber));



        //    // Tool
        //    _adapter.AddDataItem(new ShdrDataItem("Tool_number", _simulator.Path.ToolNumber));
        //    _adapter.AddDataItem(new ShdrDataItem("Tool_group", _simulator.Path.ToolGroup));

        //    // Origin
        //    _adapter.AddDataItem(new ShdrDataItem("woffset", _simulator.Path.WorkOffset));
        //    _adapter.AddDataItem(new ShdrDataItem("pathpos", _simulator.Path.Position));
        //    _adapter.AddDataItem(new ShdrDataItem("orientation", _simulator.Path.Orientation));
        //    _adapter.AddDataItem(new ShdrDataItem("workoffsettrans", _simulator.Path.WorkOffsetTranslation));
        //    _adapter.AddDataItem(new ShdrDataItem("workoffsetrot", _simulator.Path.WorkOffsetRotation));
        //    _adapter.AddDataItem(new ShdrDataItem("activeaxes", _simulator.Path.ActiveAxes));

        //    // Variables
        //    var variables = new List<DataSetEntry>();
        //    foreach (var variable in _simulator.Path.Variables)
        //    {
        //        variables.Add(new DataSetEntry(variable.Key.ToString(), variable.Value));
        //    }
        //    _adapter.AddDataSet(new ShdrDataSet("cvars", variables));

        //    // Part
        //    _adapter.AddDataItem(new ShdrDataItem("PartCountAct", _simulator.Path.PartCount));
        //    _adapter.AddDataItem(new ShdrDataItem("partnumber", _simulator.Path.PartNumber));
        //    _adapter.AddDataItem(new ShdrDataItem("partserialnumber", _simulator.Path.PartSerialNumber));

        //    // Chuck State
        //    switch (_simulator.Path.ChuckState)
        //    {
        //        case -1: _adapter.AddDataItem(new ShdrDataItem("hd1chuckstate", "OPEN")); break;
        //        case 0: _adapter.AddDataItem(new ShdrDataItem("hd1chuckstate", "UNLATCHED")); break;
        //        case 1: _adapter.AddDataItem(new ShdrDataItem("hd1chuckstate", "CLOSED")); break;
        //    }
        //}

        //void UpdateXAxis()
        //{
        //    var axis = _simulator.LinearAxes[0];
        //    _adapter.AddDataItem(new ShdrDataItem("Xabs", axis.MachinePosition));
        //    _adapter.AddDataItem(new ShdrDataItem("Xpos", axis.WorkPosition));
        //    _adapter.AddDataItem(new ShdrDataItem("Xfrt", axis.Feedrate));
        //    _adapter.AddDataItem(new ShdrDataItem("Xload", axis.Load));
        //    _adapter.AddDataItem(new ShdrDataItem("servotemp1", axis.Temperature));

        //    switch (axis.State)
        //    {
        //        case 0: _adapter.AddDataItem(new ShdrDataItem("xaxisstate", AxisState.STOPPED)); break;
        //        case 1: _adapter.AddDataItem(new ShdrDataItem("xaxisstate", AxisState.HOME)); break;
        //        case 2: _adapter.AddDataItem(new ShdrDataItem("xaxisstate", AxisState.PARKED)); break;
        //        case 3: _adapter.AddDataItem(new ShdrDataItem("xaxisstate", AxisState.TRAVEL)); break;
        //    }

        //    if (axis.Overtravel)
        //    {
        //        var condition = new ShdrCondition("Xtravel");
        //        condition.Fault("X Axis Overtravel Limit Reached");
        //        _adapter.AddCondition(condition);

        //        //var condition = new ShdrCondition("Xtravel", ConditionLevel.FAULT);
        //        //condition.Message = "X Axis Overtravel Limit Reached";
        //        //_adapter.AddCondition(condition);
        //    }
        //    else
        //    {
        //        var condition = new ShdrCondition("Xtravel");
        //        condition.Normal();
        //        _adapter.AddCondition(condition);
        //        //_adapter.AddCondition(new ShdrCondition("Xtravel", ConditionLevel.NORMAL));
        //    }
        //}

        //void UpdateYAxis()
        //{
        //    var axis = _simulator.LinearAxes[1];
        //    _adapter.AddDataItem(new ShdrDataItem("Yabs", axis.MachinePosition));
        //    _adapter.AddDataItem(new ShdrDataItem("Ypos", axis.WorkPosition));
        //    _adapter.AddDataItem(new ShdrDataItem("Yfrt", axis.Feedrate));
        //    _adapter.AddDataItem(new ShdrDataItem("Yload", axis.Load));
        //    _adapter.AddDataItem(new ShdrDataItem("servotemp2", axis.Temperature));

        //    switch (axis.State)
        //    {
        //        case 0: _adapter.AddDataItem(new ShdrDataItem("yaxisstate", AxisState.STOPPED)); break;
        //        case 1: _adapter.AddDataItem(new ShdrDataItem("yaxisstate", AxisState.HOME)); break;
        //        case 2: _adapter.AddDataItem(new ShdrDataItem("yaxisstate", AxisState.PARKED)); break;
        //        case 3: _adapter.AddDataItem(new ShdrDataItem("yaxisstate", AxisState.TRAVEL)); break;
        //    }

        //    if (axis.Overtravel)
        //    {
        //        var condition = new ShdrCondition("Ytravel");
        //        condition.Fault("Y Axis Overtravel Limit Reached");
        //        _adapter.AddCondition(condition);

        //        //var condition = new ShdrCondition("Ytravel", ConditionLevel.FAULT);
        //        //condition.Message = "Y Axis Overtravel Limit Reached";
        //        //_adapter.AddCondition(condition);
        //    }
        //    else
        //    {
        //        var condition = new ShdrCondition("Ytravel");
        //        condition.Normal();
        //        _adapter.AddCondition(condition);
        //        //_adapter.AddCondition(new ShdrCondition("Ytravel", ConditionLevel.NORMAL));
        //    }
        //}

        //void UpdateZAxis()
        //{
        //    var axis = _simulator.LinearAxes[2];
        //    _adapter.AddDataItem(new ShdrDataItem("Zabs", axis.MachinePosition));
        //    _adapter.AddDataItem(new ShdrDataItem("Zpos", axis.WorkPosition));
        //    _adapter.AddDataItem(new ShdrDataItem("Zfrt", axis.Feedrate));
        //    _adapter.AddDataItem(new ShdrDataItem("Zload", axis.Load));
        //    _adapter.AddDataItem(new ShdrDataItem("servotemp3", axis.Temperature));

        //    switch (axis.State)
        //    {
        //        case 0: _adapter.AddDataItem(new ShdrDataItem("zaxisstate", AxisState.STOPPED)); break;
        //        case 1: _adapter.AddDataItem(new ShdrDataItem("zaxisstate", AxisState.HOME)); break;
        //        case 2: _adapter.AddDataItem(new ShdrDataItem("zaxisstate", AxisState.PARKED)); break;
        //        case 3: _adapter.AddDataItem(new ShdrDataItem("zaxisstate", AxisState.TRAVEL)); break;
        //    }

        //    if (axis.Overtravel)
        //    {
        //        var condition = new ShdrCondition("Ztravel");
        //        condition.Fault("Z Axis Overtravel Limit Reached");
        //        _adapter.AddCondition(condition);

        //        //var condition = new ShdrCondition("Ztravel", ConditionLevel.FAULT);
        //        //condition.Message = "Z Axis Overtravel Limit Reached";
        //        //_adapter.AddCondition(condition);
        //    }
        //    else
        //    {
        //        var condition = new ShdrCondition("Ztravel");
        //        condition.Normal();
        //        _adapter.AddCondition(condition);
        //        //_adapter.AddCondition(new ShdrCondition("Ztravel", ConditionLevel.NORMAL));
        //    }
        //}

        public void AddCuttingTools()
        {
            var tool = new Assets.CuttingTools.CuttingToolAsset();
            //tool.Description = new Devices.Description
            //{
            //    Manufacturer = "Sandvik",
            //    Model = "B5632",
            //    SerialNumber = "12345678946"
            //};
            tool.AssetId = "5.12";
            tool.ToolId = "12";
            tool.CuttingToolLifeCycle = new Assets.CuttingTools.CuttingToolLifeCycle
            {
                Location = new Assets.CuttingTools.Location { Type = Assets.CuttingTools.LocationType.SPINDLE },
                ProgramToolNumber = "12",
                ProgramToolGroup = "5"
            };
            tool.CuttingToolLifeCycle.Measurements.Add(new Assets.CuttingTools.Measurements.FunctionalLengthMeasurement(7.6543));
            tool.CuttingToolLifeCycle.Measurements.Add(new Assets.CuttingTools.Measurements.CuttingDiameterMaxMeasurement(0.375));
            tool.CuttingToolLifeCycle.CuttingItems.Add(new Assets.CuttingTools.CuttingItem
            {
                ItemId = "12.1",
                Locus = Assets.CuttingTools.CuttingItemLocas.FLUTE.ToString()
            });
            tool.CuttingToolLifeCycle.CutterStatus.Add(Assets.CuttingTools.CutterStatus.AVAILABLE);
            tool.CuttingToolLifeCycle.CutterStatus.Add(Assets.CuttingTools.CutterStatus.NEW);
            tool.CuttingToolLifeCycle.CutterStatus.Add(Assets.CuttingTools.CutterStatus.MEASURED);
            tool.DateTime = DateTime.Now;

            _adapter.AddAsset(tool);
        }
    }
}
