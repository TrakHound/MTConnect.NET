using MTConnect.SysML.Xmi;
using MTConnect.SysML.Xmi.UML;
using System.Linq;

namespace MTConnect.SysML.Models.Devices
{
    public class MTConnectDataItemModel : MTConnectClassModel
    {
        public MTConnectDataItemModel(XmiDocument xmiDocument, UmlClass umlClass) : base(xmiDocument, "Devices.DataItem", umlClass)
        {
            IsAbstract = false;

            // Override Type
            var typeProperty = Properties?.FirstOrDefault(o => o.Name == "Type");
            if (typeProperty != null) typeProperty.DataType = "string";

            // Override SubType
            var subtypeProperty = Properties?.FirstOrDefault(o => o.Name == "SubType");
            if (subtypeProperty != null) subtypeProperty.DataType = "string";

            // Override Units
            var unitsProperty = Properties?.FirstOrDefault(o => o.Name == "Units");
            if (unitsProperty != null) unitsProperty.DataType = "string";

            // Override NativeUnits
            var nativeUnitsProperty = Properties?.FirstOrDefault(o => o.Name == "NativeUnits");
            if (nativeUnitsProperty != null) nativeUnitsProperty.DataType = "string";


            Properties?.RemoveAll(o => o.Name == "Observation");
        }
    }
}
