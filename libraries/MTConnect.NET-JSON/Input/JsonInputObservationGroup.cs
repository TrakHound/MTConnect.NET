// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace MTConnect.Mqtt
{
    public class JsonInputObservationGroup
    {
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonPropertyName("observations")]
        public List<JsonInputObservation> Observations { get; set; } = new List<JsonInputObservation>();


        public JsonInputObservationGroup() { }

        public JsonInputObservationGroup(IObservationInput observation) 
        {
            if (observation != null)
            {
                Timestamp = observation.Timestamp.ToDateTime();

                var jsonObservations = new List<JsonInputObservation>();
                jsonObservations.Add(new JsonInputObservation(observation));
                Observations = jsonObservations;
            }
        }

        public static IEnumerable<JsonInputObservationGroup> Create(IEnumerable<IObservationInput> observations)
        {
            if (!observations.IsNullOrEmpty())
            {
                var jsonObservationGroups = new List<JsonInputObservationGroup>();

                var timestamps = observations.Select(o => o.Timestamp).Distinct();
                foreach (var timestamp in timestamps)
                {
                    var jsonObservationGroup = new JsonInputObservationGroup();
                    jsonObservationGroup.Timestamp = timestamp.ToDateTime();

                    var jsonObservations = new List<JsonInputObservation>();
                    var timestampObservations = observations.Where(o => o.Timestamp == timestamp);
                    foreach (var observation in timestampObservations)
                    {
                        jsonObservations.Add(new JsonInputObservation(observation));
                    }
                    jsonObservationGroup.Observations = jsonObservations;

                    jsonObservationGroups.Add(jsonObservationGroup);
                }

                return jsonObservationGroups;
            }

            return null;
        }


        public IEnumerable<IObservationInput> ToObservationInputs()
        {
            var observations = new List<IObservationInput>();
            observations.AddRange(ToObservationInputs(Timestamp, Observations));
            return observations;
        }

        private static IEnumerable<IObservationInput> ToObservationInputs(DateTime timestamp, List<JsonInputObservation> mqttObservations)
        {
            var observations = new List<IObservationInput>();

            if (!mqttObservations.IsNullOrEmpty())
            {
                var ts = timestamp.ToUnixTime();

                foreach (var mqttObservation in mqttObservations)
                {
                    if (!string.IsNullOrEmpty(mqttObservation.DataItem) && !mqttObservation.Values.IsNullOrEmpty())
                    {
                        var observation = new ObservationInput();
                        observation.DataItemKey = mqttObservation.DataItem;
                        observation.Timestamp = ts;

                        foreach (var value in mqttObservation.Values)
                        {
                            if (!string.IsNullOrEmpty(value.Key))
                            {
                                observation.AddValue(value.Key, value.Value);
                            }
                        }

                        observations.Add(observation);
                    }
                }
            }

            return observations;
        }
    }
}
