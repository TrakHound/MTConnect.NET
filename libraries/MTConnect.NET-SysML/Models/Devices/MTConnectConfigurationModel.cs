using MTConnect.SysML.Xmi;
using MTConnect.SysML.Xmi.UML;
using System.Collections.Generic;

namespace MTConnect.SysML.Models.Devices
{
    /// <summary>
    /// Parsed model for the MTConnect <c>Devices.Configurations.Configuration</c>
    /// element together with the configuration sub-element classes and
    /// enumerations it carries.
    /// </summary>
    public class MTConnectConfigurationModel : MTConnectClassModel
    {
        /// <summary>
        /// The configuration sub-element classes (coordinate systems,
        /// specifications, sensor configuration, etc.).
        /// </summary>
        public List<MTConnectClassModel> Classes { get; set; } = new();

        /// <summary>
        /// The enumerations referenced by the configuration classes.
        /// </summary>
        public List<MTConnectEnumModel> Enums { get; set; } = new();

        /// <summary>
        /// Parses the <c>Configuration</c> class as a concrete, root-level
        /// type (parent cleared).
        /// </summary>
        public MTConnectConfigurationModel(XmiDocument xmiDocument, UmlClass umlClass) : base(xmiDocument, "Devices.Configurations.Configuration", umlClass)
        {
            ParentName = null;
        }
    }
}
