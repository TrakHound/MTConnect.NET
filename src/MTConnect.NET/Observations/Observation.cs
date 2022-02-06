// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices;
using MTConnect.Streams;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Observations
{
    /// <summary>
    /// An Information Model that describes the Streaming Data reported by a piece of equipment.
    /// </summary>
    public class Observation : IObservation
    {
        /// <summary>
        /// The name of the Device that the Observation is associated with
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// The (ID, Name, or Source) of the DataItem that the Observation is associated with
        /// </summary>
        public string Key { get; set; }

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
            get => GetValue(ValueTypes.Duration).ToDouble();
            set => AddValue(new ObservationValue(ValueTypes.Duration, value));
        }

        /// <summary>
        /// For those DataItem elements that report data that may be periodically reset to an initial value, 
        /// resetTriggered identifies when a reported value has been reset and what has caused that reset to occur.
        /// </summary>
        public ResetTriggered ResetTriggered
        {
            get => GetValue(ValueTypes.ResetTriggered).ConvertEnum<ResetTriggered>();
            set => AddValue(new ObservationValue(ValueTypes.ResetTriggered, value));
        }

        /// <summary>
        /// A MD5 Hash of the Observation that can be used for comparison
        /// </summary>
        public string ChangeId
        {
            get
            {
                if (!Values.IsNullOrEmpty())
                {
                    var valueString = "";
                    foreach (var value in Values) valueString += value.Value;
                    return valueString.ToMD5Hash();
                }
                
                return null;
            }
        }


        public Observation() { }

        public Observation(string key)
        {
            Key = key;
            Timestamp = 0;
        }

        public Observation(string key, object value)
        {
            Key = key;
            Values = new List<ObservationValue> 
            { 
                new ObservationValue(ValueTypes.CDATA, value != null ? value.ToString() : string.Empty)
            };
            Timestamp = 0;
        }

        public Observation(string key, object value, long timestamp)
        {
            Key = key;
            Values = new List<ObservationValue>
            {
                new ObservationValue(ValueTypes.CDATA, value != null ? value.ToString() : string.Empty)
            };
            Timestamp = timestamp;
        }

        public Observation(string key, object value, DateTime timestamp)
        {
            Key = key;
            Values = new List<ObservationValue>
            {
                new ObservationValue(ValueTypes.CDATA, value != null ? value.ToString() : string.Empty)
            };
            Timestamp = timestamp.ToUnixTime();
        }

        public Observation(string key, string valueType, object value, long timestamp)
        {
            Key = key;
            Values = new List<ObservationValue>
            {
                new ObservationValue(valueType, value != null ? value.ToString() : string.Empty)
            };
            Timestamp = timestamp;
        }

        public Observation(string key, string valueType, object value, DateTime timestamp)
        {
            Key = key;
            Values = new List<ObservationValue>
            {
                new ObservationValue(valueType, value != null ? value.ToString() : string.Empty)
            };
            Timestamp = timestamp.ToUnixTime();
        }


        protected void AddValue(string valueType, object value)
        {
            AddValue(new ObservationValue(valueType, value));
        }

        protected void AddValue(ObservationValue observationValue)
        {
            List<ObservationValue> x = null;
            if (!Values.IsNullOrEmpty()) x = Values.ToList();
            if (x == null) x = new List<ObservationValue>();
            x.RemoveAll(o => o.ValueType == observationValue.ValueType);
            x.Add(observationValue);
            Values = x;
        }

        public string GetValue(string valueType)
        {
            if (!string.IsNullOrEmpty(valueType) && !Values.IsNullOrEmpty())
            {
                var x = Values.FirstOrDefault(o => o.ValueType == valueType);
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
