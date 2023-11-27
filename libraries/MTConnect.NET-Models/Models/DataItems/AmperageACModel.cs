// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Observations.Samples.Values;

namespace MTConnect.Models.DataItems
{
    /// <summary>
    /// The measurement of an electrical current
    /// </summary>
    public class AmperageACModel
    {
        /// <summary>
        /// The measured or reported value of an observation.
        /// </summary>
        public AmperageACValue Actual { get; set; }
        public IDataItemModel ActualDataItem { get; set; }

        /// <summary>
        /// Directive value including adjustments such as an offset or overrides.
        /// </summary>
        public AmperageACValue Commanded { get; set; }
        public IDataItemModel CommandedDataItem { get; set; }

        /// <summary>
        /// Directive value without offsets and adjustments.
        /// </summary>
        public AmperageACValue Programmed { get; set; }
        public IDataItemModel ProgrammedDataItem { get; set; }
    }
}