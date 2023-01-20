// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

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
