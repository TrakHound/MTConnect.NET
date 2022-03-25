// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.Components;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// Sensor is is an Auxiliary that represents the information for a piece of equipment that responds to a physical stimulus and transmits a resulting impulse or value from a sensing unit.
    /// </summary>
    public class SensorModel : EnvironmentalModel
    {
        public SensorModel()
        {
            Type = SensorComponent.TypeId;
        }

        public SensorModel(string componentId)
        {
            Id = componentId;
            Type = SensorComponent.TypeId;
        }
    }
}
