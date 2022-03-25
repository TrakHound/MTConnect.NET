// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Observations.Samples.Values;

namespace MTConnect.Models.DataItems
{
    /// <summary>
    /// The measurement of the feedrate of a linear axis.
    /// </summary>
    public class AxisFeedrateModel
    {
        /// <summary>
        /// The measured or reported value of an observation.
        /// </summary>
        public AxisFeedrateValue Actual { get; set; }
        public IDataItemModel ActualDataItem { get; set; }

        /// <summary>
        /// Directive value including adjustments such as an offset or overrides.
        /// </summary>
        public AxisFeedrateValue Commanded { get; set; }
        public IDataItemModel CommandedDataItem { get; set; }

        /// <summary>
        /// The feedrate specified by a logic or motion program, by a pre-set value, or set by a switch as the feedrate for a linear axis when operating in a manual state or method(jogging).
        /// </summary>
        public AxisFeedrateValue Jog { get; set; }
        public IDataItemModel JogDataItem { get; set; }

        /// <summary>
        /// Directive value without offsets and adjustments.
        /// </summary>
        public AxisFeedrateValue Programmed { get; set; }
        public IDataItemModel ProgrammedDataItem { get; set; }

        /// <summary>
        /// Performing an operation faster or in less time than nominal rate.
        /// </summary>
        public AxisFeedrateValue Rapid { get; set; }
        public IDataItemModel RapidDataItem { get; set; }
    }
}
