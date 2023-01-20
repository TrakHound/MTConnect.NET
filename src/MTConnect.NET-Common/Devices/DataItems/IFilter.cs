// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Filter provides a means to control when an Agent records updated information for a data item.
    /// </summary>
    public interface IFilter
    {
        DataItemFilterType Type { get; }

        /// <summary>
        /// The value associated with each Filter
        /// </summary>
        double Value { get; }
    }
}