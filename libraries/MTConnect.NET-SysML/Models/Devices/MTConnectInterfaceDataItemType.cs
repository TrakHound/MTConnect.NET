using MTConnect.SysML.Xmi;
using MTConnect.SysML.Xmi.UML;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.SysML.Models.Devices
{
    /// <summary>
    /// A parsed interface-specific DataItem type. Identical in shape to
    /// <see cref="MTConnectDataItemType"/> but parsed from the Interface
    /// Interaction Model's DataItem package.
    /// </summary>
    public class MTConnectInterfaceDataItemType : MTConnectDataItemType
    {
        /// <summary>
        /// Creates an empty model for manual population.
        /// </summary>
        public MTConnectInterfaceDataItemType() { }

        /// <summary>
        /// Parses an interface DataItem type, delegating to the
        /// <see cref="MTConnectDataItemType"/> base parser.
        /// </summary>
        public MTConnectInterfaceDataItemType(XmiDocument xmiDocument, string category, string idPrefix, UmlClass umlClass, UmlEnumerationLiteral umlEnumerationLiteral, IEnumerable<UmlClass> subClasses = null)
            : base(xmiDocument, category, idPrefix, umlClass, umlEnumerationLiteral, subClasses) { }


        /// <summary>
        /// Parses every top-level interface DataItem type in
        /// <paramref name="umlClasses"/> (excluding <c>Type.SubType</c>
        /// entries), matching each against
        /// <paramref name="umlEnumeration"/>, ordered by name.
        /// </summary>
        public static new IEnumerable<MTConnectInterfaceDataItemType> Parse(XmiDocument xmiDocument, string category, string idPrefix, IEnumerable<UmlClass> umlClasses, UmlEnumeration umlEnumeration)
        {
            var types = new List<MTConnectInterfaceDataItemType>();

            if (umlClasses != null && umlEnumeration != null)
            {
                foreach (var umlClass in umlClasses)
                {
                    // Filter out SubTypes (ex. Type.Subtype)
                    if (!umlClass.Name.Contains('.'))
                    {
                        var enumItem = umlEnumeration.Items.FirstOrDefault(o => o.Name.ToTitleCase() == umlClass.Name);
                        var subClasses = umlClasses.Where(o => o.Name.StartsWith($"{umlClass.Name}."));

                        types.Add(new MTConnectInterfaceDataItemType(xmiDocument, category, idPrefix, umlClass, enumItem, subClasses));
                    }
                }
            }

            return types.OrderBy(o => o.Name);
        }
    }
}
