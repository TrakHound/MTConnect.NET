// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

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
