// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events.Values
{
    /// <summary>
    /// The particular condition of the part occurrence at a specific time.
    /// </summary>
    public enum PartProcessingState
    {
        /// <summary>
        /// The part occurrence is not actively being processed, but the processing has not ended. 
        /// Processing requirements exist that have not yet been fulfilled. 
        /// This is the default entry state when the part occurrence is originally received.
        /// In some cases, the part occurrence may return to this state while it waits for additional processing to be performed.
        /// </summary>
        NEEDS_PROCESSING,

        /// <summary>
        /// The part occurrence is actively being processed
        /// </summary>
        IN_PROCESS,

        /// <summary>
        /// The part occurrence is no longer being processed. A general state when the reason for termination is unknown.
        /// </summary>
        PROCESSING_END,

        /// <summary>
        /// The part occurrence has completed processing successfully
        /// </summary>
        PROCESSS_ENDED_COMPLETE,

        /// <summary>
        ///  The process has been stopped during the processing. The part occurrence will require special treatment
        /// </summary>
        PROCESSING_ENDED_STOPPED,

        /// <summary>
        /// The processing of the part occurrence has come to a premature end.
        /// </summary>
        PROCESSING_ENDED_ABORTED
    }
}