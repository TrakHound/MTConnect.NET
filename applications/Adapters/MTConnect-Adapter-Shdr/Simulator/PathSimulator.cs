using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTConnect.Applications.Adapters.Shdr.Simulator
{
    public class PathSimulator
    {
        public int Execution { get; set; }
        public int WaitState { get; set; }
        public int ControllerMode { get; set; }
        public bool MachineAxisLock { get; set; }
        public bool SingleBlock { get; set; }
        public bool DryRun { get; set; }


        public string MotionAlarm { get; set; }
        public string SystemAlarm { get; set; }


        public double Feedrate { get; set; }
        public double CuttingSpeed { get; set; }

        public double FeedrateOverride { get; set; }
        public double RapidOverride { get; set; }
        public double SpindleOverride { get; set; }


        public string MainProgram { get; set; }
        public string ActiveProgram { get; set; }
        public string MainProgramComment { get; set; }
        public string ActiveProgramComment { get; set; }
        public int ProgramEdit { get; set; }
        public string ProgramEditName { get; set; }

        public string LineLabel { get; set; }
        public int LineNumber { get; set; }


        public int ToolNumber { get; set; }
        public int ToolGroup { get; set; }


        public string WorkOffset { get; set; }
        public double[] Position { get; set; }
        public double[] Orientation { get; set; }
        public double[] WorkOffsetTranslation { get; set; }
        public double[] WorkOffsetRotation { get; set; }
        public string[] ActiveAxes { get; set; }


        public List<KeyValuePair<string, string>> Variables { get; set; }


        public int PartCount { get; set; }
        public string PartNumber { get; set; }
        public string PartSerialNumber { get; set; }


        public int ChuckState { get; set; }
    }
}
