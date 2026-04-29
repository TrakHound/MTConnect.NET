// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// SysML XMI: https://github.com/mtconnect/mtconnect_sysml_model
//   UML class RotationDataSet (UML ID `_2024x_68e0225_1727807509860_595526_23700`).

// JSON shape: flat object with PascalCase keys ({"A": v, "B": v, "C": v}) per the
// cppagent v2 DataSet convention.

using MTConnect.Devices.Configurations;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonRotationDataSet
    {
        [JsonPropertyName("A")]
        public string A { get; set; }

        [JsonPropertyName("B")]
        public string B { get; set; }

        [JsonPropertyName("C")]
        public string C { get; set; }


        public JsonRotationDataSet() { }

        public JsonRotationDataSet(IRotationDataSet dataSet)
        {
            if (dataSet != null)
            {
                A = dataSet.A;
                B = dataSet.B;
                C = dataSet.C;
            }
        }


        public IRotationDataSet ToRotationDataSet()
        {
            var dataSet = new RotationDataSet();
            dataSet.A = A;
            dataSet.B = B;
            dataSet.C = C;
            return dataSet;
        }
    }
}
