// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations
{
    /// <summary>
    /// A Cell represents a Column within a <see cref="ITableEntry"></see>.
    /// </summary>
    public class TableCell : ITableCell
    {
        /// <summary>
        /// A unique identifier for each key-value pair.
        /// </summary>
        public string Key { get; set; }

        public string KeyDescription { get; set; }

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