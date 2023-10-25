// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.CuttingTools
{
    public class CuttingToolArchetype : CuttingToolAsset
    {
        [XmlElement("CuttingToolDefinition")]
        [JsonPropertyName("cuttingToolDefinition")]
        public CuttingToolDefinition CuttingToolDefinition { get; set; }


        public CuttingToolArchetype()
        {
            Type = "CuttingToolArchetype";
            CuttingToolLifeCycle = new CuttingToolLifeCycle();
        }
    }
}