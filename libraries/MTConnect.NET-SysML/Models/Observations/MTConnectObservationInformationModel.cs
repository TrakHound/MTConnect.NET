using MTConnect.SysML.Xmi;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.SysML.Models.Observations
{
    public class MTConnectObservationInformationModel
    {
        public List<MTConnectObservationModel> Types { get; set; } = new();

        public List<MTConnectClassModel> Results { get; set; } = new();


        public MTConnectObservationInformationModel() { }

        public MTConnectObservationInformationModel(XmiDocument xmiDocument)
        {
            Parse(xmiDocument);
        }


        private void Parse(XmiDocument xmiDocument)
        {
            if (xmiDocument != null)
            {
                var umlModel = xmiDocument.Model;

                // Find Information Model in the UML
                var observationInformationModel = umlModel.Packages.FirstOrDefault(o => o.Name == "Observation Information Model");
                if (observationInformationModel != null)
                {
                    var observationTypesPackage = observationInformationModel.Packages.FirstOrDefault(o => o.Name == "Observation Types");
                    if (observationTypesPackage != null)
                    {
                        // Conditions
                        var conditionEnum = umlModel.Profiles.FirstOrDefault().Packages.FirstOrDefault().Enumerations.FirstOrDefault(o => o.Name == "ConditionEnum");
                        var conditionValues = observationTypesPackage.Packages.FirstOrDefault(o => o.Name == "Condition Types");
                        Types.AddRange(MTConnectObservationModel.Parse(xmiDocument, "Condition", "Observations.Conditions", conditionValues.Classes, conditionEnum));

                        // Events
                        var eventEnum = umlModel.Profiles.FirstOrDefault().Packages.FirstOrDefault().Enumerations.FirstOrDefault(o => o.Name == "EventEnum");
                        var eventValues = observationTypesPackage.Packages.FirstOrDefault(o => o.Name == "Event Types");
                        Types.AddRange(MTConnectObservationModel.Parse(xmiDocument, "Event", "Observations.Events", eventValues.Classes, eventEnum));

                        // Samples
                        var sampleEnum = umlModel.Profiles.FirstOrDefault().Packages.FirstOrDefault().Enumerations.FirstOrDefault(o => o.Name == "SampleEnum");
                        var sampleValues = observationTypesPackage.Packages.FirstOrDefault(o => o.Name == "Sample Types");
                        Types.AddRange(MTConnectObservationModel.Parse(xmiDocument, "Sample", "Observations.Samples", sampleValues.Classes, sampleEnum));
                    }
                }

                // Find Interface Information Model in the UML
                var interfaceInformationModel = umlModel.Packages.FirstOrDefault(o => o.Name == "Interface Interaction Model");
                if (interfaceInformationModel != null && observationInformationModel != null)
                {
                    // DataItems
                    var interfaceDataItems = interfaceInformationModel.Packages?.FirstOrDefault(o => o.Name == "DataItem Types for Interface");
                    if (interfaceDataItems != null)
                    {
                        // Event Observations
                        var eventEnum = umlModel.Profiles.FirstOrDefault().Packages.FirstOrDefault().Enumerations.FirstOrDefault(o => o.Name == "InterfaceEventEnum");
                        Types.AddRange(MTConnectObservationModel.Parse(xmiDocument, "Event", "Observations.Events", interfaceDataItems.Classes, eventEnum));
                    }
                }

                // Find Observatoin Result Model in the UML
                var dataTypes = umlModel.Profiles.FirstOrDefault().Packages.FirstOrDefault(o => o.Name == "DataTypes");
                if (dataTypes != null)
                {
                    var resultDataTypes = dataTypes.Classes.Where(o => o.Name.EndsWith("Result"));
                    if (!resultDataTypes.IsNullOrEmpty())
                    {
                        Results.AddRange(MTConnectClassModel.Parse(xmiDocument, "Observations.Events", resultDataTypes));
                    }
                }
            }
        }
    }
}
