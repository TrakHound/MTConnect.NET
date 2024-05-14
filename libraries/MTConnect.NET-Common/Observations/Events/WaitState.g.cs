// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// Indication of the reason that Execution is reporting a value of `WAIT`.
    /// </summary>
    public enum WaitState
    {
        /// <summary>
        /// Execution is waiting while the equipment is powering up and is not currently available to begin producing parts or products.
        /// </summary>
        POWERING_UP,
        
        /// <summary>
        /// Execution is waiting while the equipment is powering down but has not fully reached a stopped state.
        /// </summary>
        POWERING_DOWN,
        
        /// <summary>
        /// Execution is waiting while one or more discrete workpieces are being loaded.
        /// </summary>
        PART_LOAD,
        
        /// <summary>
        /// Execution is waiting while one or more discrete workpieces are being unloaded.
        /// </summary>
        PART_UNLOAD,
        
        /// <summary>
        /// Execution is waiting while a tool or tooling is being loaded.
        /// </summary>
        TOOL_LOAD,
        
        /// <summary>
        /// Execution is waiting while a tool or tooling is being unloaded.
        /// </summary>
        TOOL_UNLOAD,
        
        /// <summary>
        /// Execution is waiting while material is being loaded.
        /// </summary>
        MATERIAL_LOAD,
        
        /// <summary>
        /// Execution is waiting while material is being unloaded.
        /// </summary>
        MATERIAL_UNLOAD,
        
        /// <summary>
        /// Execution is waiting while another process is completed before the execution can resume.
        /// </summary>
        SECONDARY_PROCESS,
        
        /// <summary>
        /// Execution is waiting while the equipment is pausing but the piece of equipment has not yet reached a fully paused state.
        /// </summary>
        PAUSING,
        
        /// <summary>
        /// Execution is waiting while the equipment is resuming the production cycle but has not yet resumed execution.
        /// </summary>
        RESUMING
    }
}