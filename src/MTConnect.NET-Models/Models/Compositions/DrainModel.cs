// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Compositions;

namespace MTConnect.Models.Compositions
{
    /// <summary>
    /// A mechanism that allows material to flow for the purpose of drainage from, for example, a vessel or tank.
    /// </summary>
    public class DrainModel : CompositionModel, IDrainModel
    {
        public DrainModel() 
        {
            Type = DrainComposition.TypeId;
        }

        public DrainModel(string compositionId)
        {
            Id = compositionId;
            Type = DrainComposition.TypeId;
        }
    }
}