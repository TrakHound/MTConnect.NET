using MTConnect.SysML.Xmi;
using MTConnect.SysML.Xmi.UML;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.SysML.Models.Assets
{
    /// <summary>
    /// A parsed cutting-tool measurement type, carrying its measurement
    /// category, MTConnect type id, and the default code and units pulled
    /// from the XMI default-value instances.
    /// </summary>
    public class MTConnectMeasurementModel : MTConnectClassModel
    {
        /// <summary>
        /// The measurement category (for example the asset measurement
        /// group this type belongs to).
        /// </summary>
        public string MeasurementType { get; set; }

        /// <summary>
        /// The MTConnect type id of the measurement.
        /// </summary>
        public string TypeId { get; set; }

        /// <summary>
        /// The default measurement code, resolved from the XMI
        /// <c>code</c> default-value instance, or <c>null</c> when none is
        /// declared.
        /// </summary>
        public string CodeId { get; set; }

        /// <summary>
        /// The default units, resolved from the XMI <c>units</c>
        /// default-value instance, or <c>null</c> when none is declared.
        /// </summary>
        public string Units { get; set; }


        /// <summary>
        /// Creates an empty model for manual population.
        /// </summary>
        public MTConnectMeasurementModel() { }

        /// <summary>
        /// Parses a measurement type from <paramref name="umlClass"/> under
        /// <paramref name="idPrefix"/>, resolving its default code and units
        /// from the XMI default-value instances.
        /// </summary>
        public MTConnectMeasurementModel(XmiDocument xmiDocument, string measurementType, string idPrefix, UmlClass umlClass) : base(null, null, null)
        {
            if (umlClass != null)
            {
                UmlId = umlClass.Id;

                var name = $"{umlClass.Name.ToTitleCase()}Measurement";

                Id = $"{idPrefix}.{name}";
                Name = name;
                MeasurementType = measurementType;
                TypeId = umlClass.Name.ToTitleCase();

                var description = umlClass.Comments?.FirstOrDefault().Body;
                Description = ModelHelper.ProcessDescription(description);

                // Load Properties
                var umlProperties = umlClass.Properties?.Where(o => !o.Name.StartsWith("made") && !o.Name.StartsWith("is") && !o.Name.StartsWith("observes"));
                if (umlProperties != null)
                {
                    foreach (var umlProperty in umlProperties)
                    {
                        // Code
                        if (umlProperty.Name == "code")
                        {
                            var instanceValue = umlProperty.DefaultValue as UmlInstanceValue;
                            if (instanceValue != null)
                            {
                                CodeId = ModelHelper.GetEnumValue(xmiDocument, umlProperty.PropertyType, instanceValue.Instance);
                            }
                        }

                        // Units
                        if (umlProperty.Name == "units")
                        {
                            var instanceValue = umlProperty.DefaultValue as UmlInstanceValue;
                            if (instanceValue != null)
                            {
                                Units = ModelHelper.GetEnumValue(xmiDocument, umlProperty.PropertyType, instanceValue.Instance);
                                //Units = UnitsHelper.Get(Units);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Parses every measurement type in <paramref name="umlClasses"/>
        /// under <paramref name="idPrefix"/>, de-duplicating by id and
        /// ordering by name.
        /// </summary>
        public static IEnumerable<MTConnectMeasurementModel> Parse(XmiDocument xmiDocument, string measurementType, string idPrefix, IEnumerable<UmlClass> umlClasses)
        {
            var models = new List<MTConnectMeasurementModel>();

            if (umlClasses != null)
            {
                foreach (var umlClass in umlClasses)
                {
                    var model = new MTConnectMeasurementModel(xmiDocument, measurementType, idPrefix, umlClass);
                    if (!models.Any(o => o.Id == model.Id))
                    {
                        models.Add(model);
                    }
                }
            }

            return models.OrderBy(o => o.Name);
        }
    }
}
