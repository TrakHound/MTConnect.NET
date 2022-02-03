// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Observations;
using System.Collections.Generic;

namespace MTConnect.Agents
{
    public struct StoredObservation
    {
        public string DeviceName { get; set; }

        public string DataItemId { get; set; }

        public IEnumerable<ObservationValue> Values { get; set; }

        //public string ValueType { get; set; }

        //public object Value { get; set; }

        public long Sequence { get; set; }

        public long Timestamp { get; set; }


        public StoredObservation(string deviceName, string dataItemId, IObservation observation)
        {
            DeviceName = deviceName;
            DataItemId = dataItemId;
            Sequence = 0;
            Timestamp = 0;

            var values = new List<ObservationValue>();
            if (observation != null)
            {
                Timestamp = observation.Timestamp;

                if (!observation.Values.IsNullOrEmpty())
                {
                    values.AddRange(observation.Values);
                }
            }
            Values = values;
        }

        public StoredObservation(
            string deviceName,
            string dataItemId,
            IObservation observation,
            long sequence)
        {
            DeviceName = deviceName;
            DataItemId = dataItemId;
            Sequence = sequence;
            Timestamp = 0;

            var values = new List<ObservationValue>();
            if (observation != null)
            {
                Timestamp = observation.Timestamp;

                if (!observation.Values.IsNullOrEmpty())
                {
                    values.AddRange(observation.Values);
                }
            }
            Values = values;
        }

        //public StoredObservation(
        //    string deviceName,
        //    string dataItemId,
        //    string valueType,
        //    object value,
        //    long timestamp)
        //{
        //    DeviceName = deviceName;
        //    DataItemId = dataItemId;
        //    ValueType = valueType;
        //    Value = value;
        //    Sequence = 0;
        //    Timestamp = timestamp;
        //}

        //public StoredObservation(
        //    string deviceName,
        //    string dataItemId, 
        //    string valueType,
        //    object value,
        //    long sequence,
        //    long timestamp)
        //{
        //    DeviceName = deviceName;
        //    DataItemId = dataItemId;
        //    ValueType = valueType;
        //    Value = value;
        //    Sequence = sequence;
        //    Timestamp = timestamp;
        //}

        public string CreateHash()
        {
            var s = $"{DeviceName}|{DataItemId}";
            return s.ToMD5Hash();
        }

        public static string CreateHash(string deviceName, string dataItemId)
        {
            var s = $"{deviceName}|{dataItemId}";
            return s.ToMD5Hash();
        }
    }
}
