// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;

namespace MTConnect.Agents
{
    /// <summary>
    /// Result set used to retrieve MTConnectDeviceStreams
    /// </summary>
    public class DeviceStreamResults : IDeviceStreamResults
    {
        /// <summary>
        /// Gets the First Sequence available when the result set was processed
        /// </summary>
        public long FirstSequence { get; }

        /// <summary>
        /// Gets the Last Sequence available when the result set was processed
        /// </summary>
        public long LastSequence { get; }

        /// <summary>
        /// Gets the Next Sequence available when the result set was processed
        /// </summary>
        public long NextSequence { get; }

        /// <summary>
        /// Gets the DeviceStreams contained in the result set
        /// </summary>
        public IEnumerable<Streams.DeviceStream> DeviceStreams { get; }


        public DeviceStreamResults(long firstSequence, long lastSequence, long nextSequence, IEnumerable<Streams.DeviceStream> deviceStreams)
        {
            FirstSequence = firstSequence;
            LastSequence = lastSequence;
            NextSequence = nextSequence;
            DeviceStreams = deviceStreams;
        }
    }
}
