// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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