// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Buffers
{
    /// <summary>
    /// Reports the outcome of a buffer retention pass, identifying the range of sequence numbers purged, how many entries were removed, and how long the pass took.
    /// </summary>
    public struct ObservationBufferRetentionArgs
    {
        /// <summary>
        /// The first sequence number (inclusive) removed by the retention pass.
        /// </summary>
        public ulong From { get; }

        /// <summary>
        /// The last sequence number (inclusive) removed by the retention pass.
        /// </summary>
        public ulong To { get; }

        /// <summary>
        /// The number of observations evicted from the buffer.
        /// </summary>
        public ulong Count { get; }

        /// <summary>
        /// The elapsed time of the retention pass, in milliseconds.
        /// </summary>
        public long Duration { get; }


        /// <summary>
        /// Initializes the retention result with the evicted sequence range, count, and elapsed duration.
        /// </summary>
        /// <param name="from">The first sequence number removed.</param>
        /// <param name="to">The last sequence number removed.</param>
        /// <param name="count">The number of observations evicted.</param>
        /// <param name="duration">The elapsed retention time in milliseconds.</param>
        public ObservationBufferRetentionArgs(ulong from, ulong to, ulong count, long duration)
        {
            From = from;
            To = to;
            Count = count;
            Duration = duration;
        }
    }
}
