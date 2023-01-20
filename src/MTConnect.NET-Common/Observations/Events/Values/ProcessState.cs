// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events.Values
{
    /// <summary>
    /// The particular condition of the process occurrence at a specific time.
    /// </summary>
    public enum ProcessState
    {
        /// <summary>
        /// The device is preparing to execute the process occurrence.
        /// </summary>
        INITIALIZING,

        /// <summary>
        /// The process occurrence is ready to be executed
        /// </summary>
        READY,

        /// <summary>
        /// The process occurrence is actively executing
        /// </summary>
        ACTIVE,

        /// <summary>
        /// The process occurrence is now finished.
        /// </summary>
        COMPLETE,

        /// <summary>
        /// The process occurrence has been stopped and may be resumed.
        /// </summary>
        INTERRUPTED,

        /// <summary>
        /// The process occurrence has come to a premature end and cannot be resumed
        /// </summary>
        ABORTED
    }
}