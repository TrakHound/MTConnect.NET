using MTConnect.SysML.Xmi;
using MTConnect.SysML.Xmi.UML;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.SysML.Models.Devices
{
    public class MTConnectCompositionType : IMTConnectExportModel
    {
        public string UmlId { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string DefaultName { get; set; }

        public Version MaximumVersion { get; set; }

        public Version MinimumVersion { get; set; }


        public MTConnectCompositionType() { }

        public MTConnectCompositionType(XmiDocument xmiDocument, string idPrefix, UmlEnumerationLiteral umlEnumerationLiteral)
        {
            if (umlEnumerationLiteral != null)
            {
                UmlId = umlEnumerationLiteral.Id;

                var name = $"{umlEnumerationLiteral.Name.ToTitleCase()}Composition";

                Id = $"{idPrefix}.{name}";
                Name = name;
                DefaultName = name.ToCamelCase();

                MaximumVersion = MTConnectVersion.LookupDeprecated(xmiDocument, umlEnumerationLiteral.Id);
                MinimumVersion = MTConnectVersion.LookupNormative(xmiDocument, umlEnumerationLiteral.Id);

                var description = umlEnumerationLiteral.Comments?.FirstOrDefault().Body;
                Description = ModelHelper.ProcessDescription(description);
            }
        }


        public static IEnumerable<MTConnectCompositionType> Parse(XmiDocument xmiDocument, string idPrefix, UmlEnumeration umlEnumeration)
        {
            var subClasses = new List<MTConnectCompositionType>();

            if (umlEnumeration != null)
            {
                foreach (var item in umlEnumeration.Items)
                {
                    subClasses.Add(new MTConnectCompositionType(xmiDocument, idPrefix, item));
                }
            }

            return subClasses;
        }
    }
}
