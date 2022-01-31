// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

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
