// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.Components;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// Structures organizes Structure component types.
    /// </summary>
    public class StructuresModel : ComponentModel
    {
        public StructuresModel() 
        {
            Type = StructuresComponent.TypeId;
        }

        public StructuresModel(string componentId)
        {
            Id = componentId;
            Type = StructuresComponent.TypeId;
        }
    }
}
