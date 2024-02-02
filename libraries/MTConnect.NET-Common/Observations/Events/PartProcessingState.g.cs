// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// Particular condition of the part occurrence at a specific time.
    /// </summary>
    public enum PartProcessingState
    {
        /// <summary>
        /// Part occurrence is not actively being processed, but the processing has not ended. Processing requirements exist that have not yet been fulfilled. This is the default entry state when the part occurrence is originally received. In some cases, the part occurrence may return to this state while it waits for additional processing to be performed.
        /// </summary>
        NEEDS_PROCESSING,
        
        /// <summary>
        /// Part occurrence is actively being processed.
        /// </summary>
        IN_PROCESS,
        
        /// <summary>
        /// Part occurrence is no longer being processed. A general state when the reason for termination is unknown.
        /// </summary>
        PROCESSING_ENDED,
        
        /// <summary>
        /// Part occurrence has completed processing successfully.
        /// </summary>
        PROCESSING_ENDED_COMPLETE,
        
        /// <summary>
        /// Process has been stopped during the processing. The part occurrence will require special treatment.
        /// </summary>
        PROCESSING_ENDED_STOPPED,
        
        /// <summary>
        /// Processing of the part occurrence has come to a premature end.
        /// </summary>
        PROCESSING_ENDED_ABORTED,
        
        /// <summary>
        /// Terminal state when the part occurrence has been removed from the equipment by an external entity and it no longer exists at the equipment.
        /// </summary>
        PROCESSING_ENDED_LOST,
        
        /// <summary>
        /// Part occurrence has been skipped for processing on the piece of equipment.
        /// </summary>
        PROCESSING_ENDED_SKIPPED,
        
        /// <summary>
        /// Part occurrence has been processed completely. However, the processing may have a problem.
        /// </summary>
        PROCESSING_ENDED_REJECTED,
        
        /// <summary>
        /// Part occurrence is waiting for transit.
        /// </summary>
        WAITING_FOR_TRANSIT,
        
        /// <summary>
        /// Part occurrence is being transported to its destination.
        /// </summary>
        IN_TRANSIT,
        
        /// <summary>
        /// Part occurrence has been placed at its designated destination.
        /// </summary>
        TRANSIT_COMPLETE
    }
}