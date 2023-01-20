// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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