// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// SysML XMI: https://github.com/mtconnect/mtconnect_sysml_model
//   UML class TranslationDataSet (UML ID `_2024x_68e0225_1727807350445_154414_23573`).

using MTConnect.Devices.Configurations;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonTranslationDataSet
    {
        [JsonPropertyName("x")]
        public string X { get; set; }

        [JsonPropertyName("y")]
        public string Y { get; set; }

        [JsonPropertyName("z")]
        public string Z { get; set; }


        public JsonTranslationDataSet() { }

        public JsonTranslationDataSet(ITranslationDataSet dataSet)
        {
            if (dataSet != null)
            {
                X = dataSet.X;
                Y = dataSet.Y;
                Z = dataSet.Z;
            }
        }


        public ITranslationDataSet ToTranslationDataSet()
        {
            var dataSet = new TranslationDataSet();
            dataSet.X = X;
            dataSet.Y = Y;
            dataSet.Z = Z;
            return dataSet;
        }
    }
}
