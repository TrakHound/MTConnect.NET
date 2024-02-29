// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;
using MTConnect.Observations;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace MTConnect.Buffers
{
    struct FileCurrentObservation
    {
        public int Key { get; set; }

        public IEnumerable<object[]> Values { get; set; }

        public int Representation { get; set; }

        public ulong Sequence { get; set; }

        public long Timestamp { get; set; }


        public FileCurrentObservation(BufferObservation observation)
        {
            Values = null;

            Key = observation.Key;
            if (!observation.Values.IsNullOrEmpty())
            {
                var values = new List<object[]>();
                foreach (var value in observation.Values)
                {
                    values.Add(new FileObservationValue(value).ToArray());
                }
                Values = values;
            }
            Representation = (int)observation.Representation;
            Sequence = observation.Sequence;
            Timestamp = observation.Timestamp;
        }

        public object[] ToArray()
        {
            return new object[] {
                Key,
                Values,
                Representation,
                Sequence,
                Timestamp
            };
        }

        public static FileCurrentObservation FromArray(object[] a)
        {
            var fileObservation = new FileCurrentObservation();

            if (a != null && a.Length >= 5)
            {
                fileObservation.Key = a[0].ToInt();

                var values = new List<object[]>();
                var valueElements = (JsonElement)a[1];
                foreach (var valueElement in valueElements.EnumerateArray())
                {
                    var valueArray = new object[2];

                    var x = valueElement.EnumerateArray().ToArray();
                    valueArray[0] = x[0];
                    valueArray[1] = x[1];

                    values.Add(valueArray);
                }
                fileObservation.Values = values;

                fileObservation.Representation = a[2].ToInt();
                fileObservation.Sequence = a[3].ToULong();
                fileObservation.Timestamp = a[4].ToLong();
            }

            return fileObservation;
        }

        public BufferObservation ToStoredObservation()
        {
            var observation = new BufferObservation();
            observation.Key = Key;

            if (!Values.IsNullOrEmpty())
            {
                var values = new List<ObservationValue>();
                foreach (var valueArray in Values)
                {
                    values.Add(new ObservationValue(valueArray[0].ToString(), valueArray[1].ToString()));
                }
                observation.Values = values.ToArray();
            }

            switch (Representation)
            {
                case 1: observation.Representation = DataItemRepresentation.DATA_SET; break;
                case 2: observation.Representation = DataItemRepresentation.DISCRETE; break;
                case 3: observation.Representation = DataItemRepresentation.TIME_SERIES; break;
                case 4: observation.Representation = DataItemRepresentation.TABLE; break;
                default: observation.Representation = DataItemRepresentation.VALUE; break;
            }

            observation.Sequence = Sequence;
            observation.Timestamp = Timestamp;
            return observation;
        }
    }
}