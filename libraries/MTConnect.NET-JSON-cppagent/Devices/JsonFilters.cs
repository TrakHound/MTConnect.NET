// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonFilters
    {
        [JsonPropertyName("Filter")]
        public IEnumerable<JsonFilter> Filters { get; set; }


        public JsonFilters() { }

        public JsonFilters(IEnumerable<IFilter> filters)
        {
            if (!filters.IsNullOrEmpty())
            {
                var jsonFilters = new List<JsonFilter>();

                foreach (var filter in filters) jsonFilters.Add(new JsonFilter(filter));

                Filters = jsonFilters;
            }
        }


        public IEnumerable<IFilter> ToFilters()
        {
            var filters = new List<IFilter>();

            if (!filters.IsNullOrEmpty())
            {
                foreach (var filter in Filters) filters.Add(filter.ToFilter());
            }

            return filters;
        }
    }
}