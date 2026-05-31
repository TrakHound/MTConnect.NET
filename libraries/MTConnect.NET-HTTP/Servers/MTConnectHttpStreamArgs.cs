// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.IO;

namespace MTConnect.Servers.Http
{
    /// <summary>
    /// One chunk of an MTConnect HTTP streaming response (the long-poll <c>sample</c> request with
    /// <c>interval</c>). The server emits a sequence of these as a multipart-mixed body; each
    /// instance carries the formatted MTConnect document fragment together with timing metadata
    /// used to throttle and observe the stream.
    /// </summary>
    public struct MTConnectHttpStreamArgs
    {
        /// <summary>Identifier of the stream the chunk belongs to; used by handlers that route multiple concurrent streams.</summary>
        public string StreamId { get; set; }

        /// <summary>The already-formatted MTConnect document bytes to write as the next multipart part.</summary>
        public Stream Message { get; set; }

        /// <summary>The duration in milliseconds spent producing this chunk (formatter + queue wait).</summary>
        public double ResponseDuration { get; set; }


        /// <summary>Constructs a chunk descriptor with its stream id, formatted payload, and the time taken to produce it.</summary>
        /// <param name="streamId">Identifier of the stream the chunk belongs to.</param>
        /// <param name="message">The formatted MTConnect document to write.</param>
        /// <param name="responseDuration">Production duration in milliseconds.</param>
        public MTConnectHttpStreamArgs(string streamId, Stream message, double responseDuration)
        {
            StreamId = streamId;
            Message = message;
            ResponseDuration = responseDuration;
        }
    }
}
