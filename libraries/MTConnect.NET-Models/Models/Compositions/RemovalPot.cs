// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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