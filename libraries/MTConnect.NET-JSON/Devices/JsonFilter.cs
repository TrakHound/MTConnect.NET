// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// JSON serialization surrogate for a data item <c>Filter</c>, which
    /// limits how often a data item produces observations. Converts to and
    /// from the strongly-typed <see cref="Filter"/> model.
    /// </summary>
    public class JsonFilter
    {
        /// <summary>
        /// The filter type (MINIMUM_DELTA or PERIOD), serialized as the
        /// enumeration name.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// The filter threshold value.
        /// </summary>
        [JsonPropertyName("value")]
        public double Value { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonFilter() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed <see cref="IFilter"/>.
        /// </summary>
        public JsonFilter(IFilter filter)
        {
            if (filter != null)
            {
                Type = filter.Type.ToString();
                Value = filter.Value;
            }
        }


        /// <summary>
        /// Converts this surrogate to a strongly-typed <see cref="IFilter"/>,
        /// parsing the filter type enumeration.
        /// </summary>
        public IFilter ToFilter()
        {
            var filter = new Filter();
            filter.Type = Type.ConvertEnum<DataItemFilterType>();
            filter.Value = Value;
            return filter;
        }
    }
}