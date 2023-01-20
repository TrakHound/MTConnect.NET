// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Text.Json.Serialization;

namespace MTConnect.Observations
{
    public class JsonCell
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }


        public JsonCell() { }

        public JsonCell(ITableCell cell)
        {
            if (cell != null)
            {
                Key = cell.Key;
                Value = cell.Value;
            }
        }
    }
}
