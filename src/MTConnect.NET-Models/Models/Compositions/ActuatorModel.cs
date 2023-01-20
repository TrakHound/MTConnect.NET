// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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