// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices;
using MTConnect.Observations;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Agents
{
    public struct StoredObservation
    {
        public string DeviceName { get; set; }

        public string DataItemId { get; set; }

        public DataItemCategory DataItemCategory { get; set; }

        public DataItemRepresentation DataItemRepresentation { get; set; }

        public IEnumerable<ObservationValue> Values { get; set; }

        public long Sequence { get; set; }

        public long Timestamp { get; set; }


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


        public void AddValue(ObservationValue observationValue)
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

        public IEnumerable<ObservationValue> GetValues(string valueTypePrefix)
        {
            if (!string.IsNullOrEmpty(valueTypePrefix) && !Values.IsNullOrEmpty())
            {
                return Values.Where(o => o.ValueType != null && o.ValueType.StartsWith(valueTypePrefix));
            }

            return null;
        }
    }
}
