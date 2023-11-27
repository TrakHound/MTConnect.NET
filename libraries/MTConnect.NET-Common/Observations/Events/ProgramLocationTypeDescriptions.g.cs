// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    public static class ProgramLocationTypeDescriptions
    {
        /// <summary>
        /// Managed by the controller.
        /// </summary>
        public const string LOCAL = "Managed by the controller.";
        
        /// <summary>
        /// Not managed by the controller.
        /// </summary>
        public const string EXTERNAL = "Not managed by the controller.";


        public static string Get(ProgramLocationType value)
        {
            switch (value)
            {
                case ProgramLocationType.LOCAL: return "Managed by the controller.";
                case ProgramLocationType.EXTERNAL: return "Not managed by the controller.";
            }

            return null;
        }
    }
}