// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for the shape common to all Configuration
    /// <c>Specification</c> kinds. Mirrors the on-the-wire shape so the JSON
    /// serializer can read and write it, then converts to and from the
    /// strongly-typed <see cref="Specification"/> model;
    /// <see cref="JsonSpecification"/> extends it with the limit elements.
    /// </summary>
    public class JsonAbstractSpecification
    {
        /// <summary>
        /// The unique <c>id</c> of the specification.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The human-readable name of the specification.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The MTConnect type of the value the specification constrains.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// The subtype further qualifying <see cref="Type"/>.
        /// </summary>
        [JsonPropertyName("subType")]
        public string SubType { get; set; }

        /// <summary>
        /// Reference to the <c>id</c> of the data item the specification
        /// applies to.
        /// </summary>
        [JsonPropertyName("dataItemIdRef")]
        public string DataItemIdRef { get; set; }

        /// <summary>
        /// The engineering units the specification limits are expressed in.
        /// </summary>
        [JsonPropertyName("units")]
        public string Units { get; set; }

        /// <summary>
        /// Reference to the <c>id</c> of the composition the specification
        /// applies to.
        /// </summary>
        [JsonPropertyName("compositionIdRef")]
        public string CompositionIdRef { get; set; }

        /// <summary>
        /// Reference to the <c>id</c> of the coordinate system the
        /// specification limits are expressed in.
        /// </summary>
        [JsonPropertyName("coordinateSystemIdRef")]
        public string CoordinateSystemIdRef { get; set; }

        /// <summary>
        /// The party that defined the specification (MANUFACTURER or USER).
        /// </summary>
        [JsonPropertyName("originator")]
        public string Originator { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonAbstractSpecification() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="ISpecification"/>, converting the originator enumeration
        /// to a string.
        /// </summary>
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
                Originator = specification.Originator.ToString();
            }
        }


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="ISpecification"/>, parsing the originator enumeration.
        /// </summary>
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