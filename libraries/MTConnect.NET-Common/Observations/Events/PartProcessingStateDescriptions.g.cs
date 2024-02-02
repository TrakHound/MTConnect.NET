// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    public static class PartProcessingStateDescriptions
    {
        /// <summary>
        /// Part occurrence is not actively being processed, but the processing has not ended. Processing requirements exist that have not yet been fulfilled. This is the default entry state when the part occurrence is originally received. In some cases, the part occurrence may return to this state while it waits for additional processing to be performed.
        /// </summary>
        public const string NEEDS_PROCESSING = "Part occurrence is not actively being processed, but the processing has not ended. Processing requirements exist that have not yet been fulfilled. This is the default entry state when the part occurrence is originally received. In some cases, the part occurrence may return to this state while it waits for additional processing to be performed.";
        
        /// <summary>
        /// Part occurrence is actively being processed.
        /// </summary>
        public const string IN_PROCESS = "Part occurrence is actively being processed.";
        
        /// <summary>
        /// Part occurrence is no longer being processed. A general state when the reason for termination is unknown.
        /// </summary>
        public const string PROCESSING_ENDED = "Part occurrence is no longer being processed. A general state when the reason for termination is unknown.";
        
        /// <summary>
        /// Part occurrence has completed processing successfully.
        /// </summary>
        public const string PROCESSING_ENDED_COMPLETE = "Part occurrence has completed processing successfully.";
        
        /// <summary>
        /// Process has been stopped during the processing. The part occurrence will require special treatment.
        /// </summary>
        public const string PROCESSING_ENDED_STOPPED = "Process has been stopped during the processing. The part occurrence will require special treatment.";
        
        /// <summary>
        /// Processing of the part occurrence has come to a premature end.
        /// </summary>
        public const string PROCESSING_ENDED_ABORTED = "Processing of the part occurrence has come to a premature end.";
        
        /// <summary>
        /// Terminal state when the part occurrence has been removed from the equipment by an external entity and it no longer exists at the equipment.
        /// </summary>
        public const string PROCESSING_ENDED_LOST = "Terminal state when the part occurrence has been removed from the equipment by an external entity and it no longer exists at the equipment.";
        
        /// <summary>
        /// Part occurrence has been skipped for processing on the piece of equipment.
        /// </summary>
        public const string PROCESSING_ENDED_SKIPPED = "Part occurrence has been skipped for processing on the piece of equipment.";
        
        /// <summary>
        /// Part occurrence has been processed completely. However, the processing may have a problem.
        /// </summary>
        public const string PROCESSING_ENDED_REJECTED = "Part occurrence has been processed completely. However, the processing may have a problem.";
        
        /// <summary>
        /// Part occurrence is waiting for transit.
        /// </summary>
        public const string WAITING_FOR_TRANSIT = "Part occurrence is waiting for transit.";
        
        /// <summary>
        /// Part occurrence is being transported to its destination.
        /// </summary>
        public const string IN_TRANSIT = "Part occurrence is being transported to its destination.";
        
        /// <summary>
        /// Part occurrence has been placed at its designated destination.
        /// </summary>
        public const string TRANSIT_COMPLETE = "Part occurrence has been placed at its designated destination.";


        public static string Get(PartProcessingState value)
        {
            switch (value)
            {
                case PartProcessingState.NEEDS_PROCESSING: return "Part occurrence is not actively being processed, but the processing has not ended. Processing requirements exist that have not yet been fulfilled. This is the default entry state when the part occurrence is originally received. In some cases, the part occurrence may return to this state while it waits for additional processing to be performed.";
                case PartProcessingState.IN_PROCESS: return "Part occurrence is actively being processed.";
                case PartProcessingState.PROCESSING_ENDED: return "Part occurrence is no longer being processed. A general state when the reason for termination is unknown.";
                case PartProcessingState.PROCESSING_ENDED_COMPLETE: return "Part occurrence has completed processing successfully.";
                case PartProcessingState.PROCESSING_ENDED_STOPPED: return "Process has been stopped during the processing. The part occurrence will require special treatment.";
                case PartProcessingState.PROCESSING_ENDED_ABORTED: return "Processing of the part occurrence has come to a premature end.";
                case PartProcessingState.PROCESSING_ENDED_LOST: return "Terminal state when the part occurrence has been removed from the equipment by an external entity and it no longer exists at the equipment.";
                case PartProcessingState.PROCESSING_ENDED_SKIPPED: return "Part occurrence has been skipped for processing on the piece of equipment.";
                case PartProcessingState.PROCESSING_ENDED_REJECTED: return "Part occurrence has been processed completely. However, the processing may have a problem.";
                case PartProcessingState.WAITING_FOR_TRANSIT: return "Part occurrence is waiting for transit.";
                case PartProcessingState.IN_TRANSIT: return "Part occurrence is being transported to its destination.";
                case PartProcessingState.TRANSIT_COMPLETE: return "Part occurrence has been placed at its designated destination.";
            }

            return null;
        }
    }
}