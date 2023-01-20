// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Components;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// Parts organizes information for Parts being processed by a piece of equipment.
    /// </summary>
    public class PartsModel : ComponentModel
    {
        public PartsModel() 
        {
            Type = PartsComponent.TypeId;
        }

        public PartsModel(string componentId)
        {
            Id = componentId;
            Type = PartsComponent.TypeId;
        }
    }
}