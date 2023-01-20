// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.Components;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// Interface is a Component that coordinates actions and activities between pieces of equipment.
    /// </summary>
    class InterfaceModel : ComponentModel
    {
        InterfaceModel() 
        {
            Type = InterfaceComponent.TypeId;
        }

        InterfaceModel(string componentId)
        {
            Id = componentId;
            Type = InterfaceComponent.TypeId;
        }
    }
}
