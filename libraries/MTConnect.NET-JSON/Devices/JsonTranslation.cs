// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// SysML XMI: https://github.com/mtconnect/mtconnect_sysml_model
//   UML class Translation (UML ID `_2024x_3_3870182_1764951167326_754957_161`).

using MTConnect.Devices.Configurations;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonTranslation
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }


        public JsonTranslation() { }

        public JsonTranslation(ITranslation translation)
        {
            if (translation != null)
            {
                Value = translation.Value;
            }
        }


        public ITranslation ToTranslation()
        {
            var translation = new Translation();
            translation.Value = Value;
            return translation;
        }
    }
}
