// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations
{
    /// <summary>
    /// A key-value pair published as part of an Observation with a DataItemRepresentation of <see cref="Devices.DataItemRepresentation.DATA_SET">DATA_SET</see>.
    /// </summary>
    public interface IDataSetEntry
    {
        /// <summary>
        /// A unique identifier for each key-value pair.
        /// </summary>
        string Key { get; }

        string KeyDescription { get; }

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