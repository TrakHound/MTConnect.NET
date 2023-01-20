// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

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
