// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// SysML XMI: https://github.com/mtconnect/mtconnect_sysml_model
//   UML class OriginDataSet (UML ID `_2024x_68e0225_1727808122960_76782_23993`).

// JSON shape: flat object with PascalCase keys ({"X": v, "Y": v, "Z": v}) per the
// cppagent v2 DataSet convention.

using MTConnect.Devices.Configurations;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonOriginDataSet
    {
        [JsonPropertyName("X")]
        public string X { get; set; }

        [JsonPropertyName("Y")]
        public string Y { get; set; }

        [JsonPropertyName("Z")]
        public string Z { get; set; }


        public JsonOriginDataSet() { }

        public JsonOriginDataSet(IOriginDataSet dataSet)
        {
            if (dataSet != null)
            {
                X = dataSet.X;
                Y = dataSet.Y;
                Z = dataSet.Z;
            }
        }


        public IOriginDataSet ToOriginDataSet()
        {
            var dataSet = new OriginDataSet();
            dataSet.X = X;
            dataSet.Y = Y;
            dataSet.Z = Z;
            return dataSet;
        }
    }
}
