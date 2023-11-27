// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.CuttingTools
{
    public class JsonReconditionCount
    {
        [JsonPropertyName("maximumCount")]
        public int? MaximumCount { get; set; }

        [JsonPropertyName("value")]
        public int? Value { get; set; }


        public JsonReconditionCount() { }

        public JsonReconditionCount(IReconditionCount reconditionCount)
        {
            if (reconditionCount != null)
            {
                MaximumCount = reconditionCount.MaximumCount;
                Value = reconditionCount.Value;
            }
        }


        public IReconditionCount ToReconditionCount()
        {
            var reconditionCount = new ReconditionCount();
            reconditionCount.MaximumCount = MaximumCount;
            reconditionCount.Value = Value;
            return reconditionCount;
        }
    }
}