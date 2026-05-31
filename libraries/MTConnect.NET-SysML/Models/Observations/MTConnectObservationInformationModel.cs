using MTConnect.SysML.Xmi;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.SysML.Models.Observations
{
    /// <summary>
    /// Parses the Observation Information Model from the XMI, collecting the
    /// Condition, Event, and Sample observation types (including the
    /// interface event observations) plus the <c>*Result</c> value classes.
    /// </summary>
    public class MTConnectObservationInformationModel
    {
        /// <summary>
        /// The parsed Condition, Event, and Sample observation types.
        /// </summary>
        public List<MTConnectObservationModel> Types { get; set; } = new();

        /// <summary>
        /// The parsed <c>*Result</c> value classes referenced by the
        /// observation types.
        /// </summary>
        public List<MTConnectClassModel> Results { get; set; } = new();

        /// <summary>
        /// The per-subtype value vocabularies for the <c>CompositionState</c>
        /// and <c>Direction</c> event types. These DataItems carry a
        /// <c>subType</c> that selects which value enumeration applies, so the
        /// SysML model declares one enumeration per subtype (for example
        /// <c>CompositionStateActionEnum</c> for the <c>ACTION</c> subtype).
        /// Each is emitted under <c>Observations.Events</c> as a strongly
        /// typed enum plus its paired <c>*Descriptions</c> lookup, which the
        /// hand-authored <c>EventValue.cs</c> subtype switch consumes.
        /// </summary>
        public List<MTConnectEnumModel> Enums { get; set; } = new();


        /// <summary>
        /// Creates an empty model for manual population.
        /// </summary>
        public MTConnectObservationInformationModel() { }

        /// <summary>
        /// Parses the observation types and result classes from the given
        /// XMI document.
        /// </summary>
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

                // Find Observation Result Model in the UML
                var dataTypes = umlModel.Profiles.FirstOrDefault().Packages.FirstOrDefault(o => o.Name == "DataTypes");
                if (dataTypes != null)
                {
                    var resultDataTypes = dataTypes.Classes.Where(o => o.Name.EndsWith("Result"));
                    if (!resultDataTypes.IsNullOrEmpty())
                    {
                        Results.AddRange(MTConnectClassModel.Parse(xmiDocument, "Observations.Events", resultDataTypes));
                    }

                    // The CompositionState and Direction event DataItems
                    // resolve their value vocabulary from their subType. The
                    // SysML model carries one enumeration per subtype; emit
                    // each under Observations.Events so the generated enum and
                    // its paired *Descriptions lookup land beside
                    // EventValue.cs, which dispatches on the subType to the
                    // matching *Descriptions.Get(...) call.
                    foreach (var subtypeEnumName in new[]
                    {
                        "CompositionStateActionEnum",
                        "CompositionStateLateralEnum",
                        "CompositionStateMotionEnum",
                        "CompositionStateSwitchedEnum",
                        "CompositionStateVerticalEnum",
                        "DirectionLinearEnum",
                        "DirectionRotaryEnum",
                    })
                    {
                        var subtypeEnum = dataTypes.Enumerations.FirstOrDefault(o => o.Name == subtypeEnumName);
                        if (subtypeEnum != null)
                        {
                            Enums.Add(new MTConnectEnumModel(xmiDocument, "Observations.Events", subtypeEnum));
                        }
                    }
                }
            }
        }
    }
}
