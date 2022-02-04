using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTConnect.Streams;
using MTConnect.Streams.Events;
using MTConnect.Streams.Samples;
using MTConnect.Observations;
using MTConnect.Adapters.Shdr;

namespace MTConnect.Applications.Adapters.Shdr.Adapters
{
    internal class AdapterExample1
    {
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

            _adapter = new ShdrAdapter(deviceName);
            _adapter.Interval = 100;
            _adapter.AgentConnected += (sender, connectionId) => Console.WriteLine($"Agent Connection (ID = {connectionId}) : Agent Connected");
            _adapter.AgentDisconnected += (sender, connectionId) => Console.WriteLine($"Agent Connection (ID = {connectionId}) : Agent Disconnected");
            _adapter.PingReceived += (sender, connectionId) => Console.WriteLine($"Agent Connection (ID = {connectionId}) : Agent Ping Received");
            _adapter.PongSent += (sender, connectionId) => Console.WriteLine($"Agent Connection (ID = {connectionId}) : Pong Sent to Agent");
            _adapter.LineSent += (sender, args) => Console.WriteLine($"Agent Connection (ID = {args.ClientId}) : Line Sent : {args.Message}");
        }

        public void Start()
        {
            _simulator.Connect();
            _adapter.Start();

            _updateTimer = new System.Timers.Timer();
            _updateTimer.Interval = 100;
            _updateTimer.Elapsed += (s, e) =>
            {
                UpdateTest();

                //UpdateController();
                //UpdatePath();

                //UpdateXAxis();
                //UpdateYAxis();
                //UpdateZAxis();
            };
            _updateTimer.Start();
        }

        public void Stop()
        {
            if (_updateTimer != null) _updateTimer.Stop();
            if (_adapter != null) _adapter.Stop();
            if (_simulator != null) _simulator.Disconnect();
        }



        void UpdateTest()
        {
            // Feedrate
            var feedrate = new ShdrDataItem("Fact", _simulator.Path.Feedrate);
            feedrate.Duration = 3600;
            _adapter.AddDataItem(feedrate);
        }



        void UpdateController()
        {
            // Emergency Stop
            if (_simulator.Controller.EmergencyStop) _adapter.AddDataItem(new ShdrDataItem("estop", "TRIGGERED"));
            else _adapter.AddDataItem(new ShdrDataItem("estop", "ARMED"));

            // Communications Alarm
            if (_simulator.Controller.CommunicationsAlarm != null)
            {
                var condition = new ShdrCondition("comms_cond", ConditionLevel.FAULT);
                condition.Message = "A Communications Error Has Occurred";
                _adapter.AddCondition(condition);
            }
            else
            {
                _adapter.AddCondition(new ShdrCondition("comms_cond", ConditionLevel.NORMAL));
            }

            // Logic Alarm
            if (_simulator.Controller.LogicAlarm != null)
            {
                var condition = new ShdrCondition("logic_cond", ConditionLevel.FAULT);
                condition.Message = "A Logic Error Has Occurred";
                _adapter.AddCondition(condition);
            }
            else
            {
                _adapter.AddCondition(new ShdrCondition("logic_cond", ConditionLevel.NORMAL));
            }

            // System Alarm
            if (_simulator.Controller.SystemAlarm != null)
            {
                var condition = new ShdrCondition("system_cond", ConditionLevel.FAULT);
                condition.Message = "A System Error Has Occurred";
                _adapter.AddCondition(condition);
            }
            else
            {
                _adapter.AddCondition(new ShdrCondition("system_cond", ConditionLevel.NORMAL));
            }

            // Pallet ID
            _adapter.AddDataItem(new ShdrDataItem("pallet_num", _simulator.Controller.PalletId));
        }

