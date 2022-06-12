// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Observations;
using System.Text.Json.Serialization;

namespace MTConnect.Agents
{
    public struct FileObservationValue
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }


        public FileObservationValue() 
        {
            Key = null;
            Value = null;
        }

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
