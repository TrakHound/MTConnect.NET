using MTConnect.SysML.Xmi;
using MTConnect.SysML.Xmi.UML;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.SysML.Models.Observations
{
    /// <summary>
    /// A parsed observation type (a Condition, Event, or Sample) whose
    /// controlled result vocabulary is emitted as an enum of
    /// <see cref="MTConnectObservationValueModel"/> members.
    /// </summary>
    public class MTConnectObservationModel : IMTConnectExportModel
    {
        /// <inheritdoc/>
        public string UmlId { get; set; }

        /// <inheritdoc/>
        public string Id { get; set; }

        /// <summary>
        /// The emitted C# name for this observation type.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The cleaned description text emitted into the doc comment.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The observation's controlled result values.
        /// </summary>
        public List<MTConnectObservationValueModel> Values { get; set; } = new();


        /// <summary>
        /// Creates an empty model for manual population.
        /// </summary>
        public MTConnectObservationModel() { }

        /// <summary>
        /// Parses an observation type from <paramref name="umlClass"/> under
        /// <paramref name="idPrefix"/>, taking its cleaned description from
        /// the matching enum literal and its result values from
        /// <paramref name="resultEnumeration"/>.
        /// </summary>
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

        /// <summary>
        /// Parses every observation type in <paramref name="umlClasses"/>
        /// that has a result property whose type is an enumeration,
        /// matching each against <paramref name="umlEnumeration"/> and
        /// ordering by name.
        /// </summary>
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