        void UpdatePath()
        {
            // Execution
            switch (_simulator.Path.Execution)
            {
                case 0: _adapter.AddDataItem(new ShdrDataItem("execution", "UNAVAILABLE")); break;
                case 1: _adapter.AddDataItem(new ShdrDataItem("execution", "READY")); break;
                case 2: _adapter.AddDataItem(new ShdrDataItem("execution", "ACTIVE")); break;
            }

            // Wait State
            switch (_simulator.Path.WaitState)
            {
                case 0: _adapter.AddDataItem(new ShdrDataItem("waitstate", "")); break;
                case 1: _adapter.AddDataItem(new ShdrDataItem("waitstate", "POWERING_UP")); break;
                case 2: _adapter.AddDataItem(new ShdrDataItem("waitstate", "PART_LOAD")); break;
                case 3: _adapter.AddDataItem(new ShdrDataItem("waitstate", "TOOL_LOAD")); break;
            }

            // Controller Mode
            switch (_simulator.Path.ControllerMode)
            {
                case 0: _adapter.AddDataItem(new ShdrDataItem("mode", "MANUAL")); break;
                case 1: _adapter.AddDataItem(new ShdrDataItem("mode", "MANUAL_DATA_INPUT")); break;
                case 2: _adapter.AddDataItem(new ShdrDataItem("mode", "SEMI_AUTOMATIC")); break;
                case 3: _adapter.AddDataItem(new ShdrDataItem("mode", "AUTOMATIC")); break;
                case 4: _adapter.AddDataItem(new ShdrDataItem("mode", "EDIT")); break;
            }

            // Machine Axis Lock
            if (_simulator.Path.MachineAxisLock) _adapter.AddDataItem(new ShdrDataItem("cmomachineaxislock", "ON"));
            else _adapter.AddDataItem(new ShdrDataItem("cmomachineaxislock", "OFF"));

            // Single Block
            if (_simulator.Path.SingleBlock) _adapter.AddDataItem(new ShdrDataItem("cmosingleblock", "ON"));
            else _adapter.AddDataItem(new ShdrDataItem("cmosingleblock", "OFF"));

            // Dry Run
            if (_simulator.Path.DryRun) _adapter.AddDataItem(new ShdrDataItem("cmodryrun", "ON"));
            else _adapter.AddDataItem(new ShdrDataItem("cmodryrun", "OFF"));

            // Motion Alarm
            if (_simulator.Controller.LogicAlarm != null)
            {
                var condition = new ShdrCondition("motion_cond", ConditionLevel.FAULT);
                condition.Message = "A Path Motion Error Has Occurred";
                _adapter.AddCondition(condition);
            }
            else
            {
                _adapter.AddCondition(new ShdrCondition("motion_cond", ConditionLevel.NORMAL));
            }

            // System Alarm
            if (_simulator.Controller.SystemAlarm != null)
            {
                var condition = new ShdrCondition("path_system", ConditionLevel.FAULT);
                condition.Message = "A Path System Error Has Occurred";
                _adapter.AddCondition(condition);
            }
            else
            {
                _adapter.AddCondition(new ShdrCondition("path_system", ConditionLevel.NORMAL));
            }

            // Feedrate
            var feedrate = new ShdrDataItem("Fact", _simulator.Path.Feedrate);
            feedrate.Duration = 3600;
            _adapter.AddDataItem(feedrate);

            // Cutting Speed
            _adapter.AddDataItem(new ShdrDataItem("cspeed", _simulator.Path.CuttingSpeed));

            // Feedrate Override
            _adapter.AddDataItem(new ShdrDataItem("Fovr", _simulator.Path.FeedrateOverride));

            // Rapid Override
            _adapter.AddDataItem(new ShdrDataItem("Frapidovr", _simulator.Path.RapidOverride));

            // Speed Override
            _adapter.AddDataItem(new ShdrDataItem("Sovr", _simulator.Path.SpindleOverride));

            // Main Program
            _adapter.AddDataItem(new ShdrDataItem("program", _simulator.Path.MainProgram));
            _adapter.AddDataItem(new ShdrDataItem("program_cmt", _simulator.Path.MainProgramComment));

            // Active Program
            _adapter.AddDataItem(new ShdrDataItem("activeprog", _simulator.Path.ActiveProgram));
            _adapter.AddDataItem(new ShdrDataItem("activeprogram_cmt", _simulator.Path.ActiveProgramComment));

            // Program Edit
            switch (_simulator.Path.ProgramEdit)
            {
                case -1: _adapter.AddDataItem(new ShdrDataItem("peditmode", "NOT_READY")); break;
                case 0: _adapter.AddDataItem(new ShdrDataItem("peditmode", "READY")); break;
                case 1: _adapter.AddDataItem(new ShdrDataItem("peditmode", "ACTIVE")); break;
            }

            _adapter.AddDataItem(new ShdrDataItem("peditname", _simulator.Path.ProgramEditName));

            _adapter.AddDataItem(new ShdrDataItem("linelabel", _simulator.Path.LineLabel));
            _adapter.AddDataItem(new ShdrDataItem("linenumber", _simulator.Path.LineNumber));



            // Tool
            _adapter.AddDataItem(new ShdrDataItem("Tool_number", _simulator.Path.ToolNumber));
            _adapter.AddDataItem(new ShdrDataItem("Tool_group", _simulator.Path.ToolGroup));

            // Origin
            _adapter.AddDataItem(new ShdrDataItem("woffset", _simulator.Path.WorkOffset));
            _adapter.AddDataItem(new ShdrDataItem("pathpos", _simulator.Path.Position));
            _adapter.AddDataItem(new ShdrDataItem("orientation", _simulator.Path.Orientation));
            _adapter.AddDataItem(new ShdrDataItem("workoffsettrans", _simulator.Path.WorkOffsetTranslation));
            _adapter.AddDataItem(new ShdrDataItem("workoffsetrot", _simulator.Path.WorkOffsetRotation));
            _adapter.AddDataItem(new ShdrDataItem("activeaxes", _simulator.Path.ActiveAxes));

            // Variables
            var variables = new List<DataSetEntry>();
            foreach (var variable in _simulator.Path.Variables)
            {
                variables.Add(new DataSetEntry(variable.Key.ToString(), variable.Value));
            }
            _adapter.AddDataSet(new ShdrDataSet("cvars", variables));

            // Part
            _adapter.AddDataItem(new ShdrDataItem("PartCountAct", _simulator.Path.PartCount));
            _adapter.AddDataItem(new ShdrDataItem("partnumber", _simulator.Path.PartNumber));
            _adapter.AddDataItem(new ShdrDataItem("partserialnumber", _simulator.Path.PartSerialNumber));

            // Chuck State
            switch (_simulator.Path.ChuckState)
            {
                case -1: _adapter.AddDataItem(new ShdrDataItem("hd1chuckstate", "OPEN")); break;
                case 0: _adapter.AddDataItem(new ShdrDataItem("hd1chuckstate", "UNLATCHED")); break;
                case 1: _adapter.AddDataItem(new ShdrDataItem("hd1chuckstate", "CLOSED")); break;
            }
        }

        void UpdateXAxis()
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

        void UpdateYAxis()
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

        void UpdateZAxis()
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
    }
}
