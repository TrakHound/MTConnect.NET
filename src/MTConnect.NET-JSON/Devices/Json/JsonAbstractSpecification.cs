// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Devices.Configurations.Specifications;
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

        [JsonPropertyName("coordinateIdRef")]
        public string CoordinateIdRef { get; set; }

        [JsonPropertyName("originator")]
        public Originator Originator { get; set; }


        public JsonAbstractSpecification() { }

        public JsonAbstractSpecification(IAbstractSpecification specification)
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
                CoordinateIdRef = specification.CoordinateIdRef;
                Originator = specification.Originator;
            }
        }


        public virtual IAbstractSpecification ToSpecification()
        {
            var specification = new Specification();
            specification.Id = Id;
            specification.Name = Name;
            specification.Type = Type;
            specification.SubType = SubType;
            specification.DataItemIdRef = DataItemIdRef;
            specification.Units = Units;
            specification.CompositionIdRef = CompositionIdRef;
            specification.CoordinateIdRef = CoordinateIdRef;
            specification.Originator = Originator;
            return specification;
        }
    }
}
