// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// Coordinates of the origin position of a coordinate system represented as a dataset.
    /// </summary>
    public interface IOriginDataSet : IDataSet
    {
        /// <summary>
        /// X-coordinate.
        /// </summary>
        string X { get; }
        
        /// <summary>
        /// Y-coordinate.
        /// </summary>
        string Y { get; }
        
        /// <summary>
        /// X-coordinate.
        /// </summary>
        string Z { get; }
    }
}