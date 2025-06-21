using MTConnect.SysML.Xmi;
using MTConnect.SysML.Xmi.UML;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.SysML.Models.Assets
{
    public class MTConnectMeasurementModel : MTConnectClassModel
    {
        public string MeasurementType { get; set; }

        public string TypeId { get; set; }

        public string CodeId { get; set; }

        public string Units { get; set; }


        public MTConnectMeasurementModel() { }

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
