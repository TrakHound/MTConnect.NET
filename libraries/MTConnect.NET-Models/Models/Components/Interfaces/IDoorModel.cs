// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Observations.Events.Values;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// Door is a Component that represents the information for a mechanical mechanism or closure that can cover, for example, a physical access portal into a piece of equipment.
    /// The closure can be opened or closed to allow or restrict access to other parts of the equipment.
    /// </summary>
    public interface IDoorModel
    {
        /// <summary>
        /// The operational state of a DOOR type component or composition element.
        /// </summary>
        DoorState State { get; set; }
    }
}