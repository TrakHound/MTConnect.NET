// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Models.DataItems
{
    /// <summary>
    /// The value of a signal or calculation issued to adjust the feedrate of an individual linear type axis.
    /// </summary>
    public class AxisFeedrateOverrideModel
    {
        /// <summary>
        /// Directive value without offsets and adjustments.
        /// </summary>
        public double Programmed { get; set; }
        public IDataItemModel ProgrammedDataItem { get; set; }

        /// <summary>
        /// Performing an operation faster or in less time than nominal rate.
        /// </summary>
        public double Rapid { get; set; }
        public IDataItemModel RapidDataItem { get; set; }

        /// <summary>
        /// The value of a signal or calculation issued to adjust the feedrate of an individual linear type axis when that axis is being operated in a manual state or method(jogging).
        /// </summary>
        public double Jog { get; set; }
        public IDataItemModel JogDataItem { get; set; }
    }
}