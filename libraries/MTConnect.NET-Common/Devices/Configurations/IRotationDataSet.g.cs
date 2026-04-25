// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// Rotations about X, Y, and Z axes are expressed in A, B, and C respectively within a 3-dimensional vector represented as a dataset.
    /// </summary>
    public interface IRotationDataSet : IDataSet
    {
        /// <summary>
        /// Rotation about X axis.
        /// </summary>
        string A { get; }
        
        /// <summary>
        /// Rotation about Y axis.
        /// </summary>
        string B { get; }
        
        /// <summary>
        /// Rotation about Z axis.
        /// </summary>
        string C { get; }
    }
}