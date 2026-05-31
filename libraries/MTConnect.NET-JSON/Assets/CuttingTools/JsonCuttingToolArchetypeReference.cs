// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.CuttingTools
{
    /// <summary>
    /// JSON serialization surrogate for a <c>CuttingToolArchetypeReference</c>,
    /// a pointer from a cutting tool asset to the archetype it is an instance
    /// of. Converts to and from the strongly-typed
    /// <see cref="CuttingToolArchetypeReference"/> model.
    /// </summary>
    public class JsonCuttingToolArchetypeReference
    {
        /// <summary>
        /// The source of the archetype reference.
        /// </summary>
        [JsonPropertyName("source")]
        public string Source { get; set; }

        /// <summary>
        /// The archetype identifier.
        /// </summary>
        [JsonPropertyName("value")]
        public string Value { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonCuttingToolArchetypeReference() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="ICuttingToolArchetypeReference"/>.
        /// </summary>
        public JsonCuttingToolArchetypeReference(ICuttingToolArchetypeReference cuttingToolArchetypeReference)
        {
            if (cuttingToolArchetypeReference != null)
            {
                Source = cuttingToolArchetypeReference.Source;
                Value = cuttingToolArchetypeReference.Value;
            }
        }


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="ICuttingToolArchetypeReference"/>.
        /// </summary>
        public ICuttingToolArchetypeReference ToCuttingToolArchetypeReference()
        {
            var location = new CuttingToolArchetypeReference();
            location.Source = Source;
            location.Value = Value;
            return location;
        }
    }
}