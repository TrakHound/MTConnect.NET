// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Observations
{
    /// <summary>
    /// A Cell represents a Column within a Row of a tabular data.
    /// </summary>
    public interface ITableCell
    {
        /// <summary>
        /// A unique identifier for each key-value pair.
        /// </summary>
        string Key { get; }

        /// <summary>
        /// The Value for each key-value pair.
        /// </summary>
        string Value { get; }
    }
}
