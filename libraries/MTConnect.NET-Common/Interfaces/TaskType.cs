// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Interfaces
{
    /// <summary>
    /// Classifies the kind of work an Interface task or task archetype performs.
    /// </summary>
    public enum TaskType
    {
        /// <summary>
        /// Remove finished or in-process material from a piece of equipment.
        /// </summary>
        MATERIAL_UNLOAD,

        /// <summary>
        /// Transport material between pieces of equipment or staging locations.
        /// </summary>
        MOVE_MATERIAL,

        /// <summary>
        /// Exchange the active tool on a piece of equipment.
        /// </summary>
        TOOL_CHANGE
    }
}
