// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Http
{
    public struct MTConnectHttpStreamArgs
    {
        public string StreamId { get; set; }

        public byte[] Message { get; set; }

        public double ResponseDuration { get; set; }


        public MTConnectHttpStreamArgs(string streamId, byte[] message, double responseDuration)
        {
            StreamId = streamId;
            Message = message;
            ResponseDuration = responseDuration;
        }
    }
}
