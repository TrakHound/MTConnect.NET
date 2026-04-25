// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// Axis along or around which the Component moves relative to a coordinate system represented as a dataset.
    /// </summary>
    public interface IAxisDataSet : IDataSet
    {
        /// <summary>
        /// X-component of Axis.
        /// </summary>
        double X { get; }
        
        /// <summary>
        /// Y-component of Axis.
        /// </summary>
        double Y { get; }
        
        /// <summary>
        /// Z-component of Axis.
        /// </summary>
        double Z { get; }
    }
}