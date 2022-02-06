// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices;
using MTConnect.Observations;
using System.Collections.Generic;

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


        //public StoredObservation(string deviceName, DataItem dataItem, IObservation observation)
        //{
        //    DeviceName = deviceName;
        //    DataItemId = null;
        //    DataItemCategory = DataItemCategory.SAMPLE;
        //    DataItemRepresentation = DataItemRepresentation.VALUE;
        //    Sequence = 0;
        //    Timestamp = 0;

        //    if (dataItem != null)
        //    {
        //        DataItemId = dataItem.Id;
        //        DataItemCategory = dataItem.DataItemCategory;
        //        DataItemRepresentation = dataItem.Representation;
        //    }

        //    var values = new List<ObservationValue>();
        //    if (observation != null)
        //    {
        //        Timestamp = observation.Timestamp;

        //        if (!observation.Values.IsNullOrEmpty())
        //        {
        //            values.AddRange(observation.Values);
        //        }
        //    }
        //    Values = values;
        //}

        //public StoredObservation(string deviceName, string dataItemId, IObservation observation)
        //{
        //    DeviceName = deviceName;
        //    DataItemId = dataItemId;
        //    Sequence = 0;
        //    Timestamp = 0;

        //    var values = new List<ObservationValue>();
        //    if (observation != null)
        //    {
        //        Timestamp = observation.Timestamp;

        //        if (!observation.Values.IsNullOrEmpty())
        //        {
        //            values.AddRange(observation.Values);
        //        }
        //    }
        //    Values = values;
        //}

        //public StoredObservation(
        //    string deviceName,
        //    string dataItemId,
        //    IObservation observation,
        //    long sequence)
        //{
        //    DeviceName = deviceName;
        //    DataItemId = dataItemId;
        //    Sequence = sequence;
        //    Timestamp = 0;

        //    var values = new List<ObservationValue>();
        //    if (observation != null)
        //    {
        //        Timestamp = observation.Timestamp;

        //        if (!observation.Values.IsNullOrEmpty())
        //        {
        //            values.AddRange(observation.Values);
        //        }
        //    }
        //    Values = values;
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
