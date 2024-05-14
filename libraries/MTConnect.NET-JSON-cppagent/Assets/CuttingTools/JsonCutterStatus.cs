// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.CuttingTools
{
    public class JsonCutterStatus
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }


        public JsonCutterStatus() { }

        public JsonCutterStatus(CutterStatusType cutterStatus)
        {
            Value = cutterStatus.ToString();
        }


        public CutterStatusType ToCutterStatus()
        {
            return Value.ConvertEnum<CutterStatusType>();
        }
    }
}