// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// Enclosure is a System that represents the information for a structure used to contain or isolate a piece of equipment or area. 
    /// The Enclosure system may provide information regarding access to the internal components of a piece of equipment or the conditions within the enclosure.
    /// </summary>
    public interface IEnclosureModel : ISystemModel
    {
        /// <summary>
        /// Door is a Component that represents the information for a mechanical mechanism or closure that can cover, for example, a physical access portal into a piece of equipment.
        /// The closure can be opened or closed to allow or restrict access to other parts of the equipment.
        /// </summary>
        IEnumerable<DoorModel> Doors { get; }


        /// <summary>
        /// Get the Door Component Model with the specified Name
        /// </summary>
        IDoorModel GetDoor(string name);
    }
}