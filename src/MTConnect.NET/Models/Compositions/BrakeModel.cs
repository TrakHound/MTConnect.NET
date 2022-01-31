// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.Compositions;

namespace MTConnect.Models.Compositions
{
    /// <summary>
    /// A mechanism for slowing or stopping a moving object by the absorption or
    /// transfer of the energy of momentum, usually by means of friction, electrical force, or magnetic force.
    /// </summary>
    public class BrakeModel : CompositionModel, IBrakeModel
    {
        public BrakeModel() 
        {
            Type = BrakeComposition.TypeId;
        }

        public BrakeModel(string compositionId)
        {
            Id = compositionId;
            Type = BrakeComposition.TypeId;
        }
    }
}
