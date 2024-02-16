// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.IO;

namespace MTConnect.Servers.Http
{
    public struct MTConnectHttpStreamArgs
    {
        public string StreamId { get; set; }

        public Stream Message { get; set; }

        public double ResponseDuration { get; set; }


        public MTConnectHttpStreamArgs(string streamId, Stream message, double responseDuration)
        {
            StreamId = streamId;
            Message = message;
            ResponseDuration = responseDuration;
        }
    }
}