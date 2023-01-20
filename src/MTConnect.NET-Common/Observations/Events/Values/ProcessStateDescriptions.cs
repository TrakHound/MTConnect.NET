// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events.Values
{
    /// <summary>
    /// The particular condition of the process occurrence at a specific time.
    /// </summary>
    public static class ProcessStateDescriptions
    {
        /// <summary>
        /// The device is preparing to execute the process occurrence.
        /// </summary>
        public const string INITIALIZING = "The device is preparing to execute the process occurrence.";

        /// <summary>
        /// The process occurrence is ready to be executed.
        /// </summary>
        public const string READY = "The process occurrence is ready to be executed.";

        /// <summary>
        /// The process occurrence is actively executing.
        /// </summary>
        public const string ACTIVE = "The process occurrence is actively executing.";

        /// <summary>
        /// The process occurrence is now finished.
        /// </summary>
        public const string COMPLETE = "The process occurrence is now finished.";

        /// <summary>
        /// The process occurrence has been stopped and may be resumed.
        /// </summary>
        public const string INTERRUPTED = "The process occurrence has been stopped and may be resumed.";

        /// <summary>
        /// The process occurrence has come to a premature end and cannot be resumed.
        /// </summary>
        public const string ABORTED = "The process occurrence has come to a premature end and cannot be resumed.";


        public static string Get(ProcessState value)
        {
            switch (value)
            {
                case ProcessState.INITIALIZING: return INITIALIZING;
                case ProcessState.READY: return READY;
                case ProcessState.ACTIVE: return ACTIVE;
                case ProcessState.COMPLETE: return COMPLETE;
                case ProcessState.INTERRUPTED: return INTERRUPTED;
                case ProcessState.ABORTED: return ABORTED;
            }

            return null;
        }
    }
}