// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// Shared JSON serialization surrogate for the common attributes of any
    /// <c>Specification</c> in the cppagent-compatible shape. Carries the
    /// identifying attributes and the optional originator override, omitting
    /// the originator field when it equals the implicit default of
    /// <c>MANUFACTURER</c> to keep the wire shape minimal.
    /// </summary>
    public class JsonAbstractSpecification
    {
        /// <summary>
        /// The unique <c>id</c> of the specification.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The descriptive name of the specification.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The type of the specification (semantic category).
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// Optional sub-type qualifier of the specification.
        /// </summary>
        [JsonPropertyName("subType")]
        public string SubType { get; set; }

        /// <summary>
        /// Reference to the <c>id</c> of the data item this specification
        /// constrains.
        /// </summary>
        [JsonPropertyName("dataItemIdRef")]
        public string DataItemIdRef { get; set; }

        /// <summary>
        /// Engineering units of the specification limits.
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
        /// The party that authored the specification (only emitted when it
        /// differs from the implicit <c>MANUFACTURER</c> default).
        /// </summary>
        [JsonPropertyName("originator")]
        public string Originator { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonAbstractSpecification() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="ISpecification"/>, suppressing the originator when it
        /// equals the implicit default of <c>MANUFACTURER</c>.
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
                if (specification.Originator != Configurations.Originator.MANUFACTURER) Originator = specification.Originator.ToString();
            }
        }


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="ISpecification"/>, parsing the originator enumeration
        /// from its serialized form.
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