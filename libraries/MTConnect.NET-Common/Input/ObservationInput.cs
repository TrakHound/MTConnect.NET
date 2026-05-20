// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
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
        /// The TimeZone that is configured to Output
        /// </summary>
        public TimeZoneInfo TimeZoneInfo { get; set; }

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


        /// <summary>
        /// Initializes a new, empty Observation with no DataItem key, values, or timestamp.
        /// </summary>
        public ObservationInput() { }

        /// <summary>
        /// Initializes a new Observation for the specified DataItem with an unset timestamp.
        /// </summary>
        /// <param name="dataItemKey">The (ID, Name, or Source) of the DataItem the Observation applies to.</param>
        public ObservationInput(string dataItemKey)
        {
            DataItemKey = dataItemKey;
            Timestamp = 0;
        }

        /// <summary>
        /// Initializes a new Observation for the specified DataItem and records the value under the Result ValueKey.
        /// </summary>
        /// <param name="dataItemKey">The (ID, Name, or Source) of the DataItem the Observation applies to.</param>
        /// <param name="value">The Result value. A <c>null</c> value is stored as an empty string.</param>
        public ObservationInput(string dataItemKey, object value)
        {
            DataItemKey = dataItemKey;
            Values = new List<ObservationValue>
            {
                new ObservationValue(ValueKeys.Result, value != null ? value.ToString() : string.Empty)
            };
            Timestamp = 0;
        }

        /// <summary>
        /// Initializes a new Observation for the specified DataItem, recording the value under the Result ValueKey at the given timestamp.
        /// </summary>
        /// <param name="dataItemKey">The (ID, Name, or Source) of the DataItem the Observation applies to.</param>
        /// <param name="value">The Result value. A <c>null</c> value is stored as an empty string.</param>
        /// <param name="timestamp">The observation timestamp as UnixTime in milliseconds.</param>
        public ObservationInput(string dataItemKey, object value, long timestamp)
        {
            DataItemKey = dataItemKey;
            Values = new List<ObservationValue>
            {
                new ObservationValue(ValueKeys.Result, value != null ? value.ToString() : string.Empty)
            };
            Timestamp = timestamp;
        }

        /// <summary>
        /// Initializes a new Observation for the specified DataItem, recording the value under the Result ValueKey at the given timestamp.
        /// </summary>
        /// <param name="dataItemKey">The (ID, Name, or Source) of the DataItem the Observation applies to.</param>
        /// <param name="value">The Result value. A <c>null</c> value is stored as an empty string.</param>
        /// <param name="timestamp">The observation timestamp, converted to UnixTime in milliseconds.</param>
        public ObservationInput(string dataItemKey, object value, DateTime timestamp)
        {
            DataItemKey = dataItemKey;
            Values = new List<ObservationValue>
            {
                new ObservationValue(ValueKeys.Result, value != null ? value.ToString() : string.Empty)
            };
            Timestamp = timestamp.ToUnixTime();
        }

        /// <summary>
        /// Initializes a new Observation for the specified DataItem, recording the value under an explicit ValueKey at the given timestamp.
        /// </summary>
        /// <param name="dataItemKey">The (ID, Name, or Source) of the DataItem the Observation applies to.</param>
        /// <param name="valueKey">The ValueKey identifying the Representation component the value applies to.</param>
        /// <param name="value">The value to record. A <c>null</c> value is stored as an empty string.</param>
        /// <param name="timestamp">The observation timestamp as UnixTime in milliseconds.</param>
        public ObservationInput(string dataItemKey, string valueKey, object value, long timestamp)
        {
            DataItemKey = dataItemKey;
            Values = new List<ObservationValue>
            {
                new ObservationValue(valueKey, value != null ? value.ToString() : string.Empty)
            };
            Timestamp = timestamp;
        }

        /// <summary>
        /// Initializes a new Observation for the specified DataItem, recording the value under an explicit ValueKey at the given timestamp.
        /// </summary>
        /// <param name="dataItemKey">The (ID, Name, or Source) of the DataItem the Observation applies to.</param>
        /// <param name="valueKey">The ValueKey identifying the Representation component the value applies to.</param>
        /// <param name="value">The value to record. A <c>null</c> value is stored as an empty string.</param>
        /// <param name="timestamp">The observation timestamp, converted to UnixTime in milliseconds.</param>
        public ObservationInput(string dataItemKey, string valueKey, object value, DateTime timestamp)
        {
            DataItemKey = dataItemKey;
            Values = new List<ObservationValue>
            {
                new ObservationValue(valueKey, value != null ? value.ToString() : string.Empty)
            };
            Timestamp = timestamp.ToUnixTime();
        }

        /// <summary>
        /// Initializes a new Observation by copying the Device key, DataItem key, timestamp, and values from an existing Observation.
        /// </summary>
        /// <param name="observationInput">The source Observation to copy.</param>
        public ObservationInput(IObservationInput observationInput)
        {
            DeviceKey = observationInput.DeviceKey;
            DataItemKey = observationInput.DataItemKey;
            Timestamp = observationInput.Timestamp;
            Values = observationInput.Values;
        }

        /// <summary>
        /// Initializes a new Observation from a reported <see cref="IObservation"/>, mapping its Device UUID, DataItem ID, timestamp, and values.
        /// </summary>
        /// <param name="observation">The reported Observation to convert.</param>
        public ObservationInput(IObservation observation)
        {
            DeviceKey = observation.DeviceUuid;
            DataItemKey = observation.DataItemId;
            Timestamp = observation.Timestamp.ToUnixTime();
            Values = observation.Values;
        }


        /// <summary>
        /// Marks the Observation as Unavailable, setting the Result to the Unavailable constant and the <see cref="IsUnavailable"/> flag.
        /// </summary>
        public void Unavailable()
        {
            AddValue(ValueKeys.Result, Observation.Unavailable);
            IsUnavailable = true;
        }


        /// <summary>
        /// Adds a value to the Observation using the specified ValueKey and raw value.
        /// </summary>
        /// <param name="valueKey">The ValueKey that identifies the Representation component the value applies to.</param>
        /// <param name="value">The value to record.</param>
        public void AddValue(string valueKey, object value)
        {
            AddValue(new ObservationValue(valueKey, value));
        }

        /// <summary>
        /// Adds a <see cref="ObservationValue"/>, replacing any existing value with the same ValueKey and resetting cached change identifiers.
        /// </summary>
        /// <param name="observationValue">The ValueKey and value pair to record. Values with no content are not stored.</param>
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

        /// <summary>
        /// Gets the value recorded for the specified ValueKey.
        /// </summary>
        /// <param name="valueKey">The ValueKey that identifies the Representation component to retrieve.</param>
        /// <returns>The recorded value, or <c>null</c> when the ValueKey is empty or no matching value exists.</returns>
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
        /// Removes all recorded values and resets cached change identifiers.
        /// </summary>
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

        /// <summary>
        /// Formats a timestamp as an ISO 8601 string, optionally applying a time zone offset and appending a duration suffix.
        /// </summary>
        /// <param name="timestamp">The timestamp as UnixTime in milliseconds. A non-positive value yields a duration-only or <c>null</c> result.</param>
        /// <param name="duration">The optional sample period appended as <c>@duration</c> when greater than zero.</param>
        /// <param name="timeZoneInfo">The optional time zone used to compute the offset; UTC is used when not specified.</param>
        /// <returns>The formatted timestamp string, or <c>null</c> when neither a timestamp nor a duration is provided.</returns>
        protected static string GetTimestampString(long timestamp, double duration = 0, TimeZoneInfo timeZoneInfo = null)
        {
            if (timestamp > 0)
            {
                var dateTime = timestamp.ToDateTime();
                var dateTimeOffset = MTConnectTimeZone.GetTimestamp(dateTime, timeZoneInfo);

                if (dateTimeOffset.Offset != TimeSpan.Zero)
                {
                    if (duration > 0)
                    {
                        return $"{dateTimeOffset.ToString("o")}@{duration}";
                    }
                    else
                    {
                        return dateTimeOffset.ToString("o");
                    }
                }
                else
                {
                    if (duration > 0)
                    {
                        return $"{dateTimeOffset.UtcDateTime.ToString("o")}@{duration}";
                    }
                    else
                    {
                        return dateTimeOffset.UtcDateTime.ToString("o");
                    }
                }
            }
            else if (duration > 0)
            {
                return $"@{duration}";
            }

            return null;
        }
    }
}