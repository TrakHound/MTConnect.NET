// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.CuttingTools
{
    /// <summary>
    /// JSON serialization surrogate for a CuttingTool
    /// <c>CuttingToolDefinition</c>, identifying the format the
    /// definition document is expressed in (for example QIF, EXPRESS).
    /// </summary>
    public class JsonCuttingToolDefinition
    {
        /// <summary>
        /// The format identifier of the definition document.
        /// </summary>
        [JsonPropertyName("format")]
        public string Format { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonCuttingToolDefinition() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="ICuttingToolDefinition"/>, serializing the format
        /// to its enumeration name.
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
        /// <see cref="CuttingToolDefinition"/>, parsing the format
        /// enumeration from its serialized form.
        /// </summary>
        public CuttingToolDefinition ToCuttingToolDefinition()
        {
            var cuttingToolDefinition = new CuttingToolDefinition();
            cuttingToolDefinition.Format = Format.ConvertEnum<FormatType>();
            return cuttingToolDefinition;
        }
    }
}