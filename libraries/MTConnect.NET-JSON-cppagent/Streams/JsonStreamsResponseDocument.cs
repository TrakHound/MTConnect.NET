// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Streams.Output;
using System.Text.Json.Serialization;

namespace MTConnect.Streams.Json
{
    /// <summary>
    /// Outer JSON envelope for a Streams response document in the
    /// cppagent-compatible shape, with the actual content wrapped in a
    /// single <c>MTConnectStreams</c> property to mirror the XML root
    /// element name.
    /// </summary>
    public class JsonStreamsResponseDocument
    {
        /// <summary>
        /// The wrapped streams document.
        /// </summary>
        [JsonPropertyName("MTConnectStreams")]
        public JsonMTConnectStreams MTConnectStreams { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonStreamsResponseDocument() { }

        /// <summary>
        /// Initializes the envelope from a strongly-typed streams
        /// document.
        /// </summary>
        public JsonStreamsResponseDocument(IStreamsResponseOutputDocument streamsDocument)
        {
            if (streamsDocument != null)
            {
                MTConnectStreams = new JsonMTConnectStreams(streamsDocument);
            }
        }


        /// <summary>
        /// Unwraps the envelope and converts it to a strongly-typed
        /// streams document, returning <c>null</c> when the envelope is
        /// empty.
        /// </summary>
        public IStreamsResponseDocument ToStreamsDocument()
        {
            if (MTConnectStreams != null)
            {
                return MTConnectStreams.ToStreamsDocument();
            }

            return null;
        }
    }
}