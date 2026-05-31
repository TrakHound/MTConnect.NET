// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Buffers
{
    /// <summary>
    /// Reports the outcome of restoring a durable asset buffer from disk: how many assets were loaded and how long the load took.
    /// </summary>
    public struct AssetBufferLoadArgs
    {
        /// <summary>
        /// The number of assets recovered from the durable buffer.
        /// </summary>
        public long Count { get; }

        /// <summary>
        /// The elapsed time of the load operation, in milliseconds.
        /// </summary>
        public long Duration { get; }


        /// <summary>
        /// Initializes the load result with the recovered asset count and elapsed duration.
        /// </summary>
        /// <param name="count">The number of assets recovered.</param>
        /// <param name="duration">The elapsed load time in milliseconds.</param>
        public AssetBufferLoadArgs(long count, long duration)
        {
            Count = count;
            Duration = duration;
        }
    }
}
