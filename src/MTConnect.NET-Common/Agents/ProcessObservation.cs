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
    public class ProcessObservation
    {
        public IMTConnectAgent Agent { get; set; }

        public IDataItem DataItem { get; set; }

        public IEnumerable<ObservationValue> Values { get; set; }

        public DateTime Timestamp { get; set; }


        public ProcessObservation(IMTConnectAgent agent, IDataItem dataItem, IObservationInput observation)
        {
            Agent = agent;
            DataItem = dataItem;
            Values = observation.Values;
            Timestamp = observation.Timestamp.ToDateTime();
        }


        public void AddValue(string valueKey, object value)
        {
            AddValue(new ObservationValue(valueKey, value));
        }

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

        public string GetValue(string valueKey)
        {
            if (!string.IsNullOrEmpty(valueKey) && !Values.IsNullOrEmpty())
            {
                var x = Values.FirstOrDefault(o => o.Key == valueKey);
                return x.Value;
            }

            return null;
        }

        public void ClearValues()
        {
            Values = null;
        }
    }
}
