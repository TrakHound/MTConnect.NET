// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Components;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// Actuator is a Component that represents the information for an apparatus for moving or controlling a mechanism or system.
    /// It takes energy usually provided by air, electric current, or liquid and converts the energy into some kind of motion.
    /// </summary>
    public class ActuatorModel : ComponentModel
    {
        public ActuatorModel() 
        {
            Type = ActuatorComponent.TypeId;
        }

        public ActuatorModel(string componentId)
        {
            Id = componentId;
            Type = ActuatorComponent.TypeId;
        }
    }
}