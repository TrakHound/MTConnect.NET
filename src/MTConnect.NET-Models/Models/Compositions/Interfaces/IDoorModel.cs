// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

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
