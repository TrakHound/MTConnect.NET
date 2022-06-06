// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.Compositions;

namespace MTConnect.Models.Compositions
{
    /// <summary>
    /// A mechanism used to strengthen, support, or fasten objects in place.
    /// </summary>
    public class ClampModel : CompositionModel, IClampModel
    {
        public ClampModel() 
        {
            Type = ClampComposition.TypeId;
        }

        public ClampModel(string compositionId)
        {
            Id = compositionId;
            Type = ClampComposition.TypeId;
        }
    }
}
