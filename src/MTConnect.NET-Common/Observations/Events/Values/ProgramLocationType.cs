// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events.Values
{
    /// <summary>
    /// Defines whether the logic or motion program defined by PROGRAM is being executed from the local memory of the controller or from an outside source.
    /// </summary>
    public enum ProgramLocationType
    {
        /// <summary>
        /// Managed by the controller.
        /// </summary>
        LOCAL,

        /// <summary>
        ///  Not managed by the controller.
        /// </summary>
        EXTERNAL
    }
}