using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTConnect.Applications.Adapters.Shdr.Simulator
{
    public class ControllerSimulator
    {
        public bool EmergencyStop { get; set; }

        public string CommunicationsAlarm { get; set; }

        public string LogicAlarm { get; set; }

        public string SystemAlarm { get; set; }

        public string PalletId { get; set; }
    }
}
