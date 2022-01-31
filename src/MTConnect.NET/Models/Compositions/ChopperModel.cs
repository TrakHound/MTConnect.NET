// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.Compositions;

namespace MTConnect.Models.Compositions
{
    /// <summary>
    /// A mechanism used to break material into smaller pieces.
    /// </summary>
    public class ChopperModel : CompositionModel, IChopperModel
    {
        public ChopperModel() 
        {
            Type = ChopperComposition.TypeId;
        }

        public ChopperModel(string compositionId)
        {
            Id = compositionId;
            Type = ChopperComposition.TypeId;
        }
    }
}
