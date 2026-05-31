// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace MTConnect.Mqtt
{
    /// <summary>
    /// JSON serialization surrogate for a group of observation inputs sharing
    /// a single timestamp. Used to batch observations recorded at the same
    /// instant in a single JSON payload over transports such as MQTT.
    /// </summary>
    public class JsonInputObservationGroup
    {
        /// <summary>
        /// The timestamp at which the observations in the group were
        /// recorded.
        /// </summary>
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The observations sharing <see cref="Timestamp"/>.
        /// </summary>
        [JsonPropertyName("observations")]
        public List<JsonInputObservation> Observations { get; set; } = new List<JsonInputObservation>();


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonInputObservationGroup() { }

        /// <summary>
        /// Initializes the surrogate as a single-observation group at the
        /// observation's timestamp.
        /// </summary>
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

        /// <summary>
        /// Partitions <paramref name="observations"/> by distinct timestamp
        /// and returns one group per timestamp, or null when the input is
        /// empty.
        /// </summary>
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


        /// <summary>
        /// Converts the observations in this group to strongly-typed
        /// observation inputs, applying <see cref="Timestamp"/> to each.
        /// </summary>
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
