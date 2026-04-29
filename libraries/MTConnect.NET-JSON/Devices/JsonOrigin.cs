// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// SysML XMI: https://github.com/mtconnect/mtconnect_sysml_model
//   UML class Origin (UML ID `_19_0_3_45f01b9_1579107743274_159386_163610`).

using MTConnect.Devices.Configurations;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonOrigin
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }


        public JsonOrigin() { }

        public JsonOrigin(IOrigin origin)
        {
            if (origin != null)
            {
                Value = origin.Value;
            }
        }


        public IOrigin ToOrigin()
        {
            var origin = new Origin();
            origin.Value = Value;
            return origin;
        }
    }
}
