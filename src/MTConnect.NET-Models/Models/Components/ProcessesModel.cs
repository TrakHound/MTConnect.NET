// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.Components;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// Processes organizes information describing the manufacturing process being executed on a piece of equipment.
    /// </summary>
    public class ProcessesModel : ComponentModel
    {
        public ProcessesModel() 
        {
            Type = ProcessesComponent.TypeId;
        }

        public ProcessesModel(string componentId)
        {
            Id = componentId;
            Type = ProcessesComponent.TypeId;
        }
    }
}
