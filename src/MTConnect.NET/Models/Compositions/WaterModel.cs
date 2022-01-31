// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.Compositions;

namespace MTConnect.Models.Compositions
{
    /// <summary>
    /// A fluid.
    /// </summary>
    public class WaterModel : CompositionModel, IWaterModel
    {
        public WaterModel() 
        {
            Type = WaterComposition.TypeId;
        }

        public WaterModel(string compositionId)
        {
            Id = compositionId;
            Type = WaterComposition.TypeId;
        }
    }
}
