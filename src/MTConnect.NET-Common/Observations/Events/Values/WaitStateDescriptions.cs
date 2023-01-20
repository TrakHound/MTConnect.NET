// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events.Values
{
    /// <summary>
    /// An indication of the reason that EXECUTION is reporting a value of WAIT.
    /// </summary>
    public static class WaitStateDescriptions
    {
        /// <summary>
        /// An indication that execution is waiting while the equipment is powering up and is not currently available to begin producing parts or products.
        /// </summary>
        public const string POWERING_UP = "An indication that execution is waiting while the equipment is powering up and is not currently available to begin producing parts or products.";

        /// <summary>
        /// An indication that the execution is waiting while the equipment is powering down but has not fully reached a stopped state.
        /// </summary>
        public const string POWERING_DOWN = "An indication that the execution is waiting while the equipment is powering down but has not fully reached a stopped state.";

        /// <summary>
        /// An indication that the execution is waiting while one or more discrete workpieces are being loaded.
        /// </summary>
        public const string PART_LOAD = "An indication that the execution is waiting while one or more discrete workpieces are being loaded.";

        /// <summary>
        /// An indication that the execution is waiting while one or more discrete workpieces are being unloaded.
        /// </summary>
        public const string PART_UNLOAD = "An indication that the execution is waiting while one or more discrete workpieces are being unloaded.";

        /// <summary>
        /// An indication that the execution is waiting while a tool or tooling is being loaded.
        /// </summary>
        public const string TOOL_LOAD = "An indication that the execution is waiting while a tool or tooling is being loaded.";

        /// <summary>
        /// An indication that the execution is waiting while a tool or tooling is being unloaded.
        /// </summary>
        public const string TOOL_UNLOAD = "An indication that the execution is waiting while a tool or tooling is being unloaded.";

        /// <summary>
        /// An indication that the execution is waiting while bulk material or the container for bulk material used in the production process is being loaded. 
        /// Bulk material includes those materials from which multiple workpieces may be created.
        /// </summary>
        public const string MATERIAL_LOAD = "An indication that the execution is waiting while bulk material or the container for bulk material used in the production process is being loaded. Bulk material includes those materials from which multiple workpieces may be created.";

        /// <summary>
        /// An indication that the execution is waiting while bulk material or the container for bulk material used in the production process is being unloaded. 
        /// Bulk material includes those materials from which multiple workpieces may be created.
        /// </summary>
        public const string MATERIAL_UNLOAD = "An indication that the execution is waiting while bulk material or the container for bulk material used in the production process is being unloaded. Bulk material includes those materials from which multiple workpieces may be created.";

        /// <summary>
        /// An indication that the execution is waiting while another process is completed before the execution can resume.
        /// </summary>
        public const string SECONDARY_PROCESS = "An indication that the execution is waiting while another process is completed before the execution can resume.";

        /// <summary>
        /// An indication that the execution is waiting while the equipment is pausing but the piece of equipment has not yet reached a fully paused state.
        /// </summary>
        public const string PAUSING = "An indication that the execution is waiting while the equipment is pausing but the piece of equipment has not yet reached a fully paused state.";

        /// <summary>
        /// An indication that the execution is waiting while the equipment is resuming the production cycle but has not yet resumed execution.
        /// </summary>
        public const string RESUMING = "An indication that the execution is waiting while the equipment is resuming the production cycle but has not yet resumed execution.";


        public static string Get(WaitState value)
        {
            switch (value)
            {
                case WaitState.POWERING_UP: return POWERING_UP;
                case WaitState.POWERING_DOWN: return POWERING_DOWN;
                case WaitState.PART_LOAD: return PART_LOAD;
                case WaitState.PART_UNLOAD: return PART_UNLOAD;
                case WaitState.TOOL_LOAD: return TOOL_LOAD;
                case WaitState.TOOL_UNLOAD: return TOOL_UNLOAD;
                case WaitState.MATERIAL_LOAD: return MATERIAL_LOAD;
                case WaitState.MATERIAL_UNLOAD: return MATERIAL_UNLOAD;
                case WaitState.SECONDARY_PROCESS: return SECONDARY_PROCESS;
                case WaitState.PAUSING: return PAUSING;
                case WaitState.RESUMING: return RESUMING;
            }

            return null;
        }
    }
}