using MTConnect.SysML.Xmi;
using MTConnect.SysML.Xmi.UML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MTConnect.SysML
{
    public class MTConnectEnumModel : IMTConnectExportModel
    {
        public string UmlId { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public List<MTConnectEnumValueModel> Values { get; set; } = new();


        public MTConnectEnumModel() { }

        public MTConnectEnumModel(XmiDocument xmiDocument, string idPrefix, UmlEnumeration umlEnumeration, Func<string, string> convertFunction = null)
        {
            if (umlEnumeration != null)
            {
                UmlId = umlEnumeration.Id;

                var name = $"{umlEnumeration.Name.ToTitleCase()}";
                name = ModelHelper.ConvertEnumName(name);

                //if (convertFunction != null)
                //{
                //    name = convertFunction(umlEnumeration.Name);
                //}
                //else
                //{
                //    name = $"{umlEnumeration.Name.ToTitleCase()}";
                //    name = ModelHelper.ConvertEnumName(name);
                //}


                Id = $"{idPrefix}.{name}";
                Name = name;

                if (umlEnumeration.Items != null)
                {
                    foreach (var item in umlEnumeration.Items.OrderBy(o => o.Name))
                    {
                        Values.Add(new MTConnectEnumValueModel(Id, item, convertFunction));
                    }
                }
            }
        }

        public static IEnumerable<MTConnectEnumModel> Parse(XmiDocument xmiDocument, string idPrefix, IEnumerable<UmlEnumeration> umlEnumerations, Func<string, string> convertFunction = null)
        {
            var models = new List<MTConnectEnumModel>();

            if (umlEnumerations != null)
            {
                foreach (var umlEnumeration in umlEnumerations)
                {
                    models.Add(new MTConnectEnumModel(xmiDocument, idPrefix, umlEnumeration, convertFunction));
                }
            }

            return models.OrderBy(o => o.Name);
        }
    }
}
