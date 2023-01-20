// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.Compositions;

namespace MTConnect.Models.Compositions
{
    /// <summary>
    /// A mechanism that provides a signal or measured value.
    /// </summary>
    public class SensingElementModel : CompositionModel, ISensingElementModel
    {
        public SensingElementModel() 
        {
            Type = SensingElementComposition.TypeId;
        }

        public SensingElementModel(string compositionId)
        {
            Id = compositionId;
            Type = SensingElementComposition.TypeId;
        }
    }
}
