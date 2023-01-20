// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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