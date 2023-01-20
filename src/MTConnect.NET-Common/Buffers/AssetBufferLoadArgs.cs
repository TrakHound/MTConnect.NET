// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Buffers
{
    public struct AssetBufferLoadArgs
    {
        public long Count { get; }

        public long Duration { get; }


        public AssetBufferLoadArgs(long count, long duration)
        {
            Count = count;
            Duration = duration;
        }
    }
}