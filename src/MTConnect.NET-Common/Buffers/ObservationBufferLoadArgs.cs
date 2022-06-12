// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.


namespace MTConnect.Buffers
{
    public struct ObservationBufferLoadArgs
    {
        public long Count { get; }

        public long Duration { get; }


        public ObservationBufferLoadArgs(long count, long duration)
        {
            Count = count;
            Duration = duration;
        }
    }
}
