// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// Either a single multiplier applied to all three dimensions or a three space multiplier given in the X, Y, and Z dimensions in the coordinate system used for the SolidModel represented as a dataset.
    /// </summary>
    public interface IScaleDataSet : IDataSet
    {
        /// <summary>
        /// Multiplier for X axis.
        /// </summary>
        double X { get; }
        
        /// <summary>
        /// Multiplier for Y axis.
        /// </summary>
        double Y { get; }
        
        /// <summary>
        /// Multiplier for Z axis.
        /// </summary>
        double Z { get; }
    }
}