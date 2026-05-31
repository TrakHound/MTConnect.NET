// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.RegularExpressions;
using MTConnect.Observations;

namespace MTConnect.Shdr
{
    /// <summary>
    /// SHDR-flavoured <see cref="DataSetEntry"/> that adds round-tripping to and from the
    /// <c>key=value</c> token form used inside SHDR data-set lines (a removed entry is encoded as
    /// <c>key=</c> with an empty value).
    /// </summary>
    public class ShdrDataSetEntry : DataSetEntry
    {
        /// <summary>Creates an empty entry for serialiser-driven construction.</summary>
        public ShdrDataSetEntry() { }

        /// <summary>Creates an entry with an explicit key, value, and removed flag; <paramref name="value"/> is rendered via <see cref="object.ToString"/>.</summary>
        public ShdrDataSetEntry(string key, object value, bool removed = false)
        {
            Key = key;
            Value = value?.ToString();
            Removed = removed;
        }

        /// <summary>Clones the supplied <see cref="IDataSetEntry"/> into a new SHDR-flavoured entry.</summary>
        public ShdrDataSetEntry(IDataSetEntry entry)
        {
            if (entry != null)
            {
                Key = entry.Key;
                Value = entry.Value;
                Removed = entry.Removed;
            }
        }

        /// <summary>Serialises the entry to its SHDR <c>key=value</c> textual form (or <c>key=</c> when removed); returns the empty string when <see cref="DataSetEntry.Key"/> is empty.</summary>
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

        /// <summary>Parses a single SHDR <c>key=value</c> segment back into an entry; returns <c>null</c> when <paramref name="segment"/> is empty or does not match the expected pattern.</summary>
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