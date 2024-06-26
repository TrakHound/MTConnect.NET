// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations
{
    /// <summary>
    /// A Cell represents a Column within a <see cref="ITableEntry"></see>.
    /// </summary>
    public interface ITableCell
    {
        /// <summary>
        /// A unique identifier for each key-value pair.
        /// </summary>
        string Key { get; }

        string KeyDescription { get; set; }

        /// <summary>
        /// The Value for each key-value pair.
        /// </summary>
        string Value { get; }
    }
}