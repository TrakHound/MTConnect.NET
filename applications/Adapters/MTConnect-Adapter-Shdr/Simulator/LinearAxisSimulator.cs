using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTConnect.Applications.Adapters.Shdr.Simulator
{
    public class LinearAxisSimulator
    {
        public string Name { get; set; }

        public double MachinePosition { get; set; }

        public double WorkPosition { get; set; }

        public bool Overtravel { get; set; }

        public double Load { get; set; }

        public double Feedrate { get; set; }

        public double Temperature { get; set; }

        public int State { get; set; }


        public LinearAxisSimulator(string name)
        {
            Name = name;
        }
    }
}
