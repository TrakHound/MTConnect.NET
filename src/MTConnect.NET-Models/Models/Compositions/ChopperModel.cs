// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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