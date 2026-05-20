// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// Description text for each <see cref="PartDetect"/> value as defined by the MTConnect Standard.
    /// </summary>
    public static class PartDetectDescriptions
    {
        /// <summary>
        /// Part or work piece is detected or is present.
        /// </summary>
        public const string PRESENT = "Part or work piece is detected or is present.";
        
        /// <summary>
        /// Part or work piece is not detected or is not present.
        /// </summary>
        public const string NOT_PRESENT = "Part or work piece is not detected or is not present.";


        /// <summary>
        /// Returns the MTConnect Standard description text for the specified <see cref="PartDetect"/> value, or <c>null</c> when none is defined.
        /// </summary>
        public static string Get(PartDetect value)
        {
            switch (value)
            {
                case PartDetect.PRESENT: return "Part or work piece is detected or is present.";
                case PartDetect.NOT_PRESENT: return "Part or work piece is not detected or is not present.";
            }

            return null;
        }
    }
}