using MTConnect.SysML.Xmi;
using MTConnect.SysML.Xmi.UML;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.SysML.Models.Devices
{
    public class MTConnectInterfaceDataItemType : MTConnectDataItemType
    {
        public MTConnectInterfaceDataItemType() { }

        public MTConnectInterfaceDataItemType(XmiDocument xmiDocument, string category, string idPrefix, UmlClass umlClass, UmlEnumerationLiteral umlEnumerationLiteral, IEnumerable<UmlClass> subClasses = null)
            : base(xmiDocument, category, idPrefix, umlClass, umlEnumerationLiteral, subClasses) { }


        public static IEnumerable<MTConnectInterfaceDataItemType> Parse(XmiDocument xmiDocument, string category, string idPrefix, IEnumerable<UmlClass> umlClasses, UmlEnumeration umlEnumeration)
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
