// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System;

namespace MTConnect.Headers
{
    /// <summary>
    /// The Header element for an MTConnectStreams Response Document defines information regarding the creation
    /// of the document and additional information necessary for an application to interact and retrieve data from the Agent.
    /// </summary>
    public class MTConnectStreamsHeader : IMTConnectStreamsHeader
    {
        /// <summary>
        /// A number indicating a specific instantiation of the buffer associated with the Agent that published the Response Document.   
        /// </summary>
        public long InstanceId { get; set; }

        /// <summary>
        /// The major, minor, and revision number of the MTConnect Standard that defines the semantic data model that represents the content of the Response Document.
        /// It also includes the revision number of the schema associated with that specific semantic data model.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// An identification defining where the Agent that published the Response Document is installed or hosted.
        /// </summary>
        public string Sender { get; set; }

        /// <summary>
        /// A value representing the maximum number of Data Entities that MAY be retained in the Agent that published the Response Document at any point in time.
        /// </summary>
        public long BufferSize { get; set; }

        /// <summary>
        /// A number representing the sequence number assigned to the oldest piece of Streaming Data stored
        /// in the buffer of the Agent immediately prior to the time that the Agent published the Response Document.
        /// </summary>
        public long FirstSequence { get; set; }

        /// <summary>
        /// A number representing the sequence number assigned to the last piece of Streaming Data that was added
        /// to the buffer of the Agent immediately prior to the time that the Agent published the Response Document.
        /// </summary>
        public long LastSequence { get; set; }

        /// <summary>
        /// A number representing the sequence number of the piece of Streaming Data that is the next piece of data to be retrieved
        /// from the buffer of the Agent that was not included in the Response Document published by the Agent.
        /// </summary>
        public long NextSequence { get; set; }

        /// <summary>
        /// A timestamp in 8601 format of the last update of the Device information for any device.
        /// </summary>
        public string DeviceModelChangeTime { get; set; }

        /// <summary>
        /// A flag indicating that the Agent that published the Response Document is operating in a test mode.
        /// The contents of the Response Document may not be valid and SHOULD be used for testing and simulation purposes only.
        /// </summary>
        public string TestIndicator { get; set; }

        /// <summary>
        /// CreationTime represents the time that an Agent published the Response Document.
        /// </summary>
        public DateTime CreationTime { get; set; }
    }
}
