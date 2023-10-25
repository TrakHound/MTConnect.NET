using MTConnect.SysML.Xmi;
using MTConnect.SysML.Xmi.UML;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.SysML.Models.Devices
{
    public class MTConnectComponentType : IMTConnectExportModel
    {
        public string UmlId { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }

        public string ParentName { get; set; }

        public string Description { get; set; }

        public string DefaultName { get; set; }

        public bool IsAbstract { get; set; }

        public bool IsOrganizer { get; set; }

        public Version MaximumVersion { get; set; }

        public Version MinimumVersion { get; set; }


        public MTConnectComponentType() { }

        public MTConnectComponentType(XmiDocument xmiDocument, string idPrefix, UmlClass umlClass, bool isOrganizer = false)
        {
            if (umlClass != null)
            {
                UmlId = umlClass.Id;

                var name = $"{umlClass.Name.ToTitleCase()}Component";

                Id = $"{idPrefix}.{name}";
                Name = name;
                DefaultName = name.ToCamelCase();
                IsAbstract = umlClass.IsAbstract;
                IsOrganizer = isOrganizer;

                MaximumVersion = MTConnectVersion.LookupDeprecated(xmiDocument, umlClass.Id);
                MinimumVersion = MTConnectVersion.LookupNormative(xmiDocument, umlClass.Id);

                // Add SuperClass (ParentType)
                if (umlClass.Generalization != null)
                {
                    ParentName = ModelHelper.GetClassName(xmiDocument, umlClass.Generalization.General);
                    if (ParentName != null && ParentName != "Component") ParentName += "Component";
                }

                var description = umlClass.Comments?.FirstOrDefault().Body;
                Description = ModelHelper.ProcessDescription(description);
            }
        }


        public static IEnumerable<MTConnectComponentType> Parse(XmiDocument xmiDocument, string idPrefix, IEnumerable<UmlClass> umlClasses, bool isOrganizer = false)
        {
            var subClasses = new List<MTConnectComponentType>();

            if (umlClasses != null)
            {
                foreach (var umlClass in umlClasses)
                {
                    subClasses.Add(new MTConnectComponentType(xmiDocument, idPrefix, umlClass, isOrganizer));
                }
            }

            return subClasses;
        }
    }
}
