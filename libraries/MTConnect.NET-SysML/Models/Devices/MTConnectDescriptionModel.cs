using MTConnect.SysML.Xmi;
using MTConnect.SysML.Xmi.UML;

namespace MTConnect.SysML.Models.Devices
{
    public class MTConnectDescriptionModel : MTConnectClassModel
    {
        public MTConnectDescriptionModel(XmiDocument xmiDocument, UmlClass umlClass) : base(xmiDocument, "Devices.Description", umlClass) { }
    }
}
