using MTConnect.SysML.Xmi;
using System.Linq;

namespace MTConnect.SysML.Models.Devices
{
    public class MTConnectDeviceInformationModel
    {
        public MTConnectDeviceModel Device { get; private set; }

        public MTConnectComponentsModel Components { get; private set; }

        public MTConnectCompositionsModel Compositions { get; private set; }

        public MTConnectDataItemsModel DataItems { get; private set; }

        public MTConnectDescriptionModel Description { get; private set; }

        public MTConnectConfigurationModel Configurations { get; private set; }

        public MTConnectPackageModel References { get; private set; } = new();


        public MTConnectDeviceInformationModel() { }

        public MTConnectDeviceInformationModel(XmiDocument xmiDocument)
        {
            Parse(xmiDocument);
        }


        private void Parse(XmiDocument xmiDocument)
        {
            if (xmiDocument != null)
            {
                var umlModel = xmiDocument.Model;

                // Find Device Information Model in the UML
                var deviceInformationModel = umlModel.Packages.FirstOrDefault(o => o.Name == "Device Information Model");
                var observationInformationModel = umlModel.Packages.FirstOrDefault(o => o.Name == "Observation Information Model");
                if (deviceInformationModel != null && observationInformationModel != null)
                {
                    // Components
                    var components = deviceInformationModel.Packages?.FirstOrDefault(o => o.Name == "Components");
                    if (components != null)
                    {
                        Components = new MTConnectComponentsModel();

                        // Component
                        var componentClass = components.Classes?.FirstOrDefault(o => o.Name == "Component");
                        Components.Component = new MTConnectComponentModel(xmiDocument, componentClass);

                        var componentTypes = components.Packages?.FirstOrDefault(o => o.Name == "Component Types");
                        Components.Types.AddRange(MTConnectComponentType.Parse(xmiDocument, "Devices.Components", componentTypes.Classes));

                        var componentOrganizerTypes = componentTypes.Packages?.FirstOrDefault(o => o.Name == "Component Organizer Types");
                        Components.Types.AddRange(MTConnectComponentType.Parse(xmiDocument, "Devices.Components", componentOrganizerTypes.AssociationClasses, true));

                        // Description
                        var descriptionClass = components.Classes?.FirstOrDefault(o => o.Name == "Description");
                        Description = new MTConnectDescriptionModel(xmiDocument, descriptionClass);
                    }


                    // Device
                    var deviceClass = deviceInformationModel.Classes?.FirstOrDefault();
                    Device = new MTConnectDeviceModel(xmiDocument, deviceClass);
                    Device.AddProperties(Components.Component.Properties);


                    // Compositions
                    var compositions = deviceInformationModel.Packages?.FirstOrDefault(o => o.Name == "Compositions");
                    if (compositions != null)
                    {
                        Compositions = new MTConnectCompositionsModel();

                        // Composition
                        var compositionClass = compositions.Classes?.FirstOrDefault(o => o.Name == "Composition");
                        Compositions.Composition = new MTConnectCompositionModel(xmiDocument, compositionClass);
                        Compositions.Composition.AddProperties(Components.Component.Properties);

                        // Composition Types
                        var compositionEnum = umlModel.Profiles.FirstOrDefault().Packages.FirstOrDefault().Enumerations.FirstOrDefault(o => o.Name == "CompositionTypeEnum");
                        Compositions.Types.AddRange(MTConnectCompositionType.Parse(xmiDocument, "Devices.Compositions", compositionEnum));
                    }


                    // DataItems
                    var deviceDataItems = deviceInformationModel.Packages?.FirstOrDefault(o => o.Name == "DataItems");
                    if (deviceDataItems != null)
                    {
                        DataItems = new MTConnectDataItemsModel();

                        // DataItem
                        var dataItemClass = deviceDataItems.Classes?.FirstOrDefault(o => o.Name == "DataItem");
                        DataItems.DataItem = new MTConnectDataItemModel(xmiDocument, dataItemClass);

                        // DataItem Properties
                        var dataItemProperties = deviceDataItems.Packages?.FirstOrDefault(o => o.Name == "Properties of DataItem");
                        if (dataItemProperties != null)
                        {
                            DataItems.Classes.AddRange(MTConnectClassModel.Parse(xmiDocument, "Devices", dataItemProperties.Classes));
                            DataItems.Classes.RemoveAll(o => o.Id == "Devices.MinimumDeltaFilter");
                            DataItems.Classes.RemoveAll(o => o.Id == "Devices.PeriodFilter");

                            // Don't make Filter abstract (use Type instead)
                            var filterModel = DataItems.Classes.FirstOrDefault(o => o.Id == "Devices.Filter");
                            if (filterModel != null) filterModel.IsAbstract = false;

                            // Properties of Definition
                            var definitionProperties = dataItemProperties.Packages?.FirstOrDefault(o => o.Name == "Properties of Definition");
                            if (definitionProperties != null)
                            {
                                DataItems.Classes.AddRange(MTConnectClassModel.Parse(xmiDocument, "Devices", definitionProperties.Classes));
                            }

                            // Relationship Types for DataItem
                            var dataItemRelationships = dataItemProperties.Packages?.FirstOrDefault(o => o.Name == "Relationship Types for DataItem");
                            if (dataItemRelationships != null)
                            {
                                DataItems.Classes.AddRange(MTConnectClassModel.Parse(xmiDocument, "Devices", dataItemRelationships.Classes));
                            }
                        }

                        var observationTypes = observationInformationModel.Packages?.FirstOrDefault(o => o.Name == "Observation Types");
                        if (observationTypes != null)
                        {
                            var conditionEnum = umlModel.Profiles.FirstOrDefault().Packages.FirstOrDefault().Enumerations.FirstOrDefault(o => o.Name == "ConditionEnum");
                            var conditionTypes = observationTypes.Packages?.FirstOrDefault(o => o.Name == "Condition Types");
                            DataItems.Types.AddRange(MTConnectDataItemType.Parse(xmiDocument, "CONDITION", "Devices.DataItems", conditionTypes.Classes, conditionEnum));

                            var eventEnum = umlModel.Profiles.FirstOrDefault().Packages.FirstOrDefault().Enumerations.FirstOrDefault(o => o.Name == "EventEnum");
                            var eventTypes = observationTypes.Packages?.FirstOrDefault(o => o.Name == "Event Types");
                            DataItems.Types.AddRange(MTConnectDataItemType.Parse(xmiDocument, "EVENT", "Devices.DataItems", eventTypes.Classes, eventEnum));

                            var sampleEnum = umlModel.Profiles.FirstOrDefault().Packages.FirstOrDefault().Enumerations.FirstOrDefault(o => o.Name == "SampleEnum");
                            var sampleTypes = observationTypes.Packages?.FirstOrDefault(o => o.Name == "Sample Types");
                            DataItems.Types.AddRange(MTConnectDataItemType.Parse(xmiDocument, "SAMPLE", "Devices.DataItems", sampleTypes.Classes, sampleEnum));
                        }

                        // Add Enums
                        var profile = xmiDocument.Model.Profiles.FirstOrDefault();
                        var dataTypes = profile.Packages.FirstOrDefault(o => o.Name == "DataTypes");

                        DataItems.Enums.Add(new MTConnectEnumModel(xmiDocument, "Devices", dataTypes?.Enumerations.FirstOrDefault(o => o.Name == "UnitEnum")));
                        DataItems.Enums.Add(new MTConnectEnumModel(xmiDocument, "Devices", dataTypes?.Enumerations.FirstOrDefault(o => o.Name == "NativeUnitEnum")));
                        DataItems.Enums.Add(new MTConnectEnumModel(xmiDocument, "Devices", dataTypes?.Enumerations.FirstOrDefault(o => o.Name == "CategoryEnum")));
                        DataItems.Enums.Add(new MTConnectEnumModel(xmiDocument, "Devices", dataTypes?.Enumerations.FirstOrDefault(o => o.Name == "CoordinateSystemEnum")));
                        DataItems.Enums.Add(new MTConnectEnumModel(xmiDocument, "Devices", dataTypes?.Enumerations.FirstOrDefault(o => o.Name == "DataItemRelationshipTypeEnum")));
                        DataItems.Enums.Add(new MTConnectEnumModel(xmiDocument, "Devices", dataTypes?.Enumerations.FirstOrDefault(o => o.Name == "FilterEnum")));
                        DataItems.Enums.Add(new MTConnectEnumModel(xmiDocument, "Devices", dataTypes?.Enumerations.FirstOrDefault(o => o.Name == "RepresentationEnum")));
                        DataItems.Enums.Add(new MTConnectEnumModel(xmiDocument, "Devices", dataTypes?.Enumerations.FirstOrDefault(o => o.Name == "ResetTriggerEnum")));
                        DataItems.Enums.Add(new MTConnectEnumModel(xmiDocument, "Devices", dataTypes?.Enumerations.FirstOrDefault(o => o.Name == "SpecificationRelationshipTypeEnum")));
                        DataItems.Enums.Add(new MTConnectEnumModel(xmiDocument, "Devices", dataTypes?.Enumerations.FirstOrDefault(o => o.Name == "StatisticEnum")));

                        // Change the name of "FilterEnum" to "FilterTypeEnum"
                        if (DataItems.DataItem.Properties != null)
                        {
                            foreach (var propertyModel in DataItems.DataItem.Properties)
                            {
                                if (propertyModel.DataType == "FilterEnum") propertyModel.DataType = "FilterTypeEnum";
                            }
                        }
                        foreach (var dataItemSubClass in DataItems.Classes)
                        {
                            foreach (var propertyModel in DataItems.DataItem.Properties)
                            {
                                if (propertyModel.DataType == "FilterEnum") propertyModel.DataType = "FilterTypeEnum";
                            }
                        }

                        // Change name of "Definition" to "DataItemDefinition"
                        if (DataItems.DataItem.Properties != null)
                        {
                            foreach (var propertyModel in DataItems.DataItem.Properties)
                            {
                                if (propertyModel.DataType == "Definition") propertyModel.DataType = "DataItemDefinition";
                            }
                        }
                        foreach (var classModel in DataItems.Classes)
                        {
                            if (classModel.Id == "Devices.Definition")
                            {
                                classModel.Id = "Devices.DataItemDefinition";
                                classModel.Name = "DataItemDefinition";
                            }

                            if (classModel.ParentName == "Definition") classModel.ParentName = "DataItemDefinition";

                            if (classModel.Properties != null)
                            {
                                foreach (var propertyModel in classModel.Properties)
                                {
                                    if (propertyModel.DataType == "Definition") propertyModel.DataType = "DataItemDefinition";
                                }
                            }
                        }
                    }


                    // Configurations
                    var configurations = deviceInformationModel.Packages?.FirstOrDefault(o => o.Name == "Configurations");
                    if (configurations != null)
                    {
                        // Configuration
                        var configurationClass = configurations.Classes?.FirstOrDefault(o => o.Name == "Configuration");
                        Configurations = new MTConnectConfigurationModel(xmiDocument, configurationClass);


                        foreach (var package in configurations.Packages)
                        {
                            Configurations.Classes.AddRange(MTConnectClassModel.Parse(xmiDocument, "Devices.Configurations", package.Classes));
                        }

                        //// Change name of "ConfigurationRelationship" to "Relationship"
                        //if (Configurations.Properties != null)
                        //{
                        //    foreach (var propertyModel in Configurations.Properties)
                        //    {
                        //        if (propertyModel.DataType == "ConfigurationRelationship") propertyModel.DataType = "Relationship";
                        //    }
                        //}
                        //foreach (var classModel in Configurations.Classes)
                        //{
                        //    if (classModel.Id == "Devices.Configurations.ConfigurationRelationship")
                        //    {
                        //        classModel.Id = "Devices.Configurations.Relationship";
                        //        classModel.Name = "Relationship";
                        //    }

                        //    if (classModel.ParentName == "ConfigurationRelationship") classModel.ParentName = "Relationship";

                        //    if (classModel.Properties != null)
                        //    {
                        //        foreach (var propertyModel in classModel.Properties)
                        //        {
                        //            if (propertyModel.DataType == "ConfigurationRelationship") propertyModel.DataType = "Relationship";
                        //        }
                        //    }
                        //}

                        // Add Enums
                        var profile = xmiDocument.Model.Profiles.FirstOrDefault();
                        var dataTypes = profile.Packages.FirstOrDefault(o => o.Name == "DataTypes");

                        Configurations.Enums.Add(new MTConnectEnumModel(xmiDocument, "Devices.Configurations", dataTypes?.Enumerations.FirstOrDefault(o => o.Name == "CoordinateSystemTypeEnum")));
                        Configurations.Enums.Add(new MTConnectEnumModel(xmiDocument, "Devices.Configurations", dataTypes?.Enumerations.FirstOrDefault(o => o.Name == "MediaTypeEnum")));
                        Configurations.Enums.Add(new MTConnectEnumModel(xmiDocument, "Devices.Configurations", dataTypes?.Enumerations.FirstOrDefault(o => o.Name == "MotionActuationTypeEnum")));
                        Configurations.Enums.Add(new MTConnectEnumModel(xmiDocument, "Devices.Configurations", dataTypes?.Enumerations.FirstOrDefault(o => o.Name == "MotionTypeEnum")));
                        Configurations.Enums.Add(new MTConnectEnumModel(xmiDocument, "Devices.Configurations", dataTypes?.Enumerations.FirstOrDefault(o => o.Name == "RelationshipTypeEnum")));
                        Configurations.Enums.Add(new MTConnectEnumModel(xmiDocument, "Devices.Configurations", dataTypes?.Enumerations.FirstOrDefault(o => o.Name == "CriticalityTypeEnum")));
                        Configurations.Enums.Add(new MTConnectEnumModel(xmiDocument, "Devices.Configurations", dataTypes?.Enumerations.FirstOrDefault(o => o.Name == "RoleTypeEnum")));
                        Configurations.Enums.Add(new MTConnectEnumModel(xmiDocument, "Devices.Configurations", dataTypes?.Enumerations.FirstOrDefault(o => o.Name == "OriginatorEnum")));
                    }

                    // References
                    var references = deviceInformationModel.Packages?.FirstOrDefault(o => o.Name == "References");
                    if (references != null)
                    {
                        References.Classes.AddRange(MTConnectClassModel.Parse(xmiDocument, "Devices.References", references.Classes));

                        foreach (var referenceClass in References.Classes)
                        {
                            if (referenceClass.Id == "Devices.References.ComponentRef")
                            {
                                referenceClass.Id = "Devices.References.ComponentReference";
                                referenceClass.Name = "ComponentReference";
                                referenceClass.Properties?.RemoveAll(o => o.Name == "IdRef");
                            }

                            if (referenceClass.Id == "Devices.References.DataItemRef")
                            {
                                referenceClass.Id = "Devices.References.DataItemReference";
                                referenceClass.Name = "DataItemReference";
                                referenceClass.Properties?.RemoveAll(o => o.Name == "IdRef");
                            }
                        }
                    }
                }
            }
        }
    }
}
