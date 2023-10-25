using MTConnect.SysML.Xmi;
using MTConnect.SysML.Xmi.UML;
using System.Linq;

namespace MTConnect.SysML.Models.Devices
{
    public class MTConnectDeviceModel : MTConnectClassModel
    {
        public MTConnectDeviceModel(XmiDocument xmiDocument, UmlClass umlClass) : base(xmiDocument, "Devices.Device", umlClass)
        {
            IsAbstract = false;

            // Override MTConnectVersion (to fix case)
            var mtconnectVersionProperty = Properties?.FirstOrDefault(o => o.Name?.ToLower() == "mtconnectversion");
            if (mtconnectVersionProperty != null) mtconnectVersionProperty.Name = "MTConnectVersion";

            Properties?.RemoveAll(o => o.Name == "Adapter");
            Properties?.RemoveAll(o => o.Name == "Auxiliary");
            Properties?.RemoveAll(o => o.Name == "Axis");
            Properties?.RemoveAll(o => o.Name == "ComponentStream");
            Properties?.RemoveAll(o => o.Name == "CompositionStream");
            Properties?.RemoveAll(o => o.Name == "Controller");
            Properties?.RemoveAll(o => o.Name == "DataItemStream");
            Properties?.RemoveAll(o => o.Name == "Interface");
            Properties?.RemoveAll(o => o.Name == "Resource");
            Properties?.RemoveAll(o => o.Name == "Part");
            Properties?.RemoveAll(o => o.Name == "Process");
            Properties?.RemoveAll(o => o.Name == "Structure");
            Properties?.RemoveAll(o => o.Name == "System");
        }
    }
}
