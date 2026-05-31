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

        /// <summary>
        /// An optional human-readable description of the cell's <see cref="Key"/> (column).
        /// </summary>
        public string KeyDescription { get; set; }

        /// <summary>
        /// The Value for each key-value pair.
        /// </summary>
        public string Value { get; set; }


        /// <summary>
        /// Initializes a new, empty Table cell.
        /// </summary>
        public TableCell() { }

        /// <summary>
        /// Initializes a new Table cell with the given column key and value.
        /// </summary>
        /// <param name="key">The cell's column key.</param>
        /// <param name="value">The cell's value; stringified when stored.</param>
        public TableCell(string key, object value)
        {
            Key = key;
            Value = value?.ToString();
        }

        /// <summary>
        /// Returns the cell formatted as "key=value", or an empty string when the cell has no key.
        /// </summary>
        /// <returns>The string representation of the cell.</returns>
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