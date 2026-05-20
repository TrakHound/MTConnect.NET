// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace MTConnect.Input
{
    /// <summary>
    /// An Information Model Input that describes Condition Streaming Data, composed of one or more FaultStates, reported by a piece of equipment.
    /// </summary>
    public interface IConditionObservationInput
    {
        /// <summary>
        /// The UUID of the Device that the Observation is associated with
        /// </summary>
        string DeviceKey { get; set; }

        /// <summary>
        /// The (ID, Name, or Source) of the DataItem that the Observation is associated with
        /// </summary>
        string DataItemKey { get; set; }


        /// <summary>
        /// The set of FaultStates that make up the current state of the Condition.
        /// </summary>
        IEnumerable<IConditionFaultStateObservationInput> FaultStates { get; set; }

        /// <summary>
        /// Gets or Sets whether the Observation is Unavailable
        /// </summary>
        bool IsUnavailable { get; set; }

        /// <summary>
        /// An MD5 Hash of the Observation that can be used for comparison
        /// </summary>
        byte[] ChangeId { get; }

        /// <summary>
        /// An MD5 Hash of the Observation including the Timestamp that can be used for comparison
        /// </summary>
        byte[] ChangeIdWithTimestamp { get; }
    }
}