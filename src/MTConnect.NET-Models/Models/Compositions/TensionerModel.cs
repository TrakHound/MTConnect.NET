// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Compositions;

namespace MTConnect.Models.Compositions
{
    /// <summary>
    /// A mechanism that provides or applies a stretch or strain to another mechanism.
    /// </summary>
    public class TensionerModel : CompositionModel, ITensionerModel
    {
        public TensionerModel() 
        {
            Type = TensionerComposition.TypeId;
        }

        public TensionerModel(string compositionId)
        {
            Id = compositionId;
            Type = TensionerComposition.TypeId;
        }
    }
}