// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Observations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTConnect.Input
{
    /// <summary>
    /// An Information Model that describes the Streaming Data reported by a piece of equipment.
    /// </summary>
    public class ObservationInput : IObservationInput
    {
        private static readonly Encoding _utf8 = new UTF8Encoding();

        private byte[] _changeId;
        private byte[] _changeIdWithTimestamp;


        /// <summary>
        /// The UUID or Name of the Device that the Observation is associated with
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
            set
            {
                if (value > 0) AddValue(new ObservationValue(ValueKeys.Duration, value));
            }
        }

        /// <summary>
        /// For those DataItem elements that report data that may be periodically reset to an initial value, 
        /// resetTriggered identifies when a reported value has been reset and what has caused that reset to occur.
        /// </summary>
        public ResetTriggered ResetTriggered
        {
            get => GetValue(ValueKeys.ResetTriggered).ConvertEnum<ResetTriggered>();
            set
            {
                if (value != ResetTriggered.NOT_SPECIFIED) AddValue(new ObservationValue(ValueKeys.ResetTriggered, value));
            }
        }

        /// <summary>
        /// Gets or Sets whether the Observation is Unavailable
        /// </summary>
        public bool IsUnavailable { get; set; }

        /// <summary>
        /// An MD5 Hash of the Observation that can be used for comparison
        /// </summary>
        public byte[] ChangeId
        {
            get
            {
                if (_changeId == null) _changeId = CreateChangeId(this, false);
                return _changeId;
            }
        }

        /// <summary>
        /// An MD5 Hash of the Observation including the Timestamp that can be used for comparison
        /// </summary>
        public byte[] ChangeIdWithTimestamp
        {
            get
            {
                if (_changeIdWithTimestamp == null) _changeIdWithTimestamp = CreateChangeId(this, true);
                return _changeIdWithTimestamp;
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
                new ObservationValue(ValueKeys.Result, value != null ? value.ToString() : string.Empty)
            };
            Timestamp = 0;
        }

        public ObservationInput(string dataItemKey, object value, long timestamp)
        {
            DataItemKey = dataItemKey;
            Values = new List<ObservationValue>
            {
                new ObservationValue(ValueKeys.Result, value != null ? value.ToString() : string.Empty)
            };
            Timestamp = timestamp;
        }

        public ObservationInput(string dataItemKey, object value, DateTime timestamp)
        {
            DataItemKey = dataItemKey;
            Values = new List<ObservationValue>
            {
                new ObservationValue(ValueKeys.Result, value != null ? value.ToString() : string.Empty)
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

        public ObservationInput(IObservationInput observationInput)
        {
            DeviceKey = observationInput.DeviceKey;
            DataItemKey = observationInput.DataItemKey;
            Timestamp = observationInput.Timestamp;
            Values = observationInput.Values;
        }

        public ObservationInput(IObservation observation)
        {
            DataItemKey = observation.DataItemId;
            Timestamp = observation.Timestamp.ToUnixTime();
            Values = observation.Values;
        }


        public void Unavailable()
        {
            AddValue(ValueKeys.Result, Observation.Unavailable);
            IsUnavailable = true;
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
            _changeId = null;
            _changeIdWithTimestamp = null;
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
            _changeId = null;
            _changeIdWithTimestamp = null;
        }


        private static byte[] CreateChangeId(IObservationInput observationInput, bool includeTimestamp)
        {
            if (observationInput != null)
            {
                if (observationInput.IsUnavailable) return Observation.Unavailable.ToMD5HashBytes();

                if (!observationInput.Values.IsNullOrEmpty())
                {
                    var sb = new StringBuilder();

                    // Add DeviceKey (if specified)
                    if (!string.IsNullOrEmpty(observationInput.DeviceKey)) sb.Append($"{observationInput.DeviceKey}:::");

                    // Add DataItemKey
                    sb.Append($"{observationInput.DataItemKey}::");

                    // Add Timestamp
                    if (includeTimestamp) sb.Append($"timestamp={observationInput.Timestamp}:");

                    // Create String with ValueKey=Value segments
                    foreach (var value in observationInput.Values) sb.Append($"{value.Key}={value.Value}:");

                    // Get Bytes from StringBuilder
                    char[] a = new char[sb.Length];
                    sb.CopyTo(0, a, 0, sb.Length);

                    // Convert StringBuilder result to UTF8 MD5 Bytes
                    return _utf8.GetBytes(a).ToMD5HashBytes();
                }
            }

            return null;
        }
    }
}