// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Models.DataItems;
using MTConnect.Observations.Samples.Values;

namespace MTConnect.Models.Compositions
{
    /// <summary>
    /// A mechanism that converts electrical, pneumatic, or hydraulic energy into mechanical energy.
    /// </summary>
    public interface IMotorModel : ICompositionModel
    {
        /// <summary>
        /// The measurement of the actual versus the standard rating of a piece of equipment.
        /// </summary>
        LoadValue Load { get; set; }
        IDataItemModel LoadDataItem { get; }

        /// <summary>
        /// The measurement of temperature.
        /// </summary>
        TemperatureValue Temperature { get; set; }
        IDataItemModel TemperatureDataItem { get; }

        /// <summary>
        /// The measurement of an electrical current
        /// </summary>
        AmperageACModel AmperageAC { get; set; }

        /// <summary>
        /// The measurement of an electrical current
        /// </summary>
        AmperageDCModel AmperageDC { get; set; }

        /// <summary>
        /// The measurement of accumulated time for an activity or event.
        /// </summary>
        AccumulatedTimeValue AccumulatedTime { get; set; }
        IDataItemModel AccumulatedTimeDataItem { get; }

        /// <summary>
        /// The time and date code associated with a material or other physical item.
        /// </summary>
        DateCodeModel DateCode { get; set; }

        /// <summary>
        /// An indication of a fault associated with a piece of equipment or component that cannot be classified as a specific type.
        /// </summary>
        Observations.ConditionObservation SystemCondition { get; set; }
        IDataItemModel SystemConditionDataItem { get; }

        /// <summary>
        /// An indication of a fault associated with the hardware subsystem of the Structural Element.
        /// </summary>
        Observations.ConditionObservation HardwareCondition { get; set; }
        IDataItemModel HardwareConditionDataItem { get; }

        /// <summary>
        /// An indication that the piece of equipment has experienced a communications failure.
        /// </summary>
        Observations.ConditionObservation CommunicationsCondition { get; set; }
        IDataItemModel CommunicationsConditionDataItem { get; }
    }
}
