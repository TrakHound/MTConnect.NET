// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Observations.Samples.Values;

namespace MTConnect.Models.DataItems
{
    /// <summary>
    /// The ratio of the water vapor present over the total weight of the water vapor and air present expressed as a percent.
    /// </summary>
    public class HumiditySpecificModel
    {
        /// <summary>
        /// The measured or reported value of an observation.
        /// </summary>
        public HumiditySpecificValue Actual { get; set; }
        public IDataItemModel ActualDataItem { get; set; }

        /// <summary>
        /// Directive value including adjustments such as an offset or overrides.
        /// </summary>
        public HumiditySpecificValue Commanded { get; set; }
        public IDataItemModel CommandedDataItem { get; set; }
    }
}