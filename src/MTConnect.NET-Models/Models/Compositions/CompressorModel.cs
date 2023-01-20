// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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