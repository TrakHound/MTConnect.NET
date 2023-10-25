using MTConnect.SysML.Xmi;
using MTConnect.SysML.Xmi.UML;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.SysML.Models.Devices
{
    public class MTConnectCompositionModel : MTConnectClassModel
    {
        public List<MTConnectCompositionType> Types { get; set; } = new();


        public MTConnectCompositionModel(XmiDocument xmiDocument, UmlClass umlClass) : base(xmiDocument, "Devices.Composition", umlClass)
        {
            IsAbstract = false;
            ParentName = null;

            // Composition Type
            var typeProperty = Properties?.FirstOrDefault(o => o.Name == "Type");
            if (typeProperty != null) typeProperty.DataType = "string";

            Properties?.RemoveAll(o => o.Name == "Components");
            Properties?.RemoveAll(o => o.Name == "Compositions");
        }
    }
}
