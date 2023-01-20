// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.Components;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// ProcessPower is a System that represents the information for a power source associated with a piece of equipment that supplies energy to the manufacturing process 
    /// separate from the Electric system.For example, this could be the power source for an EDM machining process, an electroplating line, or a welding system.
    /// </summary>
    public class ProcessPowerModel : SystemModel, IProcessPowerModel
    {
        public ProcessPowerModel()
        {
            Type = ProcessPowerComponent.TypeId;
        }

        public ProcessPowerModel(string componentId)
        {
            Id = componentId;
            Type = ProcessPowerComponent.TypeId;
        }
    }
}
