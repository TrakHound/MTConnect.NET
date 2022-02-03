using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTConnect.Applications.Adapters.Shdr.Simulator
{
    public class SpindleAxisSimulator : RotaryAxisSimulator
    {
        public double SpindleLoad { get; set; }

        public double SpindleSpeed { get; set; }


        public SpindleAxisSimulator(string name)
        {
            Name = name;
        }
    }
}
