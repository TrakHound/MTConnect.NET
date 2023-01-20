// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.Compositions;

namespace MTConnect.Models.Compositions
{
    /// <summary>
    /// A mechanism for emitting a type of radiation.
    /// </summary>
    public class ExposureUnitModel : CompositionModel, IExposureUnitModel
    {
        public ExposureUnitModel() 
        {
            Type = ExposureUnitComposition.TypeId;
        }

        public ExposureUnitModel(string compositionId)
        {
            Id = compositionId;
            Type = ExposureUnitComposition.TypeId;
        }
    }
}
