// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Compositions;

namespace MTConnect.Models.Compositions
{
    /// <summary>
    /// A component consisting of one or more cells, in which chemical energy is converted into electricity and used as a source of power.
    /// </summary>
    public class StorageBatteryModel : CompositionModel, IStorageBatteryModel
    {
        public StorageBatteryModel() 
        {
            Type = StorageBatteryComposition.TypeId;
        }

        public StorageBatteryModel(string compositionId)
        {
            Id = compositionId;
            Type = StorageBatteryComposition.TypeId;
        }
    }
}