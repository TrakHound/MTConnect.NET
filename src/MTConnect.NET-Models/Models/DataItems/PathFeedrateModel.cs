// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Observations.Samples.Values;

namespace MTConnect.Models.DataItems
{
    /// <summary>
    /// The measurement of the feedrate for the axes, or a single axis, associated with a Path component-a vector.
    /// </summary>
    public class PathFeedrateModel
    {
        /// <summary>
        /// The measured or reported value of an observation.
        /// </summary>
        public PathFeedrateValue Actual { get; set; }
        public IDataItemModel ActualDataItem { get; set; }

        /// <summary>
        /// Directive value including adjustments such as an offset or overrides.
        /// </summary>
        public PathFeedrateValue Commanded { get; set; }
        public IDataItemModel CommandedDataItem { get; set; }

        /// <summary>
        /// The feedrate specified by a logic or motion program, by a pre-set value, or set by a switch as the feedrate for a linear axis when operating in a manual state or method(jogging).
        /// </summary>
        public PathFeedrateValue Jog { get; set; }
        public IDataItemModel JogDataItem { get; set; }

        /// <summary>
        /// Directive value without offsets and adjustments.
        /// </summary>
        public PathFeedrateValue Programmed { get; set; }
        public IDataItemModel ProgrammedDataItem { get; set; }

        /// <summary>
        /// Performing an operation faster or in less time than nominal rate.
        /// </summary>
        public PathFeedrateValue Rapid { get; set; }
        public IDataItemModel RapidDataItem { get; set; }
    }
}