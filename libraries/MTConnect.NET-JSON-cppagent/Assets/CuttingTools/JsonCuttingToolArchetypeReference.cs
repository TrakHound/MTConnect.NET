// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.CuttingTools
{
    /// <summary>
    /// JSON serialization surrogate for a
    /// <c>CuttingToolArchetypeReference</c> on a CuttingTool asset,
    /// pointing at the archetype this physical tool was instantiated
    /// from.
    /// </summary>
    public class JsonCuttingToolArchetypeReference
    {
        /// <summary>
        /// The URL or identifier of the source archetype document.
        /// </summary>
        [JsonPropertyName("source")]
        public string Source { get; set; }

        /// <summary>
        /// The reference value (typically the archetype asset id).
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