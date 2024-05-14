// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Headers
{
    public static class MTConnectHeaderAttributeDescriptions
    {
        /// <summary>
        /// A number indicating a specific instantiation of the buffer associated with the Agent that published the Response Document.   
        /// </summary>
        public const string InstanceId = "A number indicating a specific instantiation of the buffer associated with the Agent that published the Response Document. ";

        /// <summary>
        /// The major, minor, and revision number of the MTConnect Standard that defines the semantic data model that represents the content of the Response Document.
        /// It also includes the revision number of the schema associated with that specific semantic data model.
        /// </summary>
        public const string Version = "The major, minor, and revision number of the MTConnect Standard that defines the semantic data model that represents the content of the Response Document. It also includes the revision number of the schema associated with that specific semantic data model.";

        /// <summary>
        /// An identification defining where the Agent that published the Response Document is installed or hosted.
        /// </summary>
        public const string Sender = "An identification defining where the Agent that published the Response Document is installed or hosted.";

        /// <summary>
        /// A value representing the maximum number of Data Entities that MAY be retained in the Agent that published the Response Document at any point in time.
        /// </summary>
        public const string BufferSize = "A value representing the maximum number of Data Entities that MAY be retained in the Agent that published the Response Document at any point in time.";

        /// <summary>
        /// A number representing the sequence number assigned to the oldest piece of Streaming Data stored
        /// in the buffer of the Agent immediately prior to the time that the Agent published the Response Document.
        /// </summary>
        public const string FirstSequence = "A number representing the sequence number assigned to the oldest piece of Streaming Data stored in the buffer of the Agent immediately prior to the time that the Agent published the Response Document.";

        /// <summary>
        /// A number representing the sequence number assigned to the last piece of Streaming Data that was added
        /// in the buffer of the Agent immediately prior to the time that the Agent published the Response Document.
        /// </summary>
        public const string LastSequence = "A number representing the sequence number assigned to the last piece of Streaming Data that was added in the buffer of the Agent immediately prior to the time that the Agent published the Response Document.";

        /// <summary>
        /// A number representing the sequence number of the piece of Streaming Data that is the next piece of data to be retrieved
        /// from the buffer of the Agent that was not included in the Response Document published by the Agent.
        /// </summary>
        public const string NextSequence = "A number representing the sequence number of the piece of Streaming Data that is the next piece of data to be retrieved from the buffer of the Agent that was not included in the Response Document published by the Agent.";

        /// <summary>
        /// A value representing the maximum number of Asset Documents that can be stored in the Agent that published the Response Document.   
        /// </summary>
        public const string AssetBufferSize = "A value representing the maximum number of Asset Documents that can be stored in the Agent that published the Response Document.";

        /// <summary>
        /// A number representing the current number of Asset Documents that are currently stored in the Agent as of the creationTime that the Agent published the Response Document.
        /// </summary>
        public const string AssetCount = "A number representing the current number of Asset Documents that are currently stored in the Agent as of the creationTime that the Agent published the Response Document.";

        /// <summary>
        /// A timestamp in 8601 format of the last update of the Device information for any device.
        /// </summary>
        public const string DeviceModelChangeTime = "A timestamp in 8601 format of the last update of the Device information for any device.";

        /// <summary>
        /// A flag indicating that the Agent that published the Response Document is operating in a test mode.
        /// The contents of the Response Document may not be valid and SHOULD be used for testing and simulation purposes only.
        /// </summary>
        public const string TestIndicator = "A flag indicating that the Agent that published the Response Document is operating in a test mode. The contents of the Response Document may not be valid and SHOULD be used for testing and simulation purposes only.";

        /// <summary>
        /// CreationTime represents the time that an Agent published the Response Document.
        /// </summary>
        public const string CreationTime = "CreationTime represents the time that an Agent published the Response Document.";
    }
}