using MTConnect.SysML.Xmi;
using MTConnect.SysML.Xmi.UML;

namespace MTConnect.SysML.Models.Devices
{
    public class MTConnectComponentModel : MTConnectClassModel
    {
        public MTConnectComponentModel(XmiDocument xmiDocument, UmlClass umlClass) : base(xmiDocument, "Devices.Component", umlClass)
        {
            IsAbstract = false;

            Properties?.RemoveAll(o => o.Name == "ComponentStream");
            Properties?.RemoveAll(o => o.Name == "CompositionStream");
            Properties?.RemoveAll(o => o.Name == "DataItemStream");
        }
    }
}
