using System.Collections.Generic;

namespace MTConnect.SysML.Models.Devices
{
    public class MTConnectCompositionsModel
    {
        public MTConnectCompositionModel Composition { get; set; }

        public List<MTConnectCompositionType> Types { get; set; } = new();
    }
}
