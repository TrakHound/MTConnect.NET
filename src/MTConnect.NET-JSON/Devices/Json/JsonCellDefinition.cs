// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Devices.DataItems;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonCellDefinition
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("keyType")]
        public string KeyType { get; set; }

        [JsonPropertyName("units")]
        public string Units { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("subType")]
        public string SubType { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }


        public JsonCellDefinition() { }

        public JsonCellDefinition(ICellDefinition definition)
        {
            if (definition != null)
            {
                Key = definition.Key;
                KeyType = definition.KeyType;
                Units = definition.Units;
                Type = definition.Type;
                SubType = definition.SubType;
                Description = definition.Description;
            }
        }


        public ICellDefinition ToCellDefinition()
        {
            var definition = new CellDefinition();
            definition.Key = Key;
            definition.KeyType = KeyType;
            definition.Units = Units;
            definition.Type = Type;
            definition.SubType = SubType;
            definition.Description = Description;
            return definition;
        }
    }
}
