using MTConnect.SysML.Xmi;
using MTConnect.SysML.Xmi.UML;
using System.Linq;

namespace MTConnect.SysML.Models.Devices
{
    /// <summary>
    /// Parsed model for the MTConnect <c>Devices.DataItem</c> element, with
    /// the type/subtype/units overrides and property renames the generated
    /// <c>DataItem</c> base class requires.
    /// </summary>
    public class MTConnectDataItemModel : MTConnectClassModel
    {
        /// <summary>
        /// Parses the <c>DataItem</c> class and applies its overrides: the
        /// Type, SubType, Units, and NativeUnits properties are coerced to
        /// <c>string</c>; Constraint/Relationship are pluralized; and the
        /// inverse Observation navigation is dropped.
        /// </summary>
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

            // Override Constraints
            var constraintProperty = Properties?.FirstOrDefault(o => o.Name == "Constraint");
            if (constraintProperty != null) constraintProperty.Name = "Constraints";

            // Override Relationships
            var relationshipProperty = Properties?.FirstOrDefault(o => o.Name == "Relationship");
            if (relationshipProperty != null) relationshipProperty.Name = "Relationships";

            Properties?.RemoveAll(o => o.Name == "Observation");
        }
    }
}
