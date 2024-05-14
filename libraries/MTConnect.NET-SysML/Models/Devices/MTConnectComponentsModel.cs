using System.Collections.Generic;

namespace MTConnect.SysML.Models.Devices
{
    public class MTConnectComponentsModel
    {
        public MTConnectComponentModel Component { get; set; }

        public List<MTConnectComponentType> Types { get; set; } = new();
    }
}
