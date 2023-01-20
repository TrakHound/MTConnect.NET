// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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