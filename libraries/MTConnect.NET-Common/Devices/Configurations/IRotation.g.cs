// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// Rotations about X, Y, and Z axes are expressed in A, B, and C respectively within a 3-dimensional vector.
    /// </summary>
    public interface IRotation : IAbstractRotation
    {
        /// <summary>
        /// 
        /// </summary>
        string Value { get; }
    }
}