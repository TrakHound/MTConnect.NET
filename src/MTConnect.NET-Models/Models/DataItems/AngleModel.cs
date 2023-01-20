// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Observations.Samples.Values;

namespace MTConnect.Models.DataItems
{
    /// <summary>
    /// The measurement of angular position.
    /// </summary>
    public class AngleModel
    {
        /// <summary>
        /// The measured or reported value of an observation.
        /// </summary>
        public AngleValue Actual { get; set; }
        public IDataItemModel ActualDataItem { get; set; }

        /// <summary>
        /// Directive value including adjustments such as an offset or overrides.
        /// </summary>
        public AngleValue Commanded { get; set; }
        public IDataItemModel CommandedDataItem { get; set; }
    }
}