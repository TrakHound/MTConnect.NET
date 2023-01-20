// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.Compositions;

namespace MTConnect.Models.Compositions
{
    /// <summary>
    /// A mechanism for moving or controlling a mechanical part of a piece of equipment.
    /// </summary>
    public class ActuatorModel : CompositionModel, IActuatorModel
    {
        public ActuatorModel() 
        {
            Type = ActuatorComposition.TypeId;
        }

        public ActuatorModel(string compositionId)
        {
            Id = compositionId;
            Type = ActuatorComposition.TypeId;
        }
    }
}
