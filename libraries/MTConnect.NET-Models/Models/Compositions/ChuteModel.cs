// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Compositions;

namespace MTConnect.Models.Compositions
{
    /// <summary>
    /// An inclined channel for conveying material.
    /// </summary>
    public class ChuteModel : CompositionModel, IChuteModel
    {
        public ChuteModel() 
        {
            Type = ChuteComposition.TypeId;
        }

        public ChuteModel(string compositionId)
        {
            Id = compositionId;
            Type = ChuteComposition.TypeId;
        }
    }
}