// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Servers.Http
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