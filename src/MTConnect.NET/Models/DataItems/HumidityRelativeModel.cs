// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Streams.Samples;

namespace MTConnect.Models.DataItems
{
    /// <summary>
    /// The amount of water vapor present expressed as a percent to reach saturation at the same temperature.
    /// </summary>
    public class HumidityRelativeModel
    {
        /// <summary>
        /// The measured or reported value of an observation.
        /// </summary>
        public HumidityRelativeValue Actual { get; set; }
        public IDataItemModel ActualDataItem { get; set; }

        /// <summary>
        /// Directive value including adjustments such as an offset or overrides.
        /// </summary>
        public HumidityRelativeValue Commanded { get; set; }
        public IDataItemModel CommandedDataItem { get; set; }
    }
}
