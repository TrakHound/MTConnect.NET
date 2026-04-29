// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// SysML XMI: https://github.com/mtconnect/mtconnect_sysml_model
//   UML class RotationDataSet (UML ID `_2024x_68e0225_1727807509860_595526_23700`).

using MTConnect.Devices.Configurations;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonRotationDataSet
    {
        [JsonPropertyName("a")]
        public string A { get; set; }

        [JsonPropertyName("b")]
        public string B { get; set; }

        [JsonPropertyName("c")]
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
