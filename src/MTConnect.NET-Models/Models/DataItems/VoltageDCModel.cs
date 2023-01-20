// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Observations.Samples.Values;

namespace MTConnect.Models.DataItems
{
    /// <summary>
    /// The measurement of the electrical potential between two points in an electrical circuit in which the current is unidirectional.
    /// </summary>
    public class VoltageDCModel
    {
        /// <summary>
        /// The measured or reported value of an observation.
        /// </summary>
        public VoltageDCValue Actual { get; set; }
        public IDataItemModel ActualDataItem { get; set; }

        /// <summary>
        /// Directive value including adjustments such as an offset or overrides.
        /// </summary>
        public VoltageDCValue Commanded { get; set; }
        public IDataItemModel CommandedDataItem { get; set; }

        /// <summary>
        /// Directive value without offsets and adjustments.
        /// </summary>
        public VoltageDCValue Programmed { get; set; }
        public IDataItemModel ProgrammedDataItem { get; set; }
    }
}