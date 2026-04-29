// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// SysML XMI: https://github.com/mtconnect/mtconnect_sysml_model
//   UML class Scale (UML ID `_2024x_3_3870182_1764951924679_898861_876`).

using MTConnect.Devices.Configurations;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonScale
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }


        public JsonScale() { }

        public JsonScale(IScale scale)
        {
            if (scale != null)
            {
                Value = scale.Value;
            }
        }


        public IScale ToScale()
        {
            var scale = new Scale();
            scale.Value = Value;
            return scale;
        }
    }
}
