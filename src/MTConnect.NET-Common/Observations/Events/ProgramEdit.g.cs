// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// Indication of the status of the Controller components program editing mode.A program may be edited while another is executed.
    /// </summary>
    public enum ProgramEdit
    {
        /// <summary>
        /// Controller is in the program edit mode.
        /// </summary>
        ACTIVE,
        
        /// <summary>
        /// Controller is capable of entering the program edit mode and no function is inhibiting a change to that mode.
        /// </summary>
        READY,
        
        /// <summary>
        /// Controller is being inhibited by a function from entering the program edit mode.
        /// </summary>
        NOT_READY
    }
}