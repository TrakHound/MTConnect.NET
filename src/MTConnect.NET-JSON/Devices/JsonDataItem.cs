// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonDataItem
    {
        [JsonPropertyName("category")]
        public string DataItemCategory { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("type")]
         public string Type { get; set; }

        [JsonPropertyName("coordinateSystem")]
        public string CoordinateSystem { get; set; }

        [JsonPropertyName("coordinateSystemIdRef")]
        public string CoordinateSystemIdRef { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("nativeScale")]
        public int? NativeScale { get; set; }

        [JsonPropertyName("nativeUnits")]
        public string NativeUnits { get; set; }

        [JsonPropertyName("subType")]
        public string SubType { get; set; }

        [JsonPropertyName("statistic")]
        public string Statistic { get; set; }

        [JsonPropertyName("units")]
        public string Units { get; set; }

        [JsonPropertyName("sampleRate")]
        public double? SampleRate { get; set; }

        [JsonPropertyName("discrete")]
        public bool? Discrete { get; set; }

        [JsonPropertyName("representation")]
        public string Representation { get; set; }

        [JsonPropertyName("significantDigits")]
        public int? SignificantDigits { get; set; }

        [JsonPropertyName("source")]
        public JsonSource Source { get; set; }

        [JsonPropertyName("constraints")]
        public JsonConstraints Constraints { get; set; }

        [JsonPropertyName("filters")]
        public IEnumerable<JsonFilter> Filters { get; set; }

        [JsonPropertyName("initialValue")]
        public string InitialValue { get; set; }

        [JsonPropertyName("resetTrigger")]
        public string ResetTrigger { get; set; }

        [JsonPropertyName("definition")]
        public JsonDataItemDefinition Definition { get; set; }

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
                var relationships = new List<IAbstractDataItemRelationship>();

                //// ComponentRelationship
                //if (!Relationships.ComponentRelationships.IsNullOrEmpty())
                //{
                //    foreach (var relationship in Relationships.ComponentRelationships)
                //    {
                //        relationships.Add(relationship.ToComponentRelationship());
                //    }
                //}

                // DataItemRelationship
                if (!Relationships.DataItemRelationships.IsNullOrEmpty())
                {
                    foreach (var relationship in Relationships.DataItemRelationships)
                    {
                        relationships.Add(relationship.ToDataItemRelationship());
                    }
                }

                //// DeviceRelationship
                //if (!Relationships.DeviceRelationships.IsNullOrEmpty())
                //{
                //    foreach (var relationship in Relationships.DeviceRelationships)
                //    {
                //        relationships.Add(relationship.ToDeviceRelationship());
                //    }
                //}

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
            dataItem.CoordinateSystemIdRef = CoordinateSystemIdRef;
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