// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Observations.Events.Values;

namespace MTConnect.Models.Compositions
{
    /// <summary>
    /// A mechanical mechanism or closure that can cover a physical access portal into a piece of equipment allowing or restricting access to other parts of the equipment.
    /// </summary>
    public interface IDoorModel : ICompositionModel
    {
        /// <summary>
        /// The operational state of a DOOR type component or composition element.
        /// </summary>
        DoorState DoorState { get; set; }
    }
}