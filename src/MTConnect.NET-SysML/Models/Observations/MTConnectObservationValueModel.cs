using MTConnect.SysML.Xmi.UML;
using System.Linq;

namespace MTConnect.SysML.Models.Observations
{
    public class MTConnectObservationValueModel : IMTConnectExportModel
    {
        public string UmlId { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }


        public MTConnectObservationValueModel() { }

        public MTConnectObservationValueModel(string idPrefix, UmlEnumerationLiteral enumerationLiteral)
        {
            if (enumerationLiteral != null)
            {
                UmlId = enumerationLiteral.Id;

                var name = enumerationLiteral.Name;

                Id = $"{idPrefix}.{name}";
                Name = name;

                var description = enumerationLiteral.Comments?.FirstOrDefault().Body;
                Description = ModelHelper.ProcessDescription(description);
            }
        }
    }
}
