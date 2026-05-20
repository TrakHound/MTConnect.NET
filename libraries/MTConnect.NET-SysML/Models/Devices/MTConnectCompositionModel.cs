using MTConnect.SysML.Xmi;
using MTConnect.SysML.Xmi.UML;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.SysML.Models.Devices
{
    /// <summary>
    /// Parsed model for the MTConnect <c>Devices.Composition</c> element and
    /// its concrete composition subtypes.
    /// </summary>
    public class MTConnectCompositionModel : MTConnectClassModel
    {
        /// <summary>
        /// The concrete Composition subtypes derived from the
        /// <c>CompositionTypeEnum</c>.
        /// </summary>
        public List<MTConnectCompositionType> Types { get; set; } = new();


        /// <summary>
        /// Parses the <c>Composition</c> class as a concrete root type,
        /// coerces its Type property to <c>string</c>, and removes the nested
        /// Components/Compositions navigations.
        /// </summary>
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
