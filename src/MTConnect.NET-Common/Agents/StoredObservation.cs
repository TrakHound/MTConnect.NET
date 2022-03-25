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
        public string DeviceUuid { get; set; }

        public string DataItemId { get; set; }

        public DataItemCategory DataItemCategory { get; set; }

        public DataItemRepresentation DataItemRepresentation { get; set; }

        public IEnumerable<ObservationValue> Values { get; set; }

        public long Sequence { get; set; }

        public long Timestamp { get; set; }

        public bool IsValid =>
            !string.IsNullOrEmpty(DeviceUuid) &&
            !string.IsNullOrEmpty(DataItemId) &&
            !Values.IsNullOrEmpty() &&
            Sequence > 0 &&
            Timestamp > 0;


        public string CreateHash()
        {
            var s = $"{DeviceUuid}|{DataItemId}";
            return s.ToMD5Hash();
        }

        public static string CreateHash(string deviceUuid, string dataItemId)
        {
            var s = $"{deviceUuid}|{dataItemId}";
            return s.ToMD5Hash();
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

        public string GetValue(string valueType)
        {
            if (!string.IsNullOrEmpty(valueType) && !Values.IsNullOrEmpty())
            {
                var x = Values.FirstOrDefault(o => o.Key == valueType);
                return x.Value;
            }

            return null;
        }

        public IEnumerable<ObservationValue> GetValues(string valueTypePrefix)
        {
            if (!string.IsNullOrEmpty(valueTypePrefix) && !Values.IsNullOrEmpty())
            {
                return Values.Where(o => o.Key != null && o.Key.StartsWith(valueTypePrefix));
            }

            return null;
        }
    }
}
