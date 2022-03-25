// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Headers
{
    public static class MTConnectDevicesAttributeDescriptions
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
