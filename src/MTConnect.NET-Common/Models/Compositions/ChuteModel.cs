// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

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
