// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.CuttingTools
{
    public class CuttingToolDefinition
    {
        /// <summary>
        /// Identifies the expected representation of the enclosed data.
        /// </summary>
        [XmlAttribute("format")]
        [JsonPropertyName("format")]
        public CuttingToolDefinitionFormat Format { get; set; }
    }
}