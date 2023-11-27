// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Models.Compositions;
using System.Collections.Generic;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// ToolMagazine is a ToolingDelivery that represents a tool storage mechanism that holds any number of tools.Tools are located in POTs.
    /// POTs are moved into position to transfer tools into or out of the ToolMagazine by an AutomaticToolChanger.
    /// </summary>
    public interface IToolMagazineModel : IAuxiliaryModel
    {
        /// <summary>
        /// A POT for a tool awaiting transfer to a ToolMagazine or Turret from outside of the piece of equipment.
        /// </summary>
        IStagingPotModel StagingPot { get; set; }

        /// <summary>
        /// Tool storage locations.
        /// </summary>
        IEnumerable<IPotModel> Pots { get; }

        /// <summary>
        /// Enclosure is a System that represents the information for a structure used to contain or isolate a piece of equipment or area. 
        /// The Enclosure system may provide information regarding access to the internal components of a piece of equipment or the conditions within the enclosure.
        /// </summary>
        IEnclosureModel Enclosure { get; }


        /// <summary>
        /// Gets the Pot with the specified Pot Number
        /// (if doesn't exist then it will be created)
        /// </summary>
        IPotModel GetPot(int potNumber);
    }
}