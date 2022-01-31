// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.Compositions;

namespace MTConnect.Models.Compositions
{
    /// <summary>
    /// A mechanism for turning on or off an electric current or for making or breaking a circuit.
    /// </summary>
    public class SwitchModel : CompositionModel, ISwitchModel
    {
        public SwitchModel() 
        {
            Type = SwitchComposition.TypeId;
        }

        public SwitchModel(string compositionId)
        {
            Id = compositionId;
            Type = SwitchComposition.TypeId;
        }
    }
}
