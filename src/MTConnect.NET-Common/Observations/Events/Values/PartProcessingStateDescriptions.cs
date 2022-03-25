// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Observations.Events.Values
{
    /// <summary>
    /// The particular condition of the part occurrence at a specific time.
    /// </summary>
    public static class PartProcessingStateDescriptions
    {
        /// <summary>
        /// The part occurrence is not actively being processed, but the processing has not ended. 
        /// Processing requirements exist that have not yet been fulfilled. 
        /// This is the default entry state when the part occurrence is originally received.
        /// In some cases, the part occurrence may return to this state while it waits for additional processing to be performed.
        /// </summary>
        public const string NEEDS_PROCESSING = "The part occurrence is not actively being processed, but the processing has not ended. Processing requirements exist that have not yet been fulfilled. ";

        /// <summary>
        /// The part occurrence is actively being processed.
        /// </summary>
        public const string IN_PROCESS = "The part occurrence is actively being processed.";

        /// <summary>
        /// The part occurrence is no longer being processed. A general state when the reason for termination is unknown.
        /// </summary>
        public const string PROCESSING_END = "The part occurrence is no longer being processed. A general state when the reason for termination is unknown.";

        /// <summary>
        /// The part occurrence has completed processing successfully
        /// </summary>
        public const string PROCESSS_ENDED_COMPLETE = "The part occurrence has completed processing successfully";

        /// <summary>
        /// The process has been stopped during the processing. The part occurrence will require special treatment
        /// </summary>
        public const string PROCESSING_ENDED_STOPPED = "The process has been stopped during the processing. The part occurrence will require special treatment";

        /// <summary>
        /// The processing of the part occurrence has come to a premature end.
        /// </summary>
        public const string PROCESSING_ENDED_ABORTED = "The processing of the part occurrence has come to a premature end.";


        public static string Get(PartProcessingState value)
        {
            switch (value)
            {
                case PartProcessingState.NEEDS_PROCESSING: return NEEDS_PROCESSING;
                case PartProcessingState.IN_PROCESS: return IN_PROCESS;
                case PartProcessingState.PROCESSING_END: return PROCESSING_END;
                case PartProcessingState.PROCESSS_ENDED_COMPLETE: return PROCESSS_ENDED_COMPLETE;
                case PartProcessingState.PROCESSING_ENDED_STOPPED: return PROCESSING_ENDED_STOPPED;
                case PartProcessingState.PROCESSING_ENDED_ABORTED: return PROCESSING_ENDED_ABORTED;
            }

            return null;
        }
    }
}
