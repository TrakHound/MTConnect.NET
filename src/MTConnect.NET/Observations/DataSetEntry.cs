// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Observations
{
    /// <summary>
    /// A key-value pair published as part of a Data Set observation.
    /// </summary>
    public class DataSetEntry
    {
        /// <summary>
        /// A unique identifier for each key-value pair.
        /// </summary>
        public string Key { get; set; }

        public string Value { get; set; }


        public DataSetEntry() { }

        public DataSetEntry(string key, object value)
        {
            Key = key;
            Value = value?.ToString();
        }
    }
}
