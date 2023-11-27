// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    public static class ControllerModeDescriptions
    {
        /// <summary>
        /// Controller is configured to automatically execute a program.
        /// </summary>
        public const string AUTOMATIC = "Controller is configured to automatically execute a program.";
        
        /// <summary>
        /// Controller is not executing an active program. It is capable of receiving instructions from an external source – typically an operator. The Controller executes operations based on the instructions received from the external source.
        /// </summary>
        public const string MANUAL = "Controller is not executing an active program. It is capable of receiving instructions from an external source – typically an operator. The Controller executes operations based on the instructions received from the external source.";
        
        /// <summary>
        /// Operator can enter a series of operations for the Controller to perform.The Controller will execute this specific series of operations and then stop.
        /// </summary>
        public const string MANUAL_DATA_INPUT = "Operator can enter a series of operations for the Controller to perform.The Controller will execute this specific series of operations and then stop.";
        
        /// <summary>
        /// Controller is operating in a mode that restricts the active program from processing its next process step without operator intervention.
        /// </summary>
        public const string SEMI_AUTOMATIC = "Controller is operating in a mode that restricts the active program from processing its next process step without operator intervention.";
        
        /// <summary>
        /// Controller is currently functioning as a programming device and is not capable of executing an active program.
        /// </summary>
        public const string EDIT = "Controller is currently functioning as a programming device and is not capable of executing an active program.";
        
        /// <summary>
        /// Axes of the device are commanded to stop, but the spindle continues to function.
        /// </summary>
        public const string FEED_HOLD = "Axes of the device are commanded to stop, but the spindle continues to function.";


        public static string Get(ControllerMode value)
        {
            switch (value)
            {
                case ControllerMode.AUTOMATIC: return "Controller is configured to automatically execute a program.";
                case ControllerMode.MANUAL: return "Controller is not executing an active program. It is capable of receiving instructions from an external source – typically an operator. The Controller executes operations based on the instructions received from the external source.";
                case ControllerMode.MANUAL_DATA_INPUT: return "Operator can enter a series of operations for the Controller to perform.The Controller will execute this specific series of operations and then stop.";
                case ControllerMode.SEMI_AUTOMATIC: return "Controller is operating in a mode that restricts the active program from processing its next process step without operator intervention.";
                case ControllerMode.EDIT: return "Controller is currently functioning as a programming device and is not capable of executing an active program.";
                case ControllerMode.FEED_HOLD: return "Axes of the device are commanded to stop, but the spindle continues to function.";
            }

            return null;
        }
    }
}