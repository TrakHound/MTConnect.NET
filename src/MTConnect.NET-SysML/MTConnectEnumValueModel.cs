using MTConnect.SysML.Xmi.UML;
using System;
using System.Linq;

namespace MTConnect.SysML
{
    public class MTConnectEnumValueModel : IMTConnectExportModel
    {
        public string UmlId { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }


        public MTConnectEnumValueModel() { }

        public MTConnectEnumValueModel(string idPrefix, UmlEnumerationLiteral enumerationLiteral, Func<string, string> convertFunction = null)
        {
            if (enumerationLiteral != null)
            {
                UmlId = enumerationLiteral.Id;

                var name = enumerationLiteral.Name;
                if (convertFunction != null) name = convertFunction(name);

                Id = $"{idPrefix}.{name}";
                Name = name;

                var description = enumerationLiteral.Comments?.FirstOrDefault().Body;
                Description = ModelHelper.ProcessDescription(description);
            }
        }
    }
}
