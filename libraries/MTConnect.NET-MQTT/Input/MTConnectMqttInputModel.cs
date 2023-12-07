// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Input;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Mqtt
{
    public class MTConnectMqttInputModel
    {
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonPropertyName("observations")]
        public List<MTConnectMqttInputObservation> Observations { get; set; } = new List<MTConnectMqttInputObservation>();


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
                    if (!string.IsNullOrEmpty(mqttObservation.DataItemKey) && !mqttObservation.Values.IsNullOrEmpty())
                    {
                        var observation = new ObservationInput();
                        observation.DataItemKey = mqttObservation.DataItemKey;
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
