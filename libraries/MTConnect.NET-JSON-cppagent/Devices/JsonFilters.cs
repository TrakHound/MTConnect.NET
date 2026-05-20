// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for a typed container of
    /// <see cref="JsonFilter"/> items, keyed by the singular cppagent
    /// element name <c>Filter</c>.
    /// </summary>
    public class JsonFilters
    {
        /// <summary>
        /// The data-item filters in the container.
        /// </summary>
        [JsonPropertyName("Filter")]
        public IEnumerable<JsonFilter> Filters { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonFilters() { }

        /// <summary>
        /// Initializes the container from a filter sequence.
        /// </summary>
        public JsonFilters(IEnumerable<IFilter> filters)
        {
            if (!filters.IsNullOrEmpty())
            {
                var jsonFilters = new List<JsonFilter>();

                foreach (var filter in filters) jsonFilters.Add(new JsonFilter(filter));

                Filters = jsonFilters;
            }
        }


        /// <summary>
        /// Flattens the container back into a uniform
        /// <see cref="IFilter"/> sequence.
        /// </summary>
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