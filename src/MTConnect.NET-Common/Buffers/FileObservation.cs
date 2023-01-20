// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Observations;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace MTConnect.Buffers
{
    struct FileObservation
    {
        public int Key { get; set; }

        public IEnumerable<object[]> Values { get; set; }

        public long Sequence { get; set; }

        public long Timestamp { get; set; }


        public FileObservation(BufferObservation observation)
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
            Sequence = observation.Sequence;
            Timestamp = observation.Timestamp;
        }

        public object[] ToArray()
        {
            return new object[] {
                Key,
                Values,
                Sequence,
                Timestamp
            };
        }

        public static FileObservation FromArray(object[] a)
        {
            var fileObservation = new FileObservation();

            if (a != null && a.Length >= 4)
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

                fileObservation.Sequence = a[2].ToLong();
                fileObservation.Timestamp = a[3].ToLong();
            }

            return fileObservation;
        }

        public BufferObservation ToBufferObservation()
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

            observation.Sequence = Sequence;
            observation.Timestamp = Timestamp;
            return observation;
        }
    }
}
