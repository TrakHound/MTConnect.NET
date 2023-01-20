// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Models.DataItems;
using MTConnect.Observations;
using MTConnect.Observations.Samples.Values;

namespace MTConnect.Models.Compositions
{
    /// <summary>
    /// A viscous liquid.
    /// </summary>
    public interface IOilModel : ICompositionModel
    {
        /// <summary>
        /// The measurement of temperature.
        /// </summary>
        TemperatureValue Temperature { get; set; }
        IDataItemModel TemperatureDataItem { get; }

        /// <summary>
        /// The force per unit area measured relative to atmospheric pressure.
        /// </summary>
        PressureValue Pressure { get; set; }
        IDataItemModel PressureDataItem { get; }

        /// <summary>
        /// The measurement of a fluids resistance to flow
        /// </summary>
        ViscosityValue Viscosity { get; set; }
        IDataItemModel ViscosityDataItem { get; }

        /// <summary>
        /// The measurement of the percentage of one component within a mixture of components
        /// </summary>
        ConcentrationValue Concentration { get; set; }
        IDataItemModel ConcentrationDataItem { get; }

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
        IConditionObservation SystemCondition { get; set; }
        IDataItemModel SystemConditionDataItem { get; }

        /// <summary>
        /// An indication of a fault associated with the hardware subsystem of the Structural Element.
        /// </summary>
        IConditionObservation HardwareCondition { get; set; }
        IDataItemModel HardwareConditionDataItem { get; }

        /// <summary>
        /// An indication that the piece of equipment has experienced a communications failure.
        /// </summary>
        IConditionObservation CommunicationsCondition { get; set; }
        IDataItemModel CommunicationsConditionDataItem { get; }
    }
}
