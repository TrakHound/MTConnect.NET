// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// SysML XMI: https://github.com/mtconnect/mtconnect_sysml_model
//   UML class Rotation (UML ID `_2024x_3_3870182_1764951373391_145162_327`).

using MTConnect.Devices.Configurations;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonRotation
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }


        public JsonRotation() { }

        public JsonRotation(IRotation rotation)
        {
            if (rotation != null)
            {
                Value = rotation.Value;
            }
        }


        public IRotation ToRotation()
        {
            var rotation = new Rotation();
            rotation.Value = Value;
            return rotation;
        }
    }
}
