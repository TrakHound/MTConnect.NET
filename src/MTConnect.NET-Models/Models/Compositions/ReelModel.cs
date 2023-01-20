// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Compositions;

namespace MTConnect.Models.Compositions
{
    /// <summary>
    /// A rotary storage unit for material.
    /// </summary>
    public class ReelModel : CompositionModel, IReelModel
    {
        public ReelModel() 
        {
            Type = ReelComposition.TypeId;
        }

        public ReelModel(string compositionId)
        {
            Id = compositionId;
            Type = ReelComposition.TypeId;
        }
    }
}