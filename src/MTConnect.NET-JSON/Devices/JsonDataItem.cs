// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations.Relationships;
using MTConnect.Devices.DataItems;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// An abstract XML Element. Replaced in the XML document by Elements representing various types of DataItem XML Elements.
    /// There can be mulitple types of DataItem XML Elements in the document.
    /// </summary>
    public class JsonDataItem
    {
        /// <summary>
        /// Specifies the kind of information provided by a data item.
        /// Each category of information will provide similar characteristics in its representation.
        /// The available options are SAMPLE, EVENT, or CONDITION.
        /// </summary>
        [JsonPropertyName("category")]
        public string DataItemCategory { get; set; }

        /// <summary>
        /// The unique identifier for this DataItem.
        /// The id attribute MUST be unique across the entire document including the ids for components.
        /// An XML ID-type.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The type of data being measured.
        /// Examples of types are POSITION, VELOCITY, ANGLE, BLOCK, ROTARY_VELOCITY, etc.
        /// </summary>
        [JsonPropertyName("type")]
         public string Type { get; set; }

        /// <summary>
        /// The coordinate system being used.
        /// The available values for coordinateSystem are WORK and MACHINE.
        /// </summary>
        [JsonPropertyName("coordinateSystem")]
        public string CoordinateSystem { get; set; }

        /// <summary>
        /// The name of the DataItem. A name is provided as an additional human readable identifier for this DataItem in addtion to the id.
        /// It is not required and will be implementation dependent.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The multiplier for the native units.
        /// The received data MAY be divided by this value before conversion.
        /// If provided, the value MUST be numeric.
        /// </summary>
        [JsonPropertyName("nativeScale")]
        public double? NativeScale { get; set; }

        /// <summary>
        /// The native units used by the Component.
        /// These units will be converted before they are delivered to the application.
        /// </summary>
        [JsonPropertyName("nativeUnits")]
        public string NativeUnits { get; set; }

        /// <summary>
        /// A sub-categorization of the data item type.
        /// For example, the Sub-types of POSITION can be ACTUAL or COMMANDED.
        /// Not all types have subTypes and they can be optional.
        /// </summary>
        [JsonPropertyName("subType")]
        public string SubType { get; set; }

        /// <summary>
        /// Data calculated specific to a DataItem.
        /// Examples of statistic are AVERAGE, MINIMUM, MAXIMUM, ROOT_MEAN_SQUARE, RANGE, MEDIAN, MODE and STANDARD_DEVIATION.
        /// </summary>
        [JsonPropertyName("statistic")]
        public string Statistic { get; set; }

        /// <summary>
        /// Units MUST be present for all DataItem elements in the SAMPLE category.
        /// If the data represented by a DataItem is a numeric value, except for line number and count, the units MUST be specified.
        /// </summary>
        [JsonPropertyName("units")]
        public string Units { get; set; }

        /// <summary>
        /// The reate at which successive samples of a DataItem are recorded.
        /// SampleRate is expressed in terms of samples per second.
        /// If the SampleRate is smaller than one, the number can be represented as a floating point number.
        /// For example, a rate 1 per 10 seconds would be 0.1.
        /// </summary>
        [JsonPropertyName("sampleRate")]
        public double? SampleRate { get; set; }

        /// <summary>
        /// An indication signifying whether each value reported for the Data Entity is significant and whether duplicate values are to be suppressed.
        /// </summary>
        [JsonPropertyName("discrete")]
        public bool? Discrete { get; set; }

        /// <summary>
        /// Data consisting of multiple data points or samples or a file presented as a single DataItem.
        /// Each representation will have a unique format defined for each representation. 
        /// Examples or representation are VALUE, TIME_SERIES, DISCRETE, MP3, WAV, etc.
        /// Initially, the represenation for TIME_SERIES, DISCRETE, and VALUE are defined.
        /// If a representation is not specified, it MUST be determined to be a VALUE.
        /// </summary>
        [JsonPropertyName("representation")]
        public string Representation { get; set; }

        /// <summary>
        /// The number of significant digits in the reported value.
        /// This is used by applications to dtermine accuracy of values.
        /// This SHOULD be specified for all numeric values.
        /// </summary>
        [JsonPropertyName("significantDigits")]
        public int? SignificantDigits { get; set; }

        /// <summary>
        /// Source is an XML element that indentifies the Component, Subcomponent, or DataItem representing the part of the device from which a measured value originates.
        /// </summary>
        [JsonPropertyName("source")]
        public JsonSource Source { get; set; }

        /// <summary>
        /// The set of possible values that can be assigned to this DataItem.
        /// </summary>
        [JsonPropertyName("constraints")]
        public JsonConstraints Constraints { get; set; }

        /// <summary>
        /// The set of possible values that can be assigned to this DataItem.
        /// </summary>
        [JsonPropertyName("filters")]
        public IEnumerable<JsonFilter> Filters { get; set; }

        /// <summary>
        /// InitialValue is an optional XML element that defines the starting value for a data item as well as the value to be set for the data item after a reset event.
        /// </summary>
        [JsonPropertyName("initialValue")]
        public string InitialValue { get; set; }

        /// <summary>
        /// ResetTrigger is an XML element that describes the reset action that causes a reset to occur.
        /// </summary>
        [JsonPropertyName("resetTrigger")]
        public string ResetTrigger { get; set; }

        /// <summary>
        /// The Definition provides additional descriptive information for any DataItem representations.
        /// When the representation is either DATA_SET or TABLE, it gives the specific meaning of a key and MAY provide a Description, type, and units for semantic interpretation of data.
        /// </summary>
        [JsonPropertyName("definition")]
        public JsonDataItemDefinition Definition { get; set; }

        /// <summary>
        /// Relationships organizes DataItemRelationship and SpecificationRelationship.
        /// </summary>
        [JsonPropertyName("relationships")]
        public JsonRelationshipContainer Relationships { get; set; }


        public JsonDataItem() { }

        public JsonDataItem(IDataItem dataItem)
        {
            if (dataItem != null)
            {
                DataItemCategory = dataItem.Category.ToString();
                Id = dataItem.Id;
                Name = dataItem.Name;
                Type = dataItem.Type;
                SubType = dataItem.SubType;
                NativeUnits = dataItem.NativeUnits;
                if (dataItem.NativeScale > 0) NativeScale = dataItem.NativeScale;
                if (dataItem.SampleRate > 0) SampleRate = dataItem.SampleRate;
                if (dataItem.Source != null) Source = new JsonSource(dataItem.Source);

                // Relationships
                if (!dataItem.Relationships.IsNullOrEmpty())
                {
                    var relationships = new JsonRelationshipContainer();
                    foreach (var relationship in dataItem.Relationships)
                    {
                        // ComponentRelationship
                        if (typeof(IComponentRelationship).IsAssignableFrom(relationship.GetType()))
                        {
                            if (relationships.ComponentRelationships == null) relationships.ComponentRelationships = new List<JsonRelationship>();
                            relationships.ComponentRelationships.Add(new JsonRelationship((IComponentRelationship)relationship));
                        }

                        // DataItemRelationship
                        if (typeof(IDataItemRelationship).IsAssignableFrom(relationship.GetType()))
                        {
                            if (relationships.DataItemRelationships == null) relationships.DataItemRelationships = new List<JsonRelationship>();
                            relationships.DataItemRelationships.Add(new JsonRelationship((IDataItemRelationship)relationship));
                        }

                        // DeviceRelationship
                        if (typeof(IDeviceRelationship).IsAssignableFrom(relationship.GetType()))
                        {
                            if (relationships.DeviceRelationships == null) relationships.DeviceRelationships = new List<JsonRelationship>();
                            relationships.DeviceRelationships.Add(new JsonRelationship((IDeviceRelationship)relationship));
                        }

                        // SpecificationRelationship
                        if (typeof(ISpecificationRelationship).IsAssignableFrom(relationship.GetType()))
                        {
                            if (relationships.SpecificationRelationships == null) relationships.SpecificationRelationships = new List<JsonRelationship>();
                            relationships.SpecificationRelationships.Add(new JsonRelationship((ISpecificationRelationship)relationship));
                        }
                    }
                    Relationships = relationships;
                }

                if (dataItem.Representation != DataItemRepresentation.VALUE) Representation = dataItem.Representation.ToString();
                if (dataItem.ResetTrigger != DataItemResetTrigger.NONE) ResetTrigger = dataItem.ResetTrigger.ToString();
                if (dataItem.CoordinateSystem != DataItemCoordinateSystem.MACHINE) CoordinateSystem = dataItem.CoordinateSystem.ToString();
                if (dataItem.Constraints != null) Constraints = new JsonConstraints(dataItem.Constraints);
                if (dataItem.Definition != null) Definition = new JsonDataItemDefinition(dataItem.Definition);
                Units = dataItem.Units;
                if (dataItem.Statistic != DataItemStatistic.NONE) Statistic = dataItem.Statistic.ToString();
                if (dataItem.SignificantDigits > 0) SignificantDigits = dataItem.SignificantDigits;

                if (!dataItem.Filters.IsNullOrEmpty())
                {
                    var filters = new List<JsonFilter>();
                    foreach (var filter in dataItem.Filters)
                    {
                        filters.Add(new JsonFilter(filter));
                    }
                    Filters = filters;
                }

                InitialValue = dataItem.InitialValue;
                if (dataItem.Discrete != false) Discrete = dataItem.Discrete;
            }
        }


        public override string ToString() => JsonFunctions.Convert(this);

        public DataItem ToDataItem()
        {
            var dataItem = DataItem.Create(Type);
            if (dataItem == null) dataItem = new DataItem();

            dataItem.Category = DataItemCategory.ConvertEnum<DataItemCategory>();
            dataItem.Id = Id;
            dataItem.Name = Name;
            dataItem.Type = Type;
            dataItem.SubType = SubType;
            dataItem.NativeUnits = NativeUnits;
            dataItem.NativeScale = NativeScale.HasValue ? NativeScale.Value : 0;
            dataItem.SampleRate = SampleRate.HasValue ? SampleRate.Value : 0;
            if (Source != null) dataItem.Source = Source.ToSource();

            // Relationships
            if (Relationships != null)
            {
                var relationships = new List<IRelationship>();

                // ComponentRelationship
                if (!Relationships.ComponentRelationships.IsNullOrEmpty())
                {
                    foreach (var relationship in Relationships.ComponentRelationships)
                    {
                        relationships.Add(relationship.ToComponentRelationship());
                    }
                }

                // DataItemRelationship
                if (!Relationships.DataItemRelationships.IsNullOrEmpty())
                {
                    foreach (var relationship in Relationships.DataItemRelationships)
                    {
                        relationships.Add(relationship.ToDataItemRelationship());
                    }
                }

                // DeviceRelationship
                if (!Relationships.DeviceRelationships.IsNullOrEmpty())
                {
                    foreach (var relationship in Relationships.DeviceRelationships)
                    {
                        relationships.Add(relationship.ToDeviceRelationship());
                    }
                }

                // SpecificationRelationship
                if (!Relationships.SpecificationRelationships.IsNullOrEmpty())
                {
                    foreach (var relationship in Relationships.SpecificationRelationships)
                    {
                        relationships.Add(relationship.ToSpecificationRelationship());
                    }
                }

                dataItem.Relationships = relationships;
            }

            dataItem.Representation = Representation.ConvertEnum<DataItemRepresentation>();
            dataItem.ResetTrigger = ResetTrigger.ConvertEnum<DataItemResetTrigger>();
            dataItem.CoordinateSystem = CoordinateSystem.ConvertEnum<DataItemCoordinateSystem>();
            if (Constraints != null) dataItem.Constraints = Constraints.ToConstraints();
            if (Definition != null) dataItem.Definition = Definition.ToDefinition();
            dataItem.Units = Units;
            dataItem.Statistic = Statistic.ConvertEnum<DataItemStatistic>();
            dataItem.SignificantDigits = SignificantDigits.HasValue ? SignificantDigits.Value : 0;

            // Filters
            if (!Filters.IsNullOrEmpty())
            {
                var filters = new List<IFilter>();
                foreach (var filter in Filters)
                {
                    filters.Add(filter.ToFilter());
                }
                dataItem.Filters = filters;
            }

            dataItem.InitialValue = InitialValue;
            if (Discrete.HasValue) dataItem.Discrete = Discrete.Value;

            return dataItem;
        }
    }
}