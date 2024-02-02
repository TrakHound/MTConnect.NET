// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    public static class ProcessStateDescriptions
    {
        /// <summary>
        /// Device is preparing to execute the process occurrence.
        /// </summary>
        public const string INITIALIZING = "Device is preparing to execute the process occurrence.";
        
        /// <summary>
        /// Process occurrence is ready to be executed.
        /// </summary>
        public const string READY = "Process occurrence is ready to be executed.";
        
        /// <summary>
        /// Process occurrence is actively executing.
        /// </summary>
        public const string ACTIVE = "Process occurrence is actively executing.";
        
        /// <summary>
        /// Process occurrence is now finished.
        /// </summary>
        public const string COMPLETE = "Process occurrence is now finished.";
        
        /// <summary>
        /// Process occurrence has been stopped and may be resumed.
        /// </summary>
        public const string INTERRUPTED = "Process occurrence has been stopped and may be resumed.";
        
        /// <summary>
        /// Process occurrence has come to a premature end and cannot be resumed.
        /// </summary>
        public const string ABORTED = "Process occurrence has come to a premature end and cannot be resumed.";


        public static string Get(ProcessState value)
        {
            switch (value)
            {
                case ProcessState.INITIALIZING: return "Device is preparing to execute the process occurrence.";
                case ProcessState.READY: return "Process occurrence is ready to be executed.";
                case ProcessState.ACTIVE: return "Process occurrence is actively executing.";
                case ProcessState.COMPLETE: return "Process occurrence is now finished.";
                case ProcessState.INTERRUPTED: return "Process occurrence has been stopped and may be resumed.";
                case ProcessState.ABORTED: return "Process occurrence has come to a premature end and cannot be resumed.";
            }

            return null;
        }
    }
}