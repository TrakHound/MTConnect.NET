// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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