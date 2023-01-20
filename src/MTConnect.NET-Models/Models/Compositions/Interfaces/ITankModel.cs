// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Observations.Samples.Values;

namespace MTConnect.Models.Compositions
{
    /// <summary>
    /// A receptacle or container for holding material.
    /// </summary>
    public interface ITankModel : ICompositionModel
    {
        /// <summary>
        /// The fluid capacity of an object or container.
        /// </summary>
        CapacityFluidValue CapacityFluid { get; set; }
        IDataItemModel CapacityFluidDataItem { get; }

        /// <summary>
        /// The measurement of the amount of a substance remaining compared to the planned maximum amount of that substance.
        /// </summary>
        FillLevelValue FillLevel { get; set; }
        IDataItemModel FillLevelDataItem { get; }

        /// <summary>
        /// An indication of a fault associated with a piece of equipment or component that cannot be classified as a specific type.
        /// </summary>
        Observations.IConditionObservation SystemCondition { get; set; }
        IDataItemModel SystemConditionDataItem { get; }

        /// <summary>
        /// An indication of a fault associated with the hardware subsystem of the Structural Element.
        /// </summary>
        Observations.IConditionObservation HardwareCondition { get; set; }
        IDataItemModel HardwareConditionDataItem { get; }

        /// <summary>
        /// An indication that the piece of equipment has experienced a communications failure.
        /// </summary>
        Observations.IConditionObservation CommunicationsCondition { get; set; }
        IDataItemModel CommunicationsConditionDataItem { get; }
    }
}