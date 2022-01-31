// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

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
        /// The recorded value of the Observation
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// The ValueType of the recorded value of the Observation
        /// </summary>
        public string ValueType { get; set; }

        /// <summary>
        /// The timestamp (UnixTime in Milliseconds) that the observation was recorded at
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// A MD5 Hash of the Observation that can be used for comparison
        /// </summary>
        public string ChangeId
        {
            get
            {
                return Value.ToString();
            }
        }


        public Observation() { }

        public Observation(string key)
        {
            Key = key;
            ValueType = ValueTypes.CDATA;
            Timestamp = 0;
        }

        public Observation(string key, object value)
        {
            Key = key;
            ValueType = ValueTypes.CDATA;
            Value = value?.ToString();
            Timestamp = 0;
        }

        public Observation(string key, object value, long timestamp)
        {
            Key = key;
            ValueType = ValueTypes.CDATA;
            Value = value?.ToString();
            Timestamp = timestamp;
        }

        public Observation(string key, object value, DateTime timestamp)
        {
            Key = key;
            ValueType = ValueTypes.CDATA;
            Value = value?.ToString();
            Timestamp = timestamp.ToUnixTime();
        }

        public Observation(string key, string valueType, object value, long timestamp)
        {
            Key = key;
            ValueType = valueType;
            Value = value?.ToString();
            Timestamp = timestamp;
        }

        public Observation(string key, string valueType, object value, DateTime timestamp)
        {
            Key = key;
            ValueType = valueType;
            Value = value?.ToString();
            Timestamp = timestamp.ToUnixTime();
        }

        //public Observation(string deviceName, string key)
        //{
        //    Key = key;
        //    ValueType = ValueTypes.CDATA;
        //    Timestamp = 0;
        //}

        //public Observation(string deviceName, string key, object value)
        //{
        //    Key = key;
        //    ValueType = ValueTypes.CDATA;
        //    Value = value?.ToString();
        //    Timestamp = 0;
        //}

        //public Observation(string deviceName, string key, object value, long timestamp)
        //{
        //    Key = key;
        //    ValueType = ValueTypes.CDATA;
        //    Value = value?.ToString();
        //    Timestamp = timestamp;
        //}

        //public Observation(string deviceName, string key, object value, DateTime timestamp)
        //{
        //    Key = key;
        //    ValueType = ValueTypes.CDATA;
        //    Value = value?.ToString();
        //    Timestamp = timestamp.ToUnixTime();
        //}

        //public Observation(string deviceName, string key, string valueType, object value, long timestamp)
        //{
        //    Key = key;
        //    ValueType = valueType;
        //    Value = value?.ToString();
        //    Timestamp = timestamp;
        //}

        //public Observation(string deviceName, string key, string valueType, object value, DateTime timestamp)
        //{
        //    Key = key;
        //    ValueType = valueType;
        //    Value = value?.ToString();
        //    Timestamp = timestamp.ToUnixTime();
        //}


        public void Update(object value)
        {
            Value = value?.ToString();
        }

        public void Update(string valueType, object value)
        {
            ValueType = valueType;
            Value = value?.ToString();
        }

        public void Update(object value, DateTime timestamp)
        {
            Value = value?.ToString();
            Timestamp = timestamp.ToUnixTime();
        }

        public void Update(object value, long timestamp)
        {
            Value = value?.ToString();
            Timestamp = timestamp;
        }
    }
}
