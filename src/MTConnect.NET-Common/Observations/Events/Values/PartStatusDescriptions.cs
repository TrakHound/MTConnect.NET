// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Observations.Events.Values
{
    /// <summary>
    /// State or condition of a part.
    /// </summary>
    public static class PartStatusDescriptions
    {
        /// <summary>
        /// The part does not conform to some given requirements.
        /// </summary>
        public const string FAIL = "The part does not conform to some given requirements.";

        /// <summary>
        /// The part does conform to given requirements.
        /// </summary>
        public const string PASS = "The part does conform to given requirements.";


        public static string Get(PartStatus value)
        {
            switch (value)
            {
                case PartStatus.FAIL: return FAIL;
                case PartStatus.PASS: return PASS;
            }

            return null;
        }
    }
}
