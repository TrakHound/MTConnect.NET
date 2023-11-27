// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Compositions;

namespace MTConnect.Models.Compositions
{
    /// <summary>
    /// A heat exchange system that uses a fluid to transfer heat to the atmosphere.
    /// </summary>
    public class CoolingTowerModel : CompositionModel, ICoolingTowerModel
    {
        public CoolingTowerModel() 
        {
            Type = CoolingTowerComposition.TypeId;
        }

        public CoolingTowerModel(string compositionId)
        {
            Id = compositionId;
            Type = CoolingTowerComposition.TypeId;
        }
    }
}