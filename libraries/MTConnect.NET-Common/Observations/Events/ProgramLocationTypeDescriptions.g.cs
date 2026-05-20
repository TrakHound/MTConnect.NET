// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// Description text for each <see cref="ProgramLocationType"/> value as defined by the MTConnect Standard.
    /// </summary>
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


        /// <summary>
        /// Returns the MTConnect Standard description text for the specified <see cref="ProgramLocationType"/> value, or <c>null</c> when none is defined.
        /// </summary>
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