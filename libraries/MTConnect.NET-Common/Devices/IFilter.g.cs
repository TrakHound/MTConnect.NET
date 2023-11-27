// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices
{
    /// <summary>
    /// Provides a means to control when an agent records updated information for a DataItem.
    /// </summary>
    public interface IFilter
    {
        /// <summary>
        /// Type of Filter.
        /// </summary>
        MTConnect.Devices.DataItemFilterType Type { get; }
        
        /// <summary>
        /// 
        /// </summary>
        double Value { get; }
    }
}