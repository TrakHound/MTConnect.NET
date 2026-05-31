// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.CuttingTools
{
    /// <summary>
    /// JSON serialization surrogate for the
    /// <c>CuttingToolArchetype</c> asset wrapper, carrying the
    /// archetype's external <see cref="CuttingToolDefinition"/>
    /// reference. Converts to and from the strongly-typed
    /// <see cref="CuttingToolArchetypeAsset"/>.
    /// </summary>
    public class JsonCuttingToolArchetype
    {
        /// <summary>
        /// The cutting-tool definition reference.
        /// </summary>
        [JsonPropertyName("CuttingToolDefinition")]
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