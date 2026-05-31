// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;
using MTConnect.Input;
using MTConnect.Observations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Agents
{
    /// <summary>
    /// A mutable view of an observation as it passes through the Agent processor chain, pairing the raw observation values with the resolved Agent and DataItem context and exposing helpers to add, read, or clear individual values.
    /// </summary>
    public class ProcessObservation
    {
        /// <summary>
        /// The Agent that received the observation.
        /// </summary>
        public IMTConnectAgent Agent { get; set; }

        /// <summary>
        /// The DataItem the observation was reported for, including its owning Device.
        /// </summary>
        public IDataItem DataItem { get; set; }

        /// <summary>
        /// The current set of observation values; processors may replace this collection to transform the observation.
        /// </summary>
        public IEnumerable<ObservationValue> Values { get; set; }

        /// <summary>
        /// The time the observation occurred.
        /// </summary>
        public DateTime Timestamp { get; set; }


        /// <summary>
        /// Initializes a new instance from a received observation input and its resolved context.
        /// </summary>
        /// <param name="agent">The Agent that received the observation.</param>
        /// <param name="dataItem">The DataItem the observation was reported for.</param>
        /// <param name="observation">The received observation input whose values and timestamp seed this instance.</param>
        public ProcessObservation(IMTConnectAgent agent, IDataItem dataItem, IObservationInput observation)
        {
            Agent = agent;
            DataItem = dataItem;
            Values = observation.Values;
            Timestamp = observation.Timestamp.ToDateTime();
        }


        /// <summary>
        /// Add or replace the value with the given key.
        /// </summary>
        /// <param name="valueKey">The key identifying the value (for example, <c>CDATA</c> or a representation entry key).</param>
        /// <param name="value">The value to store.</param>
        public void AddValue(string valueKey, object value)
        {
            AddValue(new ObservationValue(valueKey, value));
        }

        /// <summary>
        /// Add or replace an observation value, removing any existing value with the same key and dropping the new value if it is empty.
        /// </summary>
        /// <param name="observationValue">The observation value to add.</param>
        public void AddValue(ObservationValue observationValue)
        {
            var x = new List<ObservationValue>();
            if (!Values.IsNullOrEmpty())
            {
                foreach (var value in Values)
                {
                    // Add existing values (except if matching the new observationValue.Key)
                    if (value._key != observationValue._key)
                    {
                        x.Add(value);
                    }
                }
            }
            if (observationValue.HasValue()) x.Add(observationValue);

            Values = x;
        }

        /// <summary>
        /// Get the value stored under the given key.
        /// </summary>
        /// <param name="valueKey">The key identifying the value to retrieve.</param>
        /// <returns>The matching value, or <c>null</c> if no value with the key is present.</returns>
        public string GetValue(string valueKey)
        {
            if (!string.IsNullOrEmpty(valueKey) && !Values.IsNullOrEmpty())
            {
                var x = Values.FirstOrDefault(o => o.Key == valueKey);
                return x.Value;
            }

            return null;
        }

        /// <summary>
        /// Remove all observation values, effectively suppressing the observation.
        /// </summary>
        public void ClearValues()
        {
            Values = null;
        }
    }
}
