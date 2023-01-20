// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.Components;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// Resources organizes Resource component types.
    /// </summary>
    public class ResourcesModel : ComponentModel
    {
        public ResourcesModel() 
        {
            Type = ResourcesComponent.TypeId;
        }

        public ResourcesModel(string componentId)
        {
            Id = componentId;
            Type = ResourcesComponent.TypeId;
        }
    }
}
