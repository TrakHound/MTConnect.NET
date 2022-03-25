// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Observations
{
    /// <summary>
    /// A key-value pair published as part of a Data Set observation.
    /// </summary>
    public interface IDataSetEntry
    {
        /// <summary>
        /// A unique identifier for each key-value pair.
        /// </summary>
        string Key { get; }

        /// <summary>
        /// Boolean removal indicator of a key-value pair that MUST be true or false.
        /// </summary>
        bool Removed { get; }

        /// <summary>
        /// The Value for each key-value pair.
        /// </summary>
        string Value { get; }
    }
}
