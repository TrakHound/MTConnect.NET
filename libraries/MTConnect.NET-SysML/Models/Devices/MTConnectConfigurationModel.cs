using MTConnect.SysML.Xmi;
using MTConnect.SysML.Xmi.UML;
using System.Collections.Generic;

namespace MTConnect.SysML.Models.Devices
{
    public class MTConnectConfigurationModel : MTConnectClassModel
    {
        public List<MTConnectClassModel> Classes { get; set; } = new();

        public List<MTConnectEnumModel> Enums { get; set; } = new();

        public MTConnectConfigurationModel(XmiDocument xmiDocument, UmlClass umlClass) : base(xmiDocument, "Devices.Configurations.Configuration", umlClass)
        {
            ParentName = null;
        }
    }
}
