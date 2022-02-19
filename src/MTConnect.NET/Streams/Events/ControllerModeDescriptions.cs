// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Events
{
    /// <summary>
    /// The current operating mode of the Controller component.
    /// </summary>
    public static class ControllerModeDescriptions
    {
        /// <summary>
        ///  The controller is not executing an active program. 
        ///  It is capable of receiving instructions from an external source – typically an operator. 
        ///  The controller executes operations based on the instructions received from the external source.
        /// </summary>
        public const string MANUAL = "The controller is not executing an active program. It is capable of receiving instructions from an external source – typically an operator. The controller executes operations based on the instructions received from the external source.";

        /// <summary>
        /// The operator can enter a series of operations for the controller to perform. 
        /// The controller will execute this specific series of operations and then stop.
        /// </summary>
        public const string MANUAL_DATA_INPUT = "The operator can enter a series of operations for the controller to perform. The controller will execute this specific series of operations and then stop.";

        /// <summary>
        /// The controller is operating in a mode that restricts the active program from processing its next process step without operator intervention.
        /// </summary>
        public const string SEMI_AUTOMATIC = "The controller is operating in a mode that restricts the active program from processing its next process step without operator intervention.";

        /// <summary>
        /// The controller is configured to automatically execute a program.
        /// </summary>
        public const string AUTOMATIC = "The controller is configured to automatically execute a program.";

        /// <summary>
        /// The controller is currently functioning as a programming device and is not capable of executing an active program.
        /// </summary>
        public const string EDIT = "The controller is currently functioning as a programming device and is not capable of executing an active program.";


        public static string Get(ControllerMode value)
        {
            switch (value)
            {
                case ControllerMode.MANUAL: return MANUAL;
                case ControllerMode.MANUAL_DATA_INPUT: return MANUAL_DATA_INPUT;
                case ControllerMode.SEMI_AUTOMATIC: return SEMI_AUTOMATIC;
                case ControllerMode.AUTOMATIC: return AUTOMATIC;
                case ControllerMode.EDIT: return EDIT;
            }

            return null;
        }
    }
}
