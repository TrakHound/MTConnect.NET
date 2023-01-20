// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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