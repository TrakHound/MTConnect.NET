using MTConnect.SysML.Xmi;
using MTConnect.SysML.Xmi.UML;

namespace MTConnect.SysML.Models.Devices
{
    /// <summary>
    /// Parsed model for the MTConnect <c>Devices.Description</c> element.
    /// </summary>
    public class MTConnectDescriptionModel : MTConnectClassModel
    {
        /// <summary>
        /// Parses the <c>Description</c> class from
        /// <paramref name="umlClass"/> under the <c>Devices.Description</c>
        /// model id.
        /// </summary>
        public MTConnectDescriptionModel(XmiDocument xmiDocument, UmlClass umlClass) : base(xmiDocument, "Devices.Description", umlClass) { }
    }
}
