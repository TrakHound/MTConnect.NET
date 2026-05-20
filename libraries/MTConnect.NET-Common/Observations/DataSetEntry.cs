// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations
{
    /// <summary>
    /// A key-value pair published as part of an Observation with a DataItemRepresentation of <see cref="Devices.DataItemRepresentation.DATA_SET">DATA_SET</see>.
    /// </summary>
    public class DataSetEntry : IDataSetEntry
    {
        /// <summary>
        /// A unique identifier for each key-value pair.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// An optional human-readable description of the entry's <see cref="Key"/>.
        /// </summary>
        public string KeyDescription { get; set; }

        /// <summary>
        /// Boolean removal indicator of a key-value pair that MUST be true or false.
        /// </summary>
        public bool Removed { get; set; }

        /// <summary>
        /// The Value for each key-value pair.
        /// </summary>
        public string Value { get; set; }


        /// <summary>
        /// Initializes a new, empty Data Set entry.
        /// </summary>
        public DataSetEntry() { }

        /// <summary>
        /// Initializes a new Data Set entry with the given key and value.
        /// </summary>
        /// <param name="key">The unique key for the entry.</param>
        /// <param name="value">The entry's value; stringified when stored.</param>
        public DataSetEntry(string key, object value)
        {
            Key = key;
            Value = value?.ToString();
        }

        /// <summary>
        /// Initializes a new Data Set entry that marks a key as added or removed.
        /// </summary>
        /// <param name="key">The unique key for the entry.</param>
        /// <param name="removed">Whether the key was removed from the set.</param>
        public DataSetEntry(string key, bool removed)
        {
            Key = key;
            Removed = removed;
        }
    }
}