using MTConnect.SysML.Xmi;
using MTConnect.SysML.Xmi.UML;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.SysML.Models.Observations
{
    public class MTConnectObservationModel : IMTConnectExportModel
    {
        public string UmlId { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public List<MTConnectObservationValueModel> Values { get; set; } = new();


        public MTConnectObservationModel() { }

        public MTConnectObservationModel(XmiDocument xmiDocument, string category, string idPrefix, UmlClass umlClass, UmlEnumerationLiteral umlEnumerationLiteral, UmlEnumeration resultEnumeration)
        {
            if (umlClass != null)
            {
                var name = $"{umlClass.Name.ToTitleCase()}";
                name = name.Replace(".", "");

                Id = $"{idPrefix}.{name}";
                Name = name;

                var description = umlEnumerationLiteral?.Comments?.FirstOrDefault().Body;
                Description = ModelHelper.ProcessDescription(description);

                if (resultEnumeration != null)
                {
                    foreach (var item in resultEnumeration.Items)
                    {
                        Values.Add(new MTConnectObservationValueModel(Id, item));
                    }
                }
            }
        }

        public static IEnumerable<MTConnectObservationModel> Parse(XmiDocument xmiDocument, string category, string idPrefix, IEnumerable<UmlClass> umlClasses, UmlEnumeration umlEnumeration)
        {
            var models = new List<MTConnectObservationModel>();

            if (umlClasses != null)
            {
                foreach (var umlClass in umlClasses)
                {
                    var enumItem = umlEnumeration.Items.FirstOrDefault(o => o.Name.ToTitleCase() == umlClass.Name);
                    if (enumItem != null)
                    {
                        // Result
                        var resultProperty = umlClass.Properties?.FirstOrDefault(o => o.Name == "result");
                        if (resultProperty != null)
                        {
                            var resultEnumeration = ModelHelper.GetEnum(xmiDocument, resultProperty.PropertyType);
                            if (resultEnumeration != null)
                            {
                                models.Add(new MTConnectObservationModel(xmiDocument, category, idPrefix, umlClass, enumItem, resultEnumeration));
                            }
                        }
                    }

                    //// Filter out SubTypes (ex. Type.Subtype)
                    //if (!umlClass.Name.Contains('.'))
                    //{
                    //    var enumItem = umlEnumeration.Items.FirstOrDefault(o => o.Name.ToTitleCase() == umlClass.Name);

                    //    // Result
                    //    var resultProperty = umlClass.Properties?.FirstOrDefault(o => o.Name == "result");
                    //    if (resultProperty != null)
                    //    {
                    //        var resultEnumeration = ModelHelper.GetEnum(xmiDocument, resultProperty.PropertyType);
                    //        if (resultEnumeration != null)
                    //        {
                    //            models.Add(new MTConnectObservationModel(xmiDocument, category, idPrefix, umlClass, enumItem, resultEnumeration));
                    //        }
                    //    }
                    //}
                }
            }

            return models.OrderBy(o => o.Name);
        }
    }
}
