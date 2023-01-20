// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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