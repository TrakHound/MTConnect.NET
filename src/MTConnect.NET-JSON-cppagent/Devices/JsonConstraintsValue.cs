// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonConstraintsValue
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }


        public JsonConstraintsValue() { }

        public JsonConstraintsValue(string value)
        {
            Value = value;
        }


        public string ToValue()
        {
            return Value;
        }
    }
}