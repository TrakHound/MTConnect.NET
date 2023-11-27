// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    public static class ProgramEditDescriptions
    {
        /// <summary>
        /// Controller is in the program edit mode.
        /// </summary>
        public const string ACTIVE = "Controller is in the program edit mode.";
        
        /// <summary>
        /// Controller is capable of entering the program edit mode and no function is inhibiting a change to that mode.
        /// </summary>
        public const string READY = "Controller is capable of entering the program edit mode and no function is inhibiting a change to that mode.";
        
        /// <summary>
        /// Controller is being inhibited by a function from entering the program edit mode.
        /// </summary>
        public const string NOT_READY = "Controller is being inhibited by a function from entering the program edit mode.";


        public static string Get(ProgramEdit value)
        {
            switch (value)
            {
                case ProgramEdit.ACTIVE: return "Controller is in the program edit mode.";
                case ProgramEdit.READY: return "Controller is capable of entering the program edit mode and no function is inhibiting a change to that mode.";
                case ProgramEdit.NOT_READY: return "Controller is being inhibited by a function from entering the program edit mode.";
            }

            return null;
        }
    }
}