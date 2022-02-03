using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTConnect.Applications.Adapters.Shdr
{
    internal class PlcSimulator
    {
        private CancellationTokenSource cancellationTokenSource;

        public bool Connected;
        public bool EmergencyStop; // true = ARMED, false = TRIGGERED
        public int Mode; // 1 = Auto, 2 = SingleBlock, 0 = Jog
        public int Status; // 0 = Stop, 1 = Idle, 2 = Active
        public SpindleData[] SpindleDatas; // [0] = Main Spindle
        public AxisData[] AxisDatas; // [0] = X, [1] = Y, [2] = Z
        public ProcessData[] ProcessDatas; // [0] = Main Process
        public ToolTableData[] ToolTable;

        public class SpindleData
        {
            public double ProgrammedSpeed { get; set; }
            public double ActualSpeed { get; set; }
        }

        public class AxisData
        {
            public double MachinePosition { get; set; }
            public double WorkPosition { get; set; }
            public double ProgrammedPosition { get; set; }
            public double DistanceToGo { get; set; }
        }

        public class ProcessData
        {
            public string Program { get; set; }
            public string SubProgram { get; set; }
            public int ToolNumber { get; set; }
            public int ToolOffset { get; set; }
        }

        public class ToolTableData
        {
            public int Tool { get; set; }
            public double Length { get; set; }
            public double Diameter { get; set; }
        }


        public PlcSimulator(string hostName)
        {
            SpindleDatas = new SpindleData[1];
            SpindleDatas[0] = new SpindleData();

            AxisDatas = new AxisData[3];
            AxisDatas[0] = new AxisData();
            AxisDatas[1] = new AxisData();
            AxisDatas[2] = new AxisData();

            ProcessDatas = new ProcessData[1];
            ProcessDatas[0] = new ProcessData();

            ToolTable = new ToolTableData[100];
            ToolTable[5] = new ToolTableData
            {
                Tool = 5,
                Length = 6.123,
                Diameter = 0.498
            };
            ToolTable[10] = new ToolTableData
            {
                Tool = 10,
                Length = 10.354,
                Diameter = 0.375
            };
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
            while (!cancellationToken.IsCancellationRequested)
            {
                Mode += 1;
                if (Mode > 2) Mode = 0;

                Status += 1;
                if (Status > 2) Status = 0;

                if (ProcessDatas[0].ToolNumber < 10)
                {
                    ProcessDatas[0].ToolNumber = 10;
                    ProcessDatas[0].ToolOffset = 10;
                }
                else if (ProcessDatas[0].ToolNumber == 10)
                {
                    ProcessDatas[0].ToolNumber = 5;
                    ProcessDatas[0].ToolOffset = 5;
                }

                for (var i = 0; i < 1000; i++)
                {
                    EmergencyStop = true;

                    ProcessDatas[0].Program = "PRT_011.NC";

                    SpindleDatas[0].ProgrammedSpeed = 5500;
                    SpindleDatas[0].ActualSpeed = 5499;

                    AxisDatas[0].MachinePosition = 123.45678;
                    AxisDatas[0].WorkPosition = 45.12345;

                    await Task.Delay(5);
                }
            }
        }
    }
}
