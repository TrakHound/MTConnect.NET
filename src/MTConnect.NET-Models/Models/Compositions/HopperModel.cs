// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Compositions;

namespace MTConnect.Models.Compositions
{
    /// <summary>
    /// A chamber or bin in which materials are stored temporarily, typically being filled through the top and dispensed through the bottom.
    /// </summary>
    public class HopperModel : CompositionModel, IHopperModel
    {
        public HopperModel() 
        {
            Type = HopperComposition.TypeId;
        }

        public HopperModel(string compositionId)
        {
            Id = compositionId;
            Type = HopperComposition.TypeId;
        }
    }
}