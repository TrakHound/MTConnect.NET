using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTConnect.Applications.Adapters.Shdr.Simulator
{
    public class DeviceSimulator
    {
        private CancellationTokenSource cancellationTokenSource;

        private int variableState = 0;


        public bool Connected { get; set; }

        public int UpdateInterval { get; set; }


        public ControllerSimulator Controller { get; set; }
        public PathSimulator Path { get; set; }

        public List<LinearAxisSimulator> LinearAxes { get; set; }
        public List<RotaryAxisSimulator> RotaryAxes { get; set; }


        public DeviceSimulator(string deviceName, int updateInterval = 100)
        {
            UpdateInterval = updateInterval;

            Controller = new ControllerSimulator();
            Path = new PathSimulator();

            LinearAxes = new List<LinearAxisSimulator>();
            LinearAxes.Add(new LinearAxisSimulator("X"));
            LinearAxes.Add(new LinearAxisSimulator("Y"));
            LinearAxes.Add(new LinearAxisSimulator("Z"));
        }


        public void Connect()
        {
            cancellationTokenSource = new CancellationTokenSource();

            Connected = true;

            _ = Task.Run(() => Worker(cancellationTokenSource.Token));
        }

        public void Disconnect()
        {
            if (cancellationTokenSource != null) cancellationTokenSource.Cancel();

            Connected = false;
        }

        private async Task Worker(CancellationToken cancellationToken)
        {
            var rnd = new Random();

            while (!cancellationToken.IsCancellationRequested)
            {
                Controller.EmergencyStop = false;
                Controller.CommunicationsAlarm = null;
                Controller.LogicAlarm = null;
                Controller.SystemAlarm = null;
                Controller.PalletId = "15";

                Path.Execution = 1;
                Path.WaitState = 0;
                Path.ControllerMode = 1;
                Path.MachineAxisLock = false;
                Path.SingleBlock = false;
                Path.DryRun = false;
                Path.MotionAlarm = null;
                Path.SystemAlarm = null;
                Path.Feedrate = rnd.Next(0, 600);
                Path.CuttingSpeed = rnd.Next(0, 600);
                Path.FeedrateOverride = rnd.Next(0, 100);
                Path.RapidOverride = rnd.Next(0, 100);
                Path.SpindleOverride = rnd.Next(0, 125);

                Path.MainProgram = "TEST-01.NC";
                Path.MainProgramComment = "Testing a New Program";
                Path.ActiveProgram = "TEST-01-SUB-04.NC";
                Path.ActiveProgramComment = "A Subprogram of TEST-01.NC";
                Path.ProgramEdit = 0;
                Path.ProgramEditName = string.Empty;
                Path.LineLabel = "N3543 G01 X432.2345 Y123.1523";
                Path.LineNumber = 3543;

                // Initialize Variables
                if (variableState < 1)
                {
                    Path.Variables = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("v1", "25"),
                        new KeyValuePair<string, string>("v2", "50"),
                        new KeyValuePair<string, string>("v3", "75"),
                        new KeyValuePair<string, string>("v4", "100")
                    };
                }

                if (variableState == 1)
                {
                    Path.Variables = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("v2", "51")
                    };
                }

                if (variableState == 2)
                {
                    Path.Variables = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("v3", "")
                    };
                }

                if (variableState == 3)
                {
                    Path.Variables = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("v2", "52")
                    };
                }

                if (variableState == 4)
                {
                    Path.Variables = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("v3", "0")
                    };
                }

                if (variableState == 5)
                {
                    Path.Variables = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("v5", "125")
                    };
                }

                variableState++;
                if (variableState > 5) variableState = 0;


                Path.PartCount = rnd.Next(0, 100);
                Path.PartNumber = StringFunctions.RandomString(10);
                Path.PartSerialNumber = StringFunctions.RandomString(15);

                Path.ChuckState = 1;

                // X Axis
                LinearAxes[0].MachinePosition = (double)rnd.Next(10000, 100000) / 10000;
                LinearAxes[0].WorkPosition = (double)rnd.Next(10000, 100000) / 10000;
                LinearAxes[0].Feedrate = rnd.Next(0, 600);
                LinearAxes[0].Load = rnd.Next(0, 100);
                LinearAxes[0].Temperature = rnd.Next(0, 100);
                LinearAxes[0].State = 3;

                // Y Axis
                LinearAxes[1].MachinePosition = (double)rnd.Next(10000, 100000) / 10000;
                LinearAxes[1].WorkPosition = (double)rnd.Next(10000, 100000) / 10000;
                LinearAxes[1].Feedrate = rnd.Next(0, 600);
                LinearAxes[1].Load = rnd.Next(0, 100);
                LinearAxes[1].Temperature = rnd.Next(0, 100);
                LinearAxes[1].State = 3;

                // Z Axis
                LinearAxes[2].MachinePosition = (double)rnd.Next(10000, 100000) / 10000;
                LinearAxes[2].WorkPosition = (double)rnd.Next(10000, 100000) / 10000;
                LinearAxes[2].Feedrate = rnd.Next(0, 600);
                LinearAxes[2].Load = rnd.Next(0, 100);
                LinearAxes[2].Temperature = rnd.Next(0, 100);
                LinearAxes[2].State = 3;

                await Task.Delay(UpdateInterval);
            }
        }
    }
}
