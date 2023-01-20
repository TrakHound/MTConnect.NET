// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Components;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// Interfaces organizes Interface component types.
    /// </summary>
    class InterfacesModel : ComponentModel
    {
        InterfacesModel() 
        {
            Type = InterfacesComponent.TypeId;
        }

        InterfacesModel(string componentId)
        {
            Id = componentId;
            Type = InterfacesComponent.TypeId;
        }
    }
}