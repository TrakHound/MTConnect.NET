// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// Particular condition of the process occurrence at a specific time.
    /// </summary>
    public enum ProcessState
    {
        /// <summary>
        /// Device is preparing to execute the process occurrence.
        /// </summary>
        INITIALIZING,
        
        /// <summary>
        /// Process occurrence is ready to be executed.
        /// </summary>
        READY,
        
        /// <summary>
        /// Process occurrence is actively executing.
        /// </summary>
        ACTIVE,
        
        /// <summary>
        /// Process occurrence is now finished.
        /// </summary>
        COMPLETE,
        
        /// <summary>
        /// Process occurrence has been stopped and may be resumed.
        /// </summary>
        INTERRUPTED,
        
        /// <summary>
        /// Process occurrence has come to a premature end and cannot be resumed.
        /// </summary>
        ABORTED
    }
}