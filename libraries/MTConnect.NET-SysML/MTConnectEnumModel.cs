using MTConnect.SysML.Xmi;
using MTConnect.SysML.Xmi.UML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MTConnect.SysML
{
    /// <summary>
    /// A parsed enumeration: its disambiguated C# name, cleaned description,
    /// and its ordered set of literal values.
    /// </summary>
    public class MTConnectEnumModel : IMTConnectExportModel
    {
        /// <inheritdoc/>
        public string UmlId { get; set; }

        /// <inheritdoc/>
        public string Id { get; set; }

        /// <summary>
        /// The C# enum name as emitted, after
        /// <see cref="ModelHelper.ConvertEnumName"/> disambiguation.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The cleaned description text emitted into the doc comment.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The enumeration's literal members, ordered by name.
        /// </summary>
        public List<MTConnectEnumValueModel> Values { get; set; } = new();


        /// <summary>
        /// Creates an empty model for manual population.
        /// </summary>
        public MTConnectEnumModel() { }

        /// <summary>
        /// Parses an enumeration under <paramref name="idPrefix"/>, applying
        /// the title-case and disambiguation rules to its name and parsing
        /// each literal (optionally rewritten by
        /// <paramref name="convertFunction"/>) in name order.
        /// </summary>
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

        /// <summary>
        /// Parses every enumeration in <paramref name="umlEnumerations"/>
        /// under <paramref name="idPrefix"/>, returning them ordered by name.
        /// </summary>
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
