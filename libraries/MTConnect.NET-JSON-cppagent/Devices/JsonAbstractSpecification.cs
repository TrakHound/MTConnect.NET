// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonAbstractSpecification
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("subType")]
        public string SubType { get; set; }

        [JsonPropertyName("dataItemIdRef")]
        public string DataItemIdRef { get; set; }

        [JsonPropertyName("units")]
        public string Units { get; set; }

        [JsonPropertyName("compositionIdRef")]
        public string CompositionIdRef { get; set; }

        [JsonPropertyName("coordinateSystemIdRef")]
        public string CoordinateSystemIdRef { get; set; }

        [JsonPropertyName("originator")]
        public string Originator { get; set; }


        public JsonAbstractSpecification() { }

        public JsonAbstractSpecification(ISpecification specification)
        {
            if (specification != null)
            {
                Id = specification.Id;
                Name = specification.Name;
                Type = specification.Type;
                SubType = specification.SubType;
                DataItemIdRef = specification.DataItemIdRef;
                Units = specification.Units;
                CompositionIdRef = specification.CompositionIdRef;
                CoordinateSystemIdRef = specification.CoordinateSystemIdRef;
                if (specification.Originator != Configurations.Originator.MANUFACTURER) Originator = specification.Originator.ToString();
            }
        }


        public virtual ISpecification ToSpecification()
        {
            var specification = new Specification();
            specification.Id = Id;
            specification.Name = Name;
            specification.Type = Type;
            specification.SubType = SubType;
            specification.DataItemIdRef = DataItemIdRef;
            specification.Units = Units;
            specification.CompositionIdRef = CompositionIdRef;
            specification.CoordinateSystemIdRef = CoordinateSystemIdRef;
            specification.Originator = Originator.ConvertEnum<Originator>();
            return specification;
        }
    }
}