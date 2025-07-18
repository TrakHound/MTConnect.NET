// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace MTConnect.Headers
{
    /// <summary>
    /// The Header element for an MTConnectAssets Response Document defines information regarding the creation
    /// of the document and the storage of Asset Documents in the Agent that generated the document.
    /// </summary>
    public class MTConnectAssetsHeader : IMTConnectAssetsHeader
    {
        /// <summary>
        /// A number indicating a specific instantiation of the buffer associated with the Agent that published the Response Document.   
        /// </summary>
        public ulong InstanceId { get; set; }

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
        /// A value representing the maximum number of Asset Documents that can be stored in the Agent that published the Response Document.   
        /// </summary>
        public ulong AssetBufferSize { get; set; }

        /// <summary>
        /// A number representing the current number of Asset Documents that are currently stored in the Agent as of the creationTime that the Agent published the Response Document.
        /// </summary>
        public ulong AssetCount { get; set; }

        /// <summary>
        /// A timestamp in 8601 format of the last update of the Device information for any device.
        /// </summary>
        public string DeviceModelChangeTime { get; set; }

        /// <summary>
        /// A flag indicating that the Agent that published the Response Document is operating in a test mode.
        /// The contents of the Response Document may not be valid and SHOULD be used for testing and simulation purposes only.
        /// </summary>
        public bool TestIndicator { get; set; }

        /// <summary>
        /// Indicates if the MTConnect Agent is validating against the normative model.
        /// </summary>
        public bool Validation { get; set; }

        /// <summary>
        /// CreationTime represents the time that an Agent published the Response Document.
        /// </summary>
        public DateTime CreationTime { get; set; }
    }
}