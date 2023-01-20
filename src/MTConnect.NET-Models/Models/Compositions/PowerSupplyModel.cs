// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

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
