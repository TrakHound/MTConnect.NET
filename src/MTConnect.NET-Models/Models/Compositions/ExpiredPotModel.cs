// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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