// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Observations.Events.Values;

namespace MTConnect.Models.DataItems
{
    /// <summary>
    /// The value of a signal or calculation issued to adjust the feedrate for the axes associated with a Path component that may represent a single axis or the coordinated movement of multiple axes.
    /// </summary>
    public class PathFeedrateOverrideModel
    {
        /// <summary>
        /// Directive value without offsets and adjustments.
        /// </summary>
        public PathFeedrateOverrideValue Programmed { get; set; }
        public IDataItemModel ProgrammedDataItem { get; set; }

        /// <summary>
        /// Performing an operation faster or in less time than nominal rate.
        /// </summary>
        public PathFeedrateOverrideValue Rapid { get; set; }
        public IDataItemModel RapidDataItem { get; set; }

        /// <summary>
        /// The value of a signal or calculation issued to adjust the feedrate of an individual linear type axis when that axis is being operated in a manual state or method(jogging).
        /// </summary>
        public PathFeedrateOverrideValue Jog { get; set; }
        public IDataItemModel JogDataItem { get; set; }
    }
}
