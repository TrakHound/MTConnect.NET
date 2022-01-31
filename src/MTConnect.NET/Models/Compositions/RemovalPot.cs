// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.Compositions;

namespace MTConnect.Models.Compositions
{
    /// <summary>
    /// A POT for a tool to be removed from a ToolMagazine or Turret to a location outside of the piece of equipment.
    /// </summary>
    public class RemovalPotModel : CompositionModel, IRemovalPotModel
    {
        public RemovalPotModel() 
        {
            Type = RemovalPotComposition.TypeId;
        }

        public RemovalPotModel(string compositionId)
        {
            Id = compositionId;
            Type = RemovalPotComposition.TypeId;
        }
    }
}
