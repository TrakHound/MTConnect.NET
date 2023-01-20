// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Observations.Samples.Values;

namespace MTConnect.Models.DataItems
{
    /// <summary>
    /// The measurement of the electrical potential between two points in an electrical circuit in which the current periodically reverses direction.
    /// </summary>
    public class VoltageACModel
    {
        /// <summary>
        /// The measured or reported value of an observation.
        /// </summary>
        public VoltageACValue Actual { get; set; }
        public IDataItemModel ActualDataItem { get; set; }

        /// <summary>
        /// Directive value including adjustments such as an offset or overrides.
        /// </summary>
        public VoltageACValue Commanded { get; set; }
        public IDataItemModel CommandedDataItem { get; set; }

        /// <summary>
        /// Directive value without offsets and adjustments.
        /// </summary>
        public VoltageACValue Programmed { get; set; }
        public IDataItemModel ProgrammedDataItem { get; set; }
    }
}
