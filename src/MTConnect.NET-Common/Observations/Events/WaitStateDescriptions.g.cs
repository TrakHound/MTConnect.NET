// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    public static class WaitStateDescriptions
    {
        /// <summary>
        /// Execution is waiting while the equipment is powering up and is not currently available to begin producing parts or products.
        /// </summary>
        public const string POWERING_UP = "Execution is waiting while the equipment is powering up and is not currently available to begin producing parts or products.";
        
        /// <summary>
        /// Execution is waiting while the equipment is powering down but has not fully reached a stopped state.
        /// </summary>
        public const string POWERING_DOWN = "Execution is waiting while the equipment is powering down but has not fully reached a stopped state.";
        
        /// <summary>
        /// Execution is waiting while one or more discrete workpieces are being loaded.
        /// </summary>
        public const string PART_LOAD = "Execution is waiting while one or more discrete workpieces are being loaded.";
        
        /// <summary>
        /// Execution is waiting while one or more discrete workpieces are being unloaded.
        /// </summary>
        public const string PART_UNLOAD = "Execution is waiting while one or more discrete workpieces are being unloaded.";
        
        /// <summary>
        /// Execution is waiting while a tool or tooling is being loaded.
        /// </summary>
        public const string TOOL_LOAD = "Execution is waiting while a tool or tooling is being loaded.";
        
        /// <summary>
        /// Execution is waiting while a tool or tooling is being unloaded.
        /// </summary>
        public const string TOOL_UNLOAD = "Execution is waiting while a tool or tooling is being unloaded.";
        
        /// <summary>
        /// Execution is waiting while material is being loaded.
        /// </summary>
        public const string MATERIAL_LOAD = "Execution is waiting while material is being loaded.";
        
        /// <summary>
        /// Execution is waiting while material is being unloaded.
        /// </summary>
        public const string MATERIAL_UNLOAD = "Execution is waiting while material is being unloaded.";
        
        /// <summary>
        /// Execution is waiting while another process is completed before the execution can resume.
        /// </summary>
        public const string SECONDARY_PROCESS = "Execution is waiting while another process is completed before the execution can resume.";
        
        /// <summary>
        /// Execution is waiting while the equipment is pausing but the piece of equipment has not yet reached a fully paused state.
        /// </summary>
        public const string PAUSING = "Execution is waiting while the equipment is pausing but the piece of equipment has not yet reached a fully paused state.";
        
        /// <summary>
        /// Execution is waiting while the equipment is resuming the production cycle but has not yet resumed execution.
        /// </summary>
        public const string RESUMING = "Execution is waiting while the equipment is resuming the production cycle but has not yet resumed execution.";


        public static string Get(WaitState value)
        {
            switch (value)
            {
                case WaitState.POWERING_UP: return "Execution is waiting while the equipment is powering up and is not currently available to begin producing parts or products.";
                case WaitState.POWERING_DOWN: return "Execution is waiting while the equipment is powering down but has not fully reached a stopped state.";
                case WaitState.PART_LOAD: return "Execution is waiting while one or more discrete workpieces are being loaded.";
                case WaitState.PART_UNLOAD: return "Execution is waiting while one or more discrete workpieces are being unloaded.";
                case WaitState.TOOL_LOAD: return "Execution is waiting while a tool or tooling is being loaded.";
                case WaitState.TOOL_UNLOAD: return "Execution is waiting while a tool or tooling is being unloaded.";
                case WaitState.MATERIAL_LOAD: return "Execution is waiting while material is being loaded.";
                case WaitState.MATERIAL_UNLOAD: return "Execution is waiting while material is being unloaded.";
                case WaitState.SECONDARY_PROCESS: return "Execution is waiting while another process is completed before the execution can resume.";
                case WaitState.PAUSING: return "Execution is waiting while the equipment is pausing but the piece of equipment has not yet reached a fully paused state.";
                case WaitState.RESUMING: return "Execution is waiting while the equipment is resuming the production cycle but has not yet resumed execution.";
            }

            return null;
        }
    }
}