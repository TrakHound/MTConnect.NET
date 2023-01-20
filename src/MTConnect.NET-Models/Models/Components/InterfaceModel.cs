// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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