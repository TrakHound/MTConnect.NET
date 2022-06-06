// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.Components;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// Structure is a Component that represents the part(s) comprising the rigid bodies of the piece of equipment.
    /// </summary>
    public class StructureModel : ComponentModel
    {
        public StructureModel() 
        {
            Type = StructuresComponent.TypeId;
        }

        public StructureModel(string componentId)
        {
            Id = componentId;
            Type = StructuresComponent.TypeId;
        }
    }
}
