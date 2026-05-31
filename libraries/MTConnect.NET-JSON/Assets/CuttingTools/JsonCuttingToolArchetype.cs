// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.CuttingTools
{
    /// <summary>
    /// JSON serialization surrogate for a <c>CuttingToolArchetype</c> asset,
    /// the template cutting tool definition that cutting tool instances refer
    /// to. Converts to and from the strongly-typed
    /// <see cref="CuttingToolArchetypeAsset"/> model.
    /// </summary>
    public class JsonCuttingToolArchetype
    {
        /// <summary>
        /// The cutting tool definition declared by the archetype.
        /// </summary>
        [JsonPropertyName("cuttingToolDefinition")]
        public JsonCuttingToolDefinition CuttingToolDefinition { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonCuttingToolArchetype() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="ICuttingToolArchetypeAsset"/>.
        /// </summary>
        public JsonCuttingToolArchetype(ICuttingToolArchetypeAsset cuttingToolArchetype)
        {
            if (cuttingToolArchetype != null)
            {
                CuttingToolDefinition = new JsonCuttingToolDefinition(cuttingToolArchetype.CuttingToolDefinition);
            }
        }


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="ICuttingToolArchetypeAsset"/>.
        /// </summary>
        public ICuttingToolArchetypeAsset ToCuttingToolArchetype()
        {
            var cuttingToolArchetype = new CuttingToolArchetypeAsset();
            if (CuttingToolDefinition != null) cuttingToolArchetype.CuttingToolDefinition = CuttingToolDefinition.ToCuttingToolDefinition();
            return cuttingToolArchetype;
        }
    }
}