// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Observations.Samples.Values;

namespace MTConnect.Models.DataItems
{
    /// <summary>
    /// A measured or calculated position of a Component element as reported by a piece of equipment.
    /// </summary>
    public class PositionModel
    {
        /// <summary>
        /// The measured or reported value of an observation.
        /// </summary>
        public PositionValue Actual { get; set; }
        public IDataItemModel ActualDataItem { get; set; }

        /// <summary>
        /// Directive value including adjustments such as an offset or overrides.
        /// </summary>
        public PositionValue Commanded { get; set; }
        public IDataItemModel CommandedDataItem { get; set; }

        /// <summary>
        /// Directive value without offsets and adjustments.
        /// </summary>
        public PositionValue Programmed { get; set; }
        public IDataItemModel ProgrammedDataItem { get; set; }

        /// <summary>
        /// The goal of the operation or process.
        /// </summary>
        public PositionValue Target { get; set; }
        public IDataItemModel TargetDataItem { get; set; }
    }
}