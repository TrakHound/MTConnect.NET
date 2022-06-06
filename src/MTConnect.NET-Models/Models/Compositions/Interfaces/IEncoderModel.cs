// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

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
        Observations.ConditionObservation SystemCondition { get; set; }

        /// <summary>
        /// An indication of a fault associated with the hardware subsystem of the Structural Element.
        /// </summary>
        Observations.ConditionObservation HardwareCondition { get; set; }

        /// <summary>
        /// An indication that the piece of equipment has experienced a communications failure.
        /// </summary>
        Observations.ConditionObservation CommunicationsCondition { get; set; }
    }
}
