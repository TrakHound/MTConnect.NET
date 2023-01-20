// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Compositions;

namespace MTConnect.Models.Compositions
{
    /// <summary>
    /// Any mechanism for producing a current of air.
    /// </summary>
    public class FanModel : CompositionModel, IFanModel
    {
        public FanModel() 
        {
            Type = FanComposition.TypeId;
        }

        public FanModel(string compositionId)
        {
            Id = compositionId;
            Type = FanComposition.TypeId;
        }
    }
}