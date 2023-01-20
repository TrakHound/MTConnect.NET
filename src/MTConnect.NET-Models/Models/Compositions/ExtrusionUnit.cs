// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.Compositions;

namespace MTConnect.Models.Compositions
{
    /// <summary>
    /// A mechanism for dispensing liquid or powered materials.
    /// </summary>
    public class ExtrusionUnitModel : CompositionModel, IExtrusionUnitModel
    {
        public ExtrusionUnitModel() 
        {
            Type = ExtrusionUnitComposition.TypeId;
        }

        public ExtrusionUnitModel(string compositionId)
        {
            Id = compositionId;
            Type = ExtrusionUnitComposition.TypeId;
        }
    }
}
