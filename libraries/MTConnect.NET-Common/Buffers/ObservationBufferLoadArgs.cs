// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Buffers
{
    /// <summary>
    /// Reports the outcome of restoring a durable observation buffer from disk: how many observations were loaded and how long the load took.
    /// </summary>
    public struct ObservationBufferLoadArgs
    {
        /// <summary>
        /// The number of observations recovered from the durable buffer.
        /// </summary>
        public long Count { get; }

        /// <summary>
        /// The elapsed time of the load operation, in milliseconds.
        /// </summary>
        public long Duration { get; }


        /// <summary>
        /// Initializes the load result with the recovered observation count and elapsed duration.
        /// </summary>
        /// <param name="count">The number of observations recovered.</param>
        /// <param name="duration">The elapsed load time in milliseconds.</param>
        public ObservationBufferLoadArgs(long count, long duration)
        {
            Count = count;
            Duration = duration;
        }
    }
}
