// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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