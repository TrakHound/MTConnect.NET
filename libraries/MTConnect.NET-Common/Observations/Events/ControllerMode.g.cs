// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// Current mode of the Controller component.
    /// </summary>
    public enum ControllerMode
    {
        /// <summary>
        /// Controller is configured to automatically execute a program.
        /// </summary>
        AUTOMATIC,
        
        /// <summary>
        /// Controller is not executing an active program. It is capable of receiving instructions from an external source – typically an operator. The Controller executes operations based on the instructions received from the external source.
        /// </summary>
        MANUAL,
        
        /// <summary>
        /// Operator can enter a series of operations for the Controller to perform.The Controller will execute this specific series of operations and then stop.
        /// </summary>
        MANUAL_DATA_INPUT,
        
        /// <summary>
        /// Controller is operating in a mode that restricts the active program from processing its next process step without operator intervention.
        /// </summary>
        SEMI_AUTOMATIC,
        
        /// <summary>
        /// Controller is currently functioning as a programming device and is not capable of executing an active program.
        /// </summary>
        EDIT,
        
        /// <summary>
        /// Axes of the device are commanded to stop, but the spindle continues to function.
        /// </summary>
        FEED_HOLD
    }
}