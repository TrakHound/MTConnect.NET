// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

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
