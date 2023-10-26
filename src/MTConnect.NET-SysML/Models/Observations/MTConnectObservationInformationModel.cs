using MTConnect.SysML.Models.Devices;
using MTConnect.SysML.Xmi;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.SysML.Models.Observations
{
    public class MTConnectObservationInformationModel
    {
        public List<MTConnectObservationModel> Models { get; set; } = new();


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
                        Models.AddRange(MTConnectObservationModel.Parse(xmiDocument, "Condition", "Observations.Conditions", conditionValues.Classes, conditionEnum));

                        // Events
                        var eventEnum = umlModel.Profiles.FirstOrDefault().Packages.FirstOrDefault().Enumerations.FirstOrDefault(o => o.Name == "EventEnum");
                        var eventValues = observationTypesPackage.Packages.FirstOrDefault(o => o.Name == "Event Types");
                        Models.AddRange(MTConnectObservationModel.Parse(xmiDocument, "Event", "Observations.Events", eventValues.Classes, eventEnum));

                        // Samples
                        var sampleEnum = umlModel.Profiles.FirstOrDefault().Packages.FirstOrDefault().Enumerations.FirstOrDefault(o => o.Name == "SampleEnum");
                        var sampleValues = observationTypesPackage.Packages.FirstOrDefault(o => o.Name == "Sample Types");
                        Models.AddRange(MTConnectObservationModel.Parse(xmiDocument, "Sample", "Observations.Samples", sampleValues.Classes, sampleEnum));
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
                        Models.AddRange(MTConnectObservationModel.Parse(xmiDocument, "Event", "Observations.Events", interfaceDataItems.Classes, eventEnum));
                    }
                }
            }
        }
    }
}
