// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

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
