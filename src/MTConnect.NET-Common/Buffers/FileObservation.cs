// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.DataItems;
using MTConnect.Observations;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace MTConnect.Agents
{
    public struct FileObservation
    {
        public string DeviceUuid { get; set; }

        public string DataItemId { get; set; }

        public DataItemCategory DataItemCategory { get; set; }

        public DataItemRepresentation DataItemRepresentation { get; set; }

        public IEnumerable<object[]> Values { get; set; }

        public long Sequence { get; set; }

        public long Timestamp { get; set; }


        public FileObservation(StoredObservation observation)
        {
            Values = null;

            DeviceUuid = observation.DeviceUuid;
            DataItemId = observation.DataItemId;
            DataItemCategory = observation.DataItemCategory;
            DataItemRepresentation = observation.DataItemRepresentation;
            if (!observation.Values.IsNullOrEmpty())
            {
                var values = new List<object[]>();
                foreach (var value in observation.Values)
                {
                    values.Add(new FileObservationValue(value).ToArray());
                }
                Values = values;
            }
            Sequence = observation.Sequence;
            Timestamp = observation.Timestamp;
        }

        public object[] ToArray()
        {
            return new object[] {
                DeviceUuid,
                DataItemId,
                DataItemCategory,
                DataItemRepresentation,
                Values,
                Sequence,
                Timestamp
            };
        }

        public static FileObservation FromArray(object[] a)
        {
            var fileObservation = new FileObservation();

            if (a != null && a.Length >= 7)
            {
                fileObservation.DeviceUuid = a[0].ToString();
                fileObservation.DataItemId = a[1].ToString();
                fileObservation.DataItemCategory = a[2].ToString().ConvertEnum<DataItemCategory>();
                fileObservation.DataItemRepresentation = a[3].ToString().ConvertEnum<DataItemRepresentation>();

                var values = new List<object[]>();
                var valueElements = (JsonElement)a[4];
                foreach (var valueElement in valueElements.EnumerateArray())
                {
                    var valueArray = new object[2];

                    var x = valueElement.EnumerateArray().ToArray();
                    valueArray[0] = x[0];
                    valueArray[1] = x[1];

                    values.Add(valueArray);
                }
                fileObservation.Values = values;

                fileObservation.Sequence = a[5].ToLong();
                fileObservation.Timestamp = a[6].ToLong();
            }

            return fileObservation;
        }

        public StoredObservation ToStoredObservation()
        {
            var observation = new StoredObservation();
            observation.DeviceUuid = DeviceUuid;
            observation.DataItemId = DataItemId;
            observation.DataItemCategory = DataItemCategory;
            observation.DataItemRepresentation = DataItemRepresentation;

            if (!Values.IsNullOrEmpty())
            {
                var values = new List<ObservationValue>();
                foreach (var valueArray in Values)
                {
                    values.Add(new ObservationValue(valueArray[0].ToString(), valueArray[1].ToString()));
                }
                observation.Values = values;
            }

            observation.Sequence = Sequence;
            observation.Timestamp = Timestamp;
            return observation;
        }
    }
}
