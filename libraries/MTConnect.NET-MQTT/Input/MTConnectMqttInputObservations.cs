// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Input;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Mqtt
{
    /// <summary>
    /// MQTT-side container that groups a batch of <see cref="MTConnectMqttInputObservation"/>
    /// values under a single timestamp. Downstream agents call <see cref="ToObservationInputs()"/>
    /// to flatten the batch into the internal <see cref="IObservationInput"/> form and feed it
    /// to the observation buffer.
    /// </summary>
    public class MTConnectMqttInputObservations
    {
        /// <summary>Wall-clock timestamp applied to every observation in <see cref="Observations"/> on conversion.</summary>
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        /// <summary>The observations carried by this batch; each is converted to an <see cref="IObservationInput"/> with <see cref="Timestamp"/> applied.</summary>
        [JsonPropertyName("observations")]
        public List<MTConnectMqttInputObservation> Observations { get; set; } = new List<MTConnectMqttInputObservation>();


        /// <summary>
        /// Flattens the batch into a list of <see cref="IObservationInput"/> instances, one per
        /// member of <see cref="Observations"/>, with <see cref="Timestamp"/> applied (in Unix
        /// time) to each. Observations missing a DataItem key or values are silently skipped.
        /// </summary>
        public IEnumerable<IObservationInput> ToObservationInputs()
        {
            var observations = new List<IObservationInput>();
            observations.AddRange(ToObservationInputs(Timestamp, Observations));
            return observations;
        }

        private static IEnumerable<IObservationInput> ToObservationInputs(DateTime timestamp, List<MTConnectMqttInputObservation> mqttObservations)
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
