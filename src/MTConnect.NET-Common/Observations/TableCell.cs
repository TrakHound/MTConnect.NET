// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Observations
{
    /// <summary>
    /// A Cell represents a Column within a Row of a tabular data.
    /// </summary>
    public class TableCell : ITableCell
    {
        /// <summary>
        /// A unique identifier for each key-value pair.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The Value for each key-value pair.
        /// </summary>
        public string Value { get; set; }


        public TableCell() { }

        public TableCell(string key, object value)
        {
            Key = key;
            Value = value?.ToString();
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Key))
            {
                return $"{Key}={Value}";
            }

            return "";
        }
    }
}
