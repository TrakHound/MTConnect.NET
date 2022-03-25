// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Observations.Events.Values
{
    /// <summary>
    /// An indication of whether the end of a piece of bar stock being feed by a bar feeder has been reached.
    /// </summary>
    public static class EndOfBarDescriptions
    {
        /// <summary>
        /// The EndOfBar has been reached.
        /// </summary>
        public const string YES = "The EndOfBar has been reached.";

        /// <summary>
        /// The EndOfBar has not been reached.
        /// </summary>
        public const string NO = "The EndOfBar has not been reached.";


        public static string Get(EndOfBar value)
        {
            switch (value)
            {
                case EndOfBar.YES: return YES;
                case EndOfBar.NO: return NO;
            }

            return null;
        }
    }
}
