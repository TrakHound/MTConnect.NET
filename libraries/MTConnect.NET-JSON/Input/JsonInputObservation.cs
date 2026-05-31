// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Input;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Mqtt
{
    /// <summary>
    /// JSON serialization surrogate for a single observation input within a
    /// <see cref="JsonInputObservationGroup"/>. Carries the data item key and
    /// its observation values as a string dictionary, allowing all
    /// representations to share a uniform on-the-wire shape.
    /// </summary>
    public class JsonInputObservation
    {
        /// <summary>
        /// The data item key (id or name) the observation reports against.
        /// </summary>
        [JsonPropertyName("dataItem")]
        public string DataItem { get; set; }

        /// <summary>
        /// The observation values keyed by MTConnect value key (for example
        /// Result, ResetTriggered, Level).
        /// </summary>
        [JsonPropertyName("values")]
        public Dictionary<string, string> Values { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonInputObservation() { }

        /// <summary>
        /// Initializes the surrogate directly from a data item key and value
        /// dictionary.
        /// </summary>
        public JsonInputObservation(string dataItemKey, Dictionary<string, string> values)
        {
            DataItem = dataItemKey;
            Values = values;
        }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IObservationInput"/>, copying its non-empty value entries
        /// into <see cref="Values"/>.
        /// </summary>
        public JsonInputObservation(IObservationInput observation)
        {
            if (observation != null && !observation.Values.IsNullOrEmpty())
            {
                DataItem = observation.DataItemKey;

                var values = new Dictionary<string, string>();
                foreach (var value in observation.Values)
                {
                    if (!string.IsNullOrEmpty(value.Key) && !string.IsNullOrEmpty(value.Value))
                    {
                        values.Remove(value.Key);
                        values.Add(value.Key, value.Value);
                    }
                }
                Values = values;
            }
        }
    }
}
