using MTConnect.SysML.Xmi.UML;
using System;
using System.Linq;

namespace MTConnect.SysML
{
    public class MTConnectSubclassModel : IMTConnectExportModel
    {
        public string UmlId { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public Version MaximumVersion { get; set; }

        public Version MinimumVersion { get; set; }


        public MTConnectSubclassModel(UmlClass umlClass)
        {
            if (umlClass != null)
            {
                Name = umlClass.Name;

                var description = umlClass.Comments?.FirstOrDefault().Body;
                Description = ModelHelper.ProcessDescription(description);
            }
        }
    }
}
