// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

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
