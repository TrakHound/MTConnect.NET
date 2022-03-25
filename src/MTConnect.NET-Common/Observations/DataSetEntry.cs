// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Observations
{
    /// <summary>
    /// A key-value pair published as part of a Data Set observation.
    /// </summary>
    public class DataSetEntry : IDataSetEntry
    {
        /// <summary>
        /// A unique identifier for each key-value pair.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Boolean removal indicator of a key-value pair that MUST be true or false.
        /// </summary>
        public bool Removed { get; set; }

        /// <summary>
        /// The Value for each key-value pair.
        /// </summary>
        public string Value { get; set; }


        public DataSetEntry() { }

        public DataSetEntry(string key, object value)
        {
            Key = key;
            Value = value?.ToString();
        }

        public DataSetEntry(string key, bool removed)
        {
            Key = key;
            Removed = removed;
        }
    }
}
