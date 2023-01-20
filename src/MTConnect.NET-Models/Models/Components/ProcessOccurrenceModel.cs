// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Components;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// ProcessOccurrence is a Component that organizesinformation about the execution of a specific process
    /// that takes place at a specific place and time, such as a specific instance of part-milling occurring at a specific timestamp.
    /// </summary>
    public class ProcessOccurrenceModel : ComponentModel
    {
        public ProcessOccurrenceModel() 
        {
            Type = ProcessOccurrenceComponent.TypeId;
        }

        public ProcessOccurrenceModel(string componentId)
        {
            Id = componentId;
            Type = ProcessOccurrenceComponent.TypeId;
        }
    }
}