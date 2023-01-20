// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Observations.Samples.Values;

namespace MTConnect.Models.DataItems
{
    /// <summary>
    /// The measurement of the amount of time a piece of equipment or a sub-part of a piece of equipment has performed specific activities.
    /// </summary>
    public class EquipmentTimerModel
    {
        /// <summary>
        /// Measurement of the time that the sub-parts of a piece of equipment are under load. 
        /// </summary>
        public EquipmentTimerValue Loaded { get; set; }
        public IDataItemModel LoadedDataItem { get; set; }

        /// <summary>
        /// Measurement of the time that a piece of equipment is performing any activity the equipment is active and performing a function under load or not.
        /// </summary>
        public EquipmentTimerValue Working { get; set; }
        public IDataItemModel WorkingDataItem { get; set; }

        /// <summary>
        /// Measurement of the time that the major sub-parts of a piece of equipment are powered or performing any activity whether producing a part or product or not.
        /// </summary>
        public EquipmentTimerValue Operating { get; set; }
        public IDataItemModel OperatingDataItem { get; set; }

        /// <summary>
        /// The measurement of time that primary power is applied to the piece of equipment and, as a minimum, 
        /// the controller or logic portion of the piece of equipment is powered and functioning or components that are required to remain on are powered.
        /// </summary>
        public EquipmentTimerValue Powered { get; set; }
        public IDataItemModel PoweredDataItem { get; set; }

        /// <summary>
        /// The elapsed time of a temporary halt of action.
        /// </summary>
        public EquipmentTimerValue Delay { get; set; }
        public IDataItemModel DelayDataItem { get; set; }
    }
}