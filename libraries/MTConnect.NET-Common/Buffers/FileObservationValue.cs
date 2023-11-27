// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Observations;
using System.Text.Json.Serialization;

namespace MTConnect.Buffers
{
    struct FileObservationValue
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }


        public FileObservationValue(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public FileObservationValue(ObservationValue observationValue)
        {
            Key = observationValue.Key;
            Value = observationValue.Value;
        }


        public string[] ToArray()
        {
            return new string[] { Key, Value };
        }

        public FileObservationValue FromArray(object[] a)
        {
            if (a != null && a.Length > 1)
            {
                var key = a[0]?.ToString();
                var value = a[1]?.ToString();

                return new FileObservationValue(key, value);
            }

            return new FileObservationValue();
        }
    }
}