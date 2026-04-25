// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// Axis along or around which the Component moves relative to a coordinate system.
    /// </summary>
    public interface IAxis : IAbstractAxis
    {
        /// <summary>
        /// 
        /// </summary>
        string Value { get; }
    }
}