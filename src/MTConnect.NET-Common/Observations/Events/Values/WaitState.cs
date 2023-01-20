// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events.Values
{
    /// <summary>
    /// An indication of the reason that EXECUTION is reporting a value of WAIT.
    /// </summary>
    public enum WaitState
    {
        /// <summary>
        /// (Not part of the MTConnect Standard) No Wait State is Specified
        /// </summary>
        NOT_SPECIFIED,

        /// <summary>
        /// An indication that execution is waiting while the equipment is powering up and is not currently available to begin producing parts or products.
        /// </summary>
        POWERING_UP,

        /// <summary>
        /// An indication that the execution is waiting while the equipment is powering down but has not fully reached a stopped state.
        /// </summary>
        POWERING_DOWN,

        /// <summary>
        /// An indication that the execution is waiting while one or more discrete workpieces are being loaded.
        /// </summary>
        PART_LOAD,

        /// <summary>
        /// An indication that the execution is waiting while one or more discrete workpieces are being unloaded.
        /// </summary>
        PART_UNLOAD,

        /// <summary>
        /// An indication that the execution is waiting while a tool or tooling is being loaded.
        /// </summary>
        TOOL_LOAD,

        /// <summary>
        /// An indication that the execution is waiting while a tool or tooling is being unloaded.
        /// </summary>
        TOOL_UNLOAD,

        /// <summary>
        /// An indication that the execution is waiting while bulk material or the container for bulk material used in the production process is being loaded. 
        /// Bulk material includes those materials from which multiple workpieces may be created.
        /// </summary>
        MATERIAL_LOAD,

        /// <summary>
        /// An indication that the execution is waiting while bulk material or the container for bulk material used in the production process is being unloaded. 
        /// Bulk material includes those materials from which multiple workpieces may be created.
        /// </summary>
        MATERIAL_UNLOAD,

        /// <summary>
        /// An indication that the execution is waiting while another process is completed before the execution can resume.
        /// </summary>
        SECONDARY_PROCESS,

        /// <summary>
        /// An indication that the execution is waiting while the equipment is pausing but the piece of equipment has not yet reached a fully paused state.
        /// </summary>
        PAUSING,

        /// <summary>
        /// An indication that the execution is waiting while the equipment is resuming the production cycle but has not yet resumed execution.
        /// </summary>
        RESUMING
    }
}