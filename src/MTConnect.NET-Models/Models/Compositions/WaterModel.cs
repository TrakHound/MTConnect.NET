// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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