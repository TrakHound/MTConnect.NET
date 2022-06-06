// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.Compositions;

namespace MTConnect.Models.Compositions
{
    /// <summary>
    /// A POT for a tool that is no longer useable for removal from a ToolMagazine or Turret.
    /// </summary>
    public class ExpiredPotModel : CompositionModel, IExpiredPotModel
    {
        public ExpiredPotModel() 
        {
            Type = ExpiredPotComposition.TypeId;
        }

        public ExpiredPotModel(string compositionId)
        {
            Id = compositionId;
            Type = ExpiredPotComposition.TypeId;
        }
    }
}
