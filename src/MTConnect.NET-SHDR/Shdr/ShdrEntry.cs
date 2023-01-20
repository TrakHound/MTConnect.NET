// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Shdr
{
    /// <summary>
    /// A Key-Value pair published as part of a DataSet or Table Observation
    /// </summary>
    internal struct ShdrEntry
    {
        /// <summary>
        /// A unique identifier for the Entry
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The value associated with the Entry that represents what was observed during the Observation
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// An indicator to determine whether the Entry has been removed from the DataSet or Table during the Observation
        /// </summary>
        public bool Removed { get; set; }


        public ShdrEntry(string key, string value, bool removed = false)
        {
            Key = key;
            Value = value;
            Removed = removed;
        }
    }
}