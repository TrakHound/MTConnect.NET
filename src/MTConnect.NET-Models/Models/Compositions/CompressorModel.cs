// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.Compositions;

namespace MTConnect.Models.Compositions
{
    /// <summary>
    /// A pump or other mechanism for reducing volume and increasing pressure of gases
    /// in order to condense the gases to drive pneumatically powered pieces of equipment.
    /// </summary>
    public class CompressorModel : CompositionModel, ICompressorModel
    {
        public CompressorModel() 
        {
            Type = CompressorComposition.TypeId;
        }

        public CompressorModel(string compositionId)
        {
            Id = compositionId;
            Type = CompressorComposition.TypeId;
        }
    }
}
