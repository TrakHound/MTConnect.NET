// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Components;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// Heating is a System used to deliver controlled amounts of heat to achieve a target temperature at a specified heating rate.
    /// </summary>
    public class HeatingModel : SystemModel, IHeatingModel
    {
        public HeatingModel()
        {
            Type = HeatingComponent.TypeId;
        }

        public HeatingModel(string componentId)
        {
            Id = componentId;
            Type = HeatingComponent.TypeId;
        }
    }
}