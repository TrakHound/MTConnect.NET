// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Input;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Mqtt
{
    public class JsonInputObservation
    {
        [JsonPropertyName("dataItem")]
        public string DataItem { get; set; }

        [JsonPropertyName("values")]
        public Dictionary<string, string> Values { get; set; }


        public JsonInputObservation() { }

        public JsonInputObservation(string dataItemKey, Dictionary<string, string> values)
        {
            DataItem = dataItemKey;
            Values = values;
        }

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
