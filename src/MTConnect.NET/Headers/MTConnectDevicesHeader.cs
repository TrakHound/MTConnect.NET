// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace MTConnect.Headers
{
    /// <summary>
    /// Contains the Header information in an MTConnect Devices XML document
    /// </summary>
    public class MTConnectDevicesHeader
    {
        /// <summary>
        /// A number indicating a specific instantiation of the buffer associated with the Agent that published the Response Document.   
        /// </summary>
        [XmlAttribute("instanceId")]
        [JsonPropertyName("instanceId")]
        public long InstanceId { get; set; }

        /// <summary>
        /// The major, minor, and revision number of the MTConnect Standard that defines the semantic data model that represents the content of the Response Document.
        /// It also includes the revision number of the schema associated with that specific semantic data model.
        /// </summary>
        [XmlAttribute("version")]
        [JsonPropertyName("version")]
        public string Version { get; set; }

        /// <summary>
        /// An identification defining where the Agent that published the Response Document is installed or hosted.
        /// </summary>
        [XmlAttribute("sender")]
        [JsonPropertyName("sender")]
        public string Sender { get; set; }

        /// <summary>
        /// A value representing the maximum number of Data Entities that MAY be retained in the Agent that published the Response Document at any point in time.
        /// </summary>
        [XmlAttribute("bufferSize")]
        [JsonPropertyName("bufferSize")]
        public long BufferSize { get; set; }

        /// <summary>
        /// A value representing the maximum number of Asset Documents that can be stored in the Agent that published the Response Document.   
        /// </summary>
        [XmlAttribute("assetBufferSize")]
        [JsonPropertyName("assetBufferSize")]
        public long AssetBufferSize { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public bool AssetBufferSizeSpecified => AssetBufferSize >= 0;

        /// <summary>
        /// A number representing the current number of Asset Documents that are currently stored in the Agent as of the creationTime that the Agent published the Response Document.
        /// </summary>
        [XmlAttribute("assetCount")]
        [JsonPropertyName("assetCount")]
        public long AssetCount { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public bool AssetCountSpecified => AssetCount >= 0;

        /// <summary>
        /// A timestamp in 8601 format of the last update of the Device information for any device.
        /// </summary>
        [XmlAttribute("deviceModelChangeTime")]
        [JsonPropertyName("deviceModelChangeTime")]
        public string DeviceModelChangeTime { get; set; }

        /// <summary>
        /// A flag indicating that the Agent that published the Response Document is operating in a test mode.
        /// The contents of the Response Document may not be valid and SHOULD be used for testing and simulation purposes only.
        /// </summary>
        [XmlAttribute("testIndicator")]
        [JsonPropertyName("testIndicator")]
        public string TestIndicator { get; set; }

        /// <summary>
        /// CreationTime represents the time that an Agent published the Response Document.
        /// </summary>
        [XmlAttribute("creationTime")]
        [JsonPropertyName("creationTime")]
        public DateTime CreationTime { get; set; }
    }
}
