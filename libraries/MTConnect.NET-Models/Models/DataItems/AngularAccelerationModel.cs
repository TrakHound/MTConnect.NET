// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Observations.Samples.Values;

namespace MTConnect.Models.DataItems
{
    /// <summary>
    /// The positive rate of change of angular velocity
    /// </summary>
    public class AngularAccelerationModel
    {
        /// <summary>
        /// The measured or reported value of an observation.
        /// </summary>
        public AngularAccelerationValue Actual { get; set; }
        public IDataItemModel ActualDataItem { get; set; }

        /// <summary>
        /// Directive value including adjustments such as an offset or overrides.
        /// </summary>
        public AngularAccelerationValue Commanded { get; set; }
        public IDataItemModel CommandedDataItem { get; set; }

        /// <summary>
        /// Directive value without offsets and adjustments.
        /// </summary>
        public AngularAccelerationValue Programmed { get; set; }
        public IDataItemModel ProgrammedDataItem { get; set; }
    }
}