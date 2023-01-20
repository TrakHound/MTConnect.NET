// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Filter provides a means to control when an Agent records updated information for a data item.
    /// </summary>
    public class Filter : IFilter
    {
        public DataItemFilterType Type { get; set; }

        /// <summary>
        /// The value associated with each Filter
        /// </summary>
        public double Value { get; set; }
    }
}
