// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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