// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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