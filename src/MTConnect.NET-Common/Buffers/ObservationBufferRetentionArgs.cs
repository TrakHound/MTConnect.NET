// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.


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
