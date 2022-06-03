// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Observations.Input
{
    /// <summary>
    /// An Information Model that describes the Streaming Data reported by a piece of equipment.
    /// </summary>
    public class ObservationInput : IObservationInput
    {
        /// <summary>
        /// The UUID of the Device that the Observation is associated with
        /// </summary>
        public string DeviceKey { get; set; }

        /// <summary>
        /// The (ID, Name, or Source) of the DataItem that the Observation is associated with
        /// </summary>
        public string DataItemKey { get; set; }

        /// <summary>
        /// The Values recorded during the Observation
        /// </summary>
        public IEnumerable<ObservationValue> Values { get; set; }

        /// <summary>
        /// The timestamp (UnixTime in Milliseconds) that the observation was recorded at
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// The frequency at which the values were observed at
        /// </summary>
        public double Duration
        {
            get => GetValue(ValueKeys.Duration).ToDouble();
            set => AddValue(new ObservationValue(ValueKeys.Duration, value));
        }

        /// <summary>
        /// For those DataItem elements that report data that may be periodically reset to an initial value, 
        /// resetTriggered identifies when a reported value has been reset and what has caused that reset to occur.
        /// </summary>
        public ResetTriggered ResetTriggered
        {
            get => GetValue(ValueKeys.ResetTriggered).ConvertEnum<ResetTriggered>();
            set => AddValue(new ObservationValue(ValueKeys.ResetTriggered, value));
        }

        /// <summary>
        /// A MD5 Hash of the Observation that can be used for comparison
        /// </summary>
        public virtual string ChangeId
        {
            get
            {
                if (!Values.IsNullOrEmpty())
                {
                    var valueString = "";
                    foreach (var value in Values) valueString += $"{value.Key}={value.Value}:";
                    return valueString.ToMD5Hash();
                }
                
                return null;
            }
        }


        public ObservationInput() { }

        public ObservationInput(string dataItemKey)
        {
            DataItemKey = dataItemKey;
            Timestamp = 0;
        }

        public ObservationInput(string dataItemKey, object value)
        {
            DataItemKey = dataItemKey;
            Values = new List<ObservationValue> 
            { 
                new ObservationValue(ValueKeys.CDATA, value != null ? value.ToString() : string.Empty)
            };
            Timestamp = 0;
        }

        public ObservationInput(string dataItemKey, object value, long timestamp)
        {
            DataItemKey = dataItemKey;
            Values = new List<ObservationValue>
            {
                new ObservationValue(ValueKeys.CDATA, value != null ? value.ToString() : string.Empty)
            };
            Timestamp = timestamp;
        }

        public ObservationInput(string dataItemKey, object value, DateTime timestamp)
        {
            DataItemKey = dataItemKey;
            Values = new List<ObservationValue>
            {
                new ObservationValue(ValueKeys.CDATA, value != null ? value.ToString() : string.Empty)
            };
            Timestamp = timestamp.ToUnixTime();
        }

        public ObservationInput(string dataItemKey, string valueKey, object value, long timestamp)
        {
            DataItemKey = dataItemKey;
            Values = new List<ObservationValue>
            {
                new ObservationValue(valueKey, value != null ? value.ToString() : string.Empty)
            };
            Timestamp = timestamp;
        }

        public ObservationInput(string dataItemKey, string valueKey, object value, DateTime timestamp)
        {
            DataItemKey = dataItemKey;
            Values = new List<ObservationValue>
            {
                new ObservationValue(valueKey, value != null ? value.ToString() : string.Empty)
            };
            Timestamp = timestamp.ToUnixTime();
        }

        public ObservationInput(IObservation observation)
        {
            DataItemKey = observation.DataItemId;
            Timestamp = observation.Timestamp.ToUnixTime();
            Values = observation.Values;
        }


        public void Unavailable()
        {
            AddValue(ValueKeys.CDATA, Observation.Unavailable);
        }


        public void AddValue(string valueKey, object value)
        {
            AddValue(new ObservationValue(valueKey, value));
        }

        public void AddValue(ObservationValue observationValue)
        {
            List<ObservationValue> x = null;
            if (!Values.IsNullOrEmpty()) x = Values.ToList();
            if (x == null) x = new List<ObservationValue>();
            x.RemoveAll(o => o.Key == observationValue.Key);
            x.Add(observationValue);
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
