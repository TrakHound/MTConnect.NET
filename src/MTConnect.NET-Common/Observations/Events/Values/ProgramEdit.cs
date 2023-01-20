// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events.Values
{
    /// <summary>
    /// An indication of the status of the Controller components program editing mode
    /// </summary>
    public enum ProgramEdit
    {
        /// <summary>
        /// The controller is in the program edit mode.
        /// </summary>
        ACTIVE,

        /// <summary>
        /// The controller is capable of entering the program edit mode and no function is inhibiting a change to that mode.
        /// </summary>
        READY,

        /// <summary>
        /// A function is inhibiting the controller from entering the program edit mode.
        /// </summary>
        NOT_READY
    }
}