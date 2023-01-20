// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Compositions;

namespace MTConnect.Models.Compositions
{
    /// <summary>
    /// A unit that provides power to electric mechanisms.
    /// </summary>
    public class PowerSupplyModel : CompositionModel, IPowerSupplyModel
    {
        public PowerSupplyModel() 
        {
            Type = PowerSupplyComposition.TypeId;
        }

        public PowerSupplyModel(string compositionId)
        {
            Id = compositionId;
            Type = PowerSupplyComposition.TypeId;
        }
    }
}