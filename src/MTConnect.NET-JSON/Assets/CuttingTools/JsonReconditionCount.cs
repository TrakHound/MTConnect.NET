// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace MTConnect.Assets.CuttingTools
{
    public class JsonReconditionCount
    {
        [JsonPropertyName("maximumCount")]
        public int MaximumCount { get; set; }

        [JsonPropertyName("value")]
        public int Value { get; set; }


        public JsonReconditionCount() { }

        public JsonReconditionCount(ReconditionCount reconditionCount)
        {
            if (reconditionCount != null)
            {
                MaximumCount = reconditionCount.MaximumCount;
                Value = reconditionCount.Value;
            }
        }


        public ReconditionCount ToReconditionCount()
        {
            var reconditionCount = new ReconditionCount();
            reconditionCount.MaximumCount = MaximumCount;
            reconditionCount.Value = Value;
            return reconditionCount;
        }
    }
}