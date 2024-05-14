// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Buffers
{
    public struct ObservationBufferRetentionArgs
    {
        public ulong From { get; }

        public ulong To { get; }

        public ulong Count { get; }

        public long Duration { get; }


        public ObservationBufferRetentionArgs(ulong from, ulong to, ulong count, long duration)
        {
            From = from;
            To = to;
            Count = count;
            Duration = duration;
        }
    }
}