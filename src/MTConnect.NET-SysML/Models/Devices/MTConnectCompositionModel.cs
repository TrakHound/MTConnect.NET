using MTConnect.SysML.Xmi;
using MTConnect.SysML.Xmi.UML;
using System.Collections.Generic;

namespace MTConnect.SysML.Models.Devices
{
    public class MTConnectCompositionModel : MTConnectClassModel
    {
        public List<MTConnectCompositionType> Types { get; set; } = new();


        public MTConnectCompositionModel(XmiDocument xmiDocument, UmlClass umlClass) : base(xmiDocument, "Devices.Composition", umlClass)
        {
            IsAbstract = false;

            Properties?.RemoveAll(o => o.Name == "Components");
            Properties?.RemoveAll(o => o.Name == "Compositions");
        }
    }
}
