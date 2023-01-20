// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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