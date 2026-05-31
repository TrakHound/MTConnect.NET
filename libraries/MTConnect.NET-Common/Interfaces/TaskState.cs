// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Interfaces
{
    /// <summary>
    /// The lifecycle state of an Interface <see cref="TaskAsset"/> as it moves from inactive through preparation and commitment to completion or failure.
    /// </summary>
    public enum TaskState
    {
        /// <summary>
        /// The task has been irrevocably accepted and is being carried out.
        /// </summary>
        COMMITTED,

        /// <summary>
        /// The task is in the process of being committed but the commitment is not yet final.
        /// </summary>
        COMMITTING,

        /// <summary>
        /// The task has finished successfully.
        /// </summary>
        COMPLETE,

        /// <summary>
        /// The task could not be completed because a failure was detected.
        /// </summary>
        FAIL,

        /// <summary>
        /// The task is defined but not currently being prepared or performed.
        /// </summary>
        INACTIVE,

        /// <summary>
        /// The task is being set up in anticipation of being committed.
        /// </summary>
        PREPARING
    }
}
