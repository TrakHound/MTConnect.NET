// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Observations.Samples.Values;

namespace MTConnect.Models.DataItems
{
    /// <summary>
    /// A measured or calculated position of a control point reported by a piece of equipment expressed in WORK coordinates. 
    /// The coordinate system will revert to MACHINE coordinates if WORK coordinates are not available.
    /// </summary>
    public class PathPositionModel
    {
        /// <summary>
        /// The measured or reported value of an observation.
        /// </summary>
        public PathPositionValue Actual { get; set; }
        public IDataItemModel ActualDataItem { get; set; }

        /// <summary>
        /// Directive value including adjustments such as an offset or overrides.
        /// </summary>
        public PathPositionValue Commanded { get; set; }
        public IDataItemModel CommandedDataItem { get; set; }

        /// <summary>
        /// Directive value without offsets and adjustments.
        /// </summary>
        public PathPositionValue Programmed { get; set; }
        public IDataItemModel ProgrammedDataItem { get; set; }

        /// <summary>
        /// The position provided by a measurement probe.
        /// </summary>
        public PathPositionValue Probe { get; set; }
        public IDataItemModel ProbeDataItem { get; set; }

        /// <summary>
        /// The goal of the operation or process.
        /// </summary>
        public PathPositionValue Target { get; set; }
        public IDataItemModel TargetDataItem { get; set; }
    }
}
