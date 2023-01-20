// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

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
