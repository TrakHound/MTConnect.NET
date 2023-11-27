// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Observations.Events
{
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