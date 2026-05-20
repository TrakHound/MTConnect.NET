// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.CuttingTools
{
    /// <summary>
    /// JSON serialization surrogate for a <c>CuttingToolDefinition</c>, which
    /// declares the format of a cutting tool's payload. Converts to and from
    /// the strongly-typed <see cref="CuttingToolDefinition"/> model.
    /// </summary>
    public class JsonCuttingToolDefinition
    {
        /// <summary>
        /// The format of the cutting tool definition, serialized as the
        /// enumeration name.
        /// </summary>
        [JsonPropertyName("format")]
        public string Format { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonCuttingToolDefinition() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="ICuttingToolDefinition"/>.
        /// </summary>
        public JsonCuttingToolDefinition(ICuttingToolDefinition cuttingToolDefinition)
        {
            if (cuttingToolDefinition != null)
            {
                Format = cuttingToolDefinition.Format.ToString();
            }
        }


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="CuttingToolDefinition"/>, parsing the format enumeration.
        /// </summary>
        public CuttingToolDefinition ToCuttingToolDefinition()
        {
            var cuttingToolDefinition = new CuttingToolDefinition();
            cuttingToolDefinition.Format = Format.ConvertEnum<FormatType>();
            return cuttingToolDefinition;
        }
    }
}