// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Buffers
{
    public struct ObservationBufferRetentionArgs
    {
        public long From { get; }

        public long To { get; }

        public long Count { get; }

        public long Duration { get; }


        public ObservationBufferRetentionArgs(long from, long to, long count, long duration)
        {
            From = from;
            To = to;
            Count = count;
            Duration = duration;
        }
    }
}