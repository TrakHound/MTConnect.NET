// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Compositions;

namespace MTConnect.Models.Compositions
{
    /// <summary>
    /// A mechanism or wheel that turns in a frame or block and serves to change the direction of or to transmit force.
    /// </summary>
    public class PulleyModel : CompositionModel, IPulleyModel
    {
        public PulleyModel() 
        {
            Type = PulleyComposition.TypeId;
        }

        public PulleyModel(string compositionId)
        {
            Id = compositionId;
            Type = PulleyComposition.TypeId;
        }
    }
}