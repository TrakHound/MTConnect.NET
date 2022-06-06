// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

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
