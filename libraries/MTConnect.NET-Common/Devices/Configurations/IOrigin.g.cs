// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// Coordinates of the origin position of a coordinate system.
    /// </summary>
    public interface IOrigin : IAbstractOrigin
    {
        /// <summary>
        /// 
        /// </summary>
        string Value { get; }
    }
}