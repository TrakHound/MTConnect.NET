// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Models.Components
{
    /// <summary>
    /// System is an abstract Component that represents part(s) of a piece of equipment that is permanently integrated into the piece of equipment.
    /// </summary>
    public interface ISystemModel : IComponentModel
    {
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

        /// <summary>
        /// Any text string of information to be transferred from a piece of equipment to a client software application.
        /// </summary>
        string Message { get; set; }
    }
}
