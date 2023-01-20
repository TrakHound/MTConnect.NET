// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Compositions;

namespace MTConnect.Models.Compositions
{
    /// <summary>
    /// A string like piece or filament of relatively rigid or flexible material provided in a variety of diameters.
    /// </summary>
    public class WireModel : CompositionModel, IWireModel
    {
        public WireModel() 
        {
            Type = WireComposition.TypeId;
        }

        public WireModel(string compositionId)
        {
            Id = compositionId;
            Type = WireComposition.TypeId;
        }
    }
}