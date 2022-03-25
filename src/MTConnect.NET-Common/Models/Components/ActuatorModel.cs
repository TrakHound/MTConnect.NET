// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

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
