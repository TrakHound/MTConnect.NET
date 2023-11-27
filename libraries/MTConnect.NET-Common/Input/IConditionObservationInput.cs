// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace MTConnect.Input
{
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