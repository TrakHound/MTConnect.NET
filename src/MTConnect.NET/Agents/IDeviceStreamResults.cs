// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;

namespace MTConnect.Agents
{
    /// <summary>
    /// Result set used to retrieve MTConnectDeviceStreams
    /// </summary>
    public interface IDeviceStreamResults
    {
        /// <summary>
        /// Gets the First Sequence available when the result set was processed
        /// </summary>
        long FirstSequence { get; }

        /// <summary>
        /// Gets the Last Sequence available when the result set was processed
        /// </summary>
        long LastSequence { get; }

        /// <summary>
        /// Gets the Next Sequence available when the result set was processed
        /// </summary>
        long NextSequence { get; }

        /// <summary>
        /// Gets the DeviceStreams contained in the result set
        /// </summary>
        IEnumerable<Streams.DeviceStream> DeviceStreams { get; }
    }
}
