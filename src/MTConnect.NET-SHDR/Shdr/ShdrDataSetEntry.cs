// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.RegularExpressions;
using MTConnect.Observations;

namespace MTConnect.Shdr
{
    public class ShdrDataSetEntry : DataSetEntry
    {
        public ShdrDataSetEntry() { }

        public ShdrDataSetEntry(string key, object value, bool removed = false)
        {
            Key = key;
            Value = value?.ToString();
            Removed = removed;
        }

        public ShdrDataSetEntry(IDataSetEntry entry)
        {
            if (entry != null)
            {
                Key = entry.Key;
                Value = entry.Value;
                Removed = entry.Removed;
            }
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Key))
            {
                if (Removed)
                {
                    return $"{Key}=";
                }
                else
                {
                    return $"{Key}={Value}";
                }
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