// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// SysML XMI: https://github.com/mtconnect/mtconnect_sysml_model
//   UML class Axis (UML ID `_2024x_3_3870182_1764951682685_285104_645`).

using MTConnect.Devices.Configurations;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonAxis
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }


        public JsonAxis() { }

        public JsonAxis(IAxis axis)
        {
            if (axis != null)
            {
                Value = axis.Value;
            }
        }


        public IAxis ToAxis()
        {
            var axis = new Axis();
            axis.Value = Value;
            return axis;
        }
    }
}
