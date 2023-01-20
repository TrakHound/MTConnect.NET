// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Components;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// Pressure is a System that delivers compressed gas or fluid and controls the pressure and rate of pressure change to a desired target set-point.
    /// </summary>
    public class PressureModel : SystemModel, IPressureModel
    {
        public PressureModel()
        {
            Type = PressureComponent.TypeId;
        }

        public PressureModel(string componentId)
        {
            Id = componentId;
            Type = PressureComponent.TypeId;
        }
    }
}