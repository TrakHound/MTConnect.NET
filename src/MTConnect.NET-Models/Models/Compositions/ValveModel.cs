// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.Compositions;

namespace MTConnect.Models.Compositions
{
    /// <summary>
    /// Any mechanism for halting or controlling the flow of a liquid, gas, or other material through a passage, pipe, inlet, or outlet.
    /// </summary>
    public class ValveModel : CompositionModel, IValveModel
    {
        public ValveModel() 
        {
            Type = ValveComposition.TypeId;
        }

        public ValveModel(string compositionId)
        {
            Id = compositionId;
            Type = ValveComposition.TypeId;
        }
    }
}
