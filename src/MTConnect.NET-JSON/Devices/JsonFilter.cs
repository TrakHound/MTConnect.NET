// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.DataItems;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonFilter
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("value")]
        public double Value { get; set; }


        public JsonFilter() { }

        public JsonFilter(IFilter filter)
        {
            if (filter != null)
            {
                Type = filter.Type.ToString();
                Value = filter.Value;
            }
        }


        public IFilter ToFilter()
        {
            var filter = new Filter();
            filter.Type = Type.ConvertEnum<DataItemFilterType>();
            filter.Value = Value;
            return filter;
        }
    }
}