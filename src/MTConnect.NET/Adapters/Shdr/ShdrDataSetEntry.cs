// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Text.RegularExpressions;
using MTConnect.Observations;

namespace MTConnect.Adapters
{
    public class ShdrDataSetEntry : DataSetEntry
    {
        public ShdrDataSetEntry() { }

        public ShdrDataSetEntry(string key, object value)
        {
            Key = key;
            Value = value?.ToString();
        }

        public ShdrDataSetEntry(DataSetEntry entry)
        {
            if (entry != null)
            {
                Key = entry.Key;
                Value = entry.Value;
            }
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Key))
            {
                return $"{Key}={Value}";
            }

            return "";
        }

        public static ShdrDataSetEntry FromString(string segment)
        {
            if (!string.IsNullOrEmpty(segment))
            {
                var regex = new Regex("(.*)=(.*)");
                var match = regex.Match(segment);
                if (match.Success && match.Groups.Count > 2)
                {
                    var key = match.Groups[1].Value;
                    var value = match.Groups[2].Value;

                    return new ShdrDataSetEntry(key, value);
                }
            }

            return null;
        }
    }
}
