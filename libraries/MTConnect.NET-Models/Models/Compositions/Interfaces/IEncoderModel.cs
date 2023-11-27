// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Models.Compositions
{
    /// <summary>
    /// A mechanism to measure position.
    /// </summary>
    public interface IEncoderModel : ICompositionModel
    {
        /// <summary>
        /// An indication of a fault associated with a piece of equipment or component that cannot be classified as a specific type.
        /// </summary>
        Observations.IConditionObservation SystemCondition { get; set; }

        /// <summary>
        /// An indication of a fault associated with the hardware subsystem of the Structural Element.
        /// </summary>
        Observations.IConditionObservation HardwareCondition { get; set; }

        /// <summary>
        /// An indication that the piece of equipment has experienced a communications failure.
        /// </summary>
        Observations.IConditionObservation CommunicationsCondition { get; set; }
    }
}