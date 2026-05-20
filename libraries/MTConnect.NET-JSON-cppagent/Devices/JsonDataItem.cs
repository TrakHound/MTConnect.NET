// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for an MTConnect <c>DataItem</c> in the
    /// cppagent-compatible JSON shape. Mirrors a DataItem element so the JSON
    /// serializer can read and write the on-the-wire shape, then converts to
    /// and from the strongly-typed <see cref="DataItem"/> model. Properties
    /// that match the MTConnect default are omitted on write and restored on
    /// read.
    /// </summary>
    public class JsonDataItem
    {
        /// <summary>
        /// The <c>category</c> classifying the data item as SAMPLE, EVENT, or
        /// CONDITION.
        /// </summary>
        [JsonPropertyName("category")]
        public string DataItemCategory { get; set; }

        /// <summary>
        /// The unique <c>id</c> of the data item within the device.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The MTConnect <c>type</c> identifying the kind of data reported.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// The coordinate system the reported values are expressed in. Omitted
        /// when the default MACHINE coordinate system applies.
        /// </summary>
        [JsonPropertyName("coordinateSystem")]
        public string CoordinateSystem { get; set; }

        /// <summary>
        /// Reference to the <c>id</c> of a CoordinateSystem the values are
        /// expressed relative to.
        /// </summary>
        [JsonPropertyName("coordinateSystemIdRef")]
        public string CoordinateSystemIdRef { get; set; }

        /// <summary>
        /// The optional human-readable <c>name</c> of the data item. Omitted
        /// from the JSON output when not set.
        /// </summary>
        [JsonPropertyName("name")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Name { get; set; }

        /// <summary>
        /// The power-of-ten scaling applied to native values before conversion
        /// to the reported units. Omitted when zero.
        /// </summary>
        [JsonPropertyName("nativeScale")]
        public double? NativeScale { get; set; }

        /// <summary>
        /// The units the data source natively reports values in, prior to
        /// conversion to <see cref="Units"/>.
        /// </summary>
        [JsonPropertyName("nativeUnits")]
        public string NativeUnits { get; set; }

        /// <summary>
        /// The optional <c>subType</c> further qualifying <see cref="Type"/>.
        /// </summary>
        [JsonPropertyName("subType")]
        public string SubType { get; set; }

        /// <summary>
        /// The statistical operation (for example AVERAGE or MAXIMUM) applied
        /// to the reported values.
        /// </summary>
        [JsonPropertyName("statistic")]
        public string Statistic { get; set; }

        /// <summary>
        /// The engineering units the reported values are expressed in.
        /// </summary>
        [JsonPropertyName("units")]
        public string Units { get; set; }

        /// <summary>
        /// The rate, in samples per second, at which the data source samples
        /// values for a TIME_SERIES representation. Omitted when zero.
        /// </summary>
        [JsonPropertyName("sampleRate")]
        public double? SampleRate { get; set; }

        /// <summary>
        /// Indicates whether the reported values are discrete events rather
        /// than a continuously varying signal. Omitted when false.
        /// </summary>
        [JsonPropertyName("discrete")]
        public bool? Discrete { get; set; }

        /// <summary>
        /// The data item's <c>representation</c> (for example DATA_SET, TABLE,
        /// or TIME_SERIES). Omitted when the default VALUE representation
        /// applies.
        /// </summary>
        [JsonPropertyName("representation")]
        public string Representation { get; set; }

        /// <summary>
        /// The number of significant digits in the reported values. Omitted
        /// when zero.
        /// </summary>
        [JsonPropertyName("significantDigits")]
        public int? SignificantDigits { get; set; }

        /// <summary>
        /// The source (component, composition, or data item) the data item's
        /// values originate from.
        /// </summary>
        [JsonPropertyName("Source")]
        public JsonSource Source { get; set; }

        /// <summary>
        /// The constraints (limits or enumerated values) bounding the reported
        /// values.
        /// </summary>
        [JsonPropertyName("Constraints")]
        public JsonConstraints Constraints { get; set; }

        /// <summary>
        /// The filters (minimum delta or period) applied to the reported
        /// values.
        /// </summary>
        [JsonPropertyName("Filters")]
        public JsonFilters Filters { get; set; }

        /// <summary>
        /// The value reported for the data item before its first observation.
        /// </summary>
        [JsonPropertyName("initialValue")]
        public string InitialValue { get; set; }

        /// <summary>
        /// The condition under which a resettable data item's accumulated
        /// value is reset.
        /// </summary>
        [JsonPropertyName("resetTrigger")]
        public string ResetTrigger { get; set; }

        /// <summary>
        /// The definition describing the structure of cell or entry values for
        /// a DATA_SET or TABLE representation.
        /// </summary>
        [JsonPropertyName("Definition")]
        public JsonDataItemDefinition Definition { get; set; }

        /// <summary>
        /// The relationships from the data item to other data items and
        /// specifications, grouped by relationship kind.
        /// </summary>
        [JsonPropertyName("Relationships")]
        public JsonRelationshipContainer Relationships { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonDataItem() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IDataItem"/>, omitting properties whose values equal the
        /// MTConnect default and grouping relationships by their concrete
        /// relationship interface.
        /// </summary>
        public JsonDataItem(IDataItem dataItem)
        {
            if (dataItem != null)
            {
                DataItemCategory = dataItem.Category.ToString();
                Id = dataItem.Id;
                if (!string.IsNullOrEmpty(dataItem.Name)) Name = dataItem.Name;
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
                        // Each IAbstractDataItemRelationship flows into the bucket
                        // matching its specialised interface.
                        switch (relationship)
                        {
                            case IDataItemRelationship dataItemRelationship:
                                if (relationships.DataItemRelationships == null) relationships.DataItemRelationships = new List<JsonRelationship>();
                                relationships.DataItemRelationships.Add(new JsonRelationship(dataItemRelationship));
                                break;

                            case ISpecificationRelationship specificationRelationship:
                                if (relationships.SpecificationRelationships == null) relationships.SpecificationRelationships = new List<JsonRelationship>();
                                relationships.SpecificationRelationships.Add(new JsonRelationship(specificationRelationship));
                                break;
                        }
                    }
                    Relationships = relationships;
                }

                if (dataItem.Representation != DataItemRepresentation.VALUE) Representation = dataItem.Representation.ToString();
                if (dataItem.ResetTrigger != null) ResetTrigger = dataItem.ResetTrigger.ToString();
                if (dataItem.CoordinateSystem != DataItemCoordinateSystem.MACHINE) CoordinateSystem = dataItem.CoordinateSystem.ToString();
                CoordinateSystemIdRef = dataItem.CoordinateSystemIdRef;
                if (dataItem.Constraints != null) Constraints = new JsonConstraints(dataItem.Constraints);
                if (dataItem.Definition != null) Definition = new JsonDataItemDefinition(dataItem.Definition);
                Units = dataItem.Units;
                if (dataItem.Statistic != null) Statistic = dataItem.Statistic.ToString();
                if (dataItem.SignificantDigits > 0) SignificantDigits = dataItem.SignificantDigits;

                if (!dataItem.Filters.IsNullOrEmpty())
                {
                    Filters = new JsonFilters(dataItem.Filters);
                }

                InitialValue = dataItem.InitialValue;
                if (dataItem.Discrete != false) Discrete = dataItem.Discrete;
            }
        }


        /// <summary>
        /// Returns the JSON representation of this surrogate.
        /// </summary>
        public override string ToString() => JsonFunctions.Convert(this);

        /// <summary>
        /// Converts this surrogate to a strongly-typed <see cref="DataItem"/>,
        /// instantiating the concrete data item subtype for <see cref="Type"/>,
        /// restoring omitted properties to their MTConnect defaults, and
        /// flattening the grouped relationships back into a single collection.
        /// </summary>
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
            dataItem.NativeScale = NativeScale.HasValue ? NativeScale.Value.ToInt() : 0;
            dataItem.SampleRate = SampleRate.HasValue ? SampleRate.Value : 0;
            if (Source != null) dataItem.Source = Source.ToSource();

            // Relationships
            if (Relationships != null)
            {
                var relationships = new List<IAbstractDataItemRelationship>();

                // DataItemRelationship
                if (!Relationships.DataItemRelationships.IsNullOrEmpty())
                {
                    foreach (var relationship in Relationships.DataItemRelationships)
                    {
                        relationships.Add(relationship.ToDataItemRelationship());
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

            if (!string.IsNullOrEmpty(Representation)) dataItem.Representation = Representation.ConvertEnum<DataItemRepresentation>();
            if (!string.IsNullOrEmpty(ResetTrigger)) dataItem.ResetTrigger = ResetTrigger.ConvertEnum<DataItemResetTrigger>();
            if (!string.IsNullOrEmpty(CoordinateSystem)) dataItem.CoordinateSystem = CoordinateSystem.ConvertEnum<DataItemCoordinateSystem>();
            dataItem.CoordinateSystemIdRef = CoordinateSystemIdRef;
            if (Constraints != null) dataItem.Constraints = Constraints.ToConstraints();
            if (Definition != null) dataItem.Definition = Definition.ToDefinition();
            dataItem.Units = Units;
            if (!string.IsNullOrEmpty(Statistic)) dataItem.Statistic = Statistic.ConvertEnum<DataItemStatistic>();
            dataItem.SignificantDigits = SignificantDigits.HasValue ? SignificantDigits.Value : 0;

            // Filters
            if (Filters != null)
            {
                dataItem.Filters = Filters.ToFilters();
            }

            dataItem.InitialValue = InitialValue;
            if (Discrete.HasValue) dataItem.Discrete = Discrete.Value;

            return dataItem;
        }
    }
}