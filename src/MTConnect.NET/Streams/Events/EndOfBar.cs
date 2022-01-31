// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Streams.Events
{
    /// <summary>
    /// An indication of whether the end of a piece of bar stock being feed by a bar feeder has been reached.
    /// </summary>
    public enum EndOfBar
    {
        /// <summary>
        ///  The EndOfBar has been reached.
        /// </summary>
        YES,

        /// <summary>
        /// The EndOfBar has not been reached.
        /// </summary>
        NO
    }
}
