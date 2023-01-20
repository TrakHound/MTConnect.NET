// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.Compositions;

namespace MTConnect.Models.Compositions
{
    /// <summary>
    /// A mechanism that holds a part, stock material, or any other item in place.
    /// </summary>
    public class GripperModel : CompositionModel, IGripperModel
    {
        public GripperModel() 
        {
            Type = GripperComposition.TypeId;
        }

        public GripperModel(string compositionId)
        {
            Id = compositionId;
            Type = GripperComposition.TypeId;
        }
    }
}
