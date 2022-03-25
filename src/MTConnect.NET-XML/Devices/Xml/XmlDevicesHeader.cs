// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Headers;
using System;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    /// <summary>
    /// The Header element for an MTConnectDevices Response Document defines information regarding
    /// the creation of the document and the data storage capability of the Agent that generated the document.
    /// </summary>
    public class XmlDevicesHeader
    {
        /// <summary>
        /// A number indicating a specific instantiation of the buffer associated with the Agent that published the Response Document.   
        /// </summary>
        [XmlAttribute("instanceId")]
        public long InstanceId { get; set; }

        /// <summary>
        /// The major, minor, and revision number of the MTConnect Standard that defines the semantic data model that represents the content of the Response Document.
        /// It also includes the revision number of the schema associated with that specific semantic data model.
        /// </summary>
        [XmlAttribute("version")]
        public string Version { get; set; }

        /// <summary>
        /// An identification defining where the Agent that published the Response Document is installed or hosted.
        /// </summary>
        [XmlAttribute("sender")]
        public string Sender { get; set; }

        /// <summary>
        /// A value representing the maximum number of Data Entities that MAY be retained in the Agent that published the Response Document at any point in time.
        /// </summary>
        [XmlAttribute("bufferSize")]
        public long BufferSize { get; set; }

        /// <summary>
        /// A value representing the maximum number of Asset Documents that can be stored in the Agent that published the Response Document.   
        /// </summary>
        [XmlAttribute("assetBufferSize")]
        public long AssetBufferSize { get; set; }

        [XmlIgnore]
        public bool AssetBufferSizeSpecified => AssetBufferSize >= 0;

        /// <summary>
        /// A number representing the current number of Asset Documents that are currently stored in the Agent as of the creationTime that the Agent published the Response Document.
        /// </summary>
        [XmlAttribute("assetCount")]
        public long AssetCount { get; set; }

        [XmlIgnore]
        public bool AssetCountSpecified => AssetCount >= 0;

        /// <summary>
        /// A timestamp in 8601 format of the last update of the Device information for any device.
        /// </summary>
        [XmlAttribute("deviceModelChangeTime")]
        public string DeviceModelChangeTime { get; set; }

        /// <summary>
        /// A flag indicating that the Agent that published the Response Document is operating in a test mode.
        /// The contents of the Response Document may not be valid and SHOULD be used for testing and simulation purposes only.
        /// </summary>
        [XmlAttribute("testIndicator")]
        public string TestIndicator { get; set; }

        /// <summary>
        /// CreationTime represents the time that an Agent published the Response Document.
        /// </summary>
        [XmlAttribute("creationTime")]
        public DateTime CreationTime { get; set; }


        public XmlDevicesHeader() { }

        public XmlDevicesHeader(IMTConnectDevicesHeader header)
        {
            if (header != null)
            {
                InstanceId = header.InstanceId;
                Version = header.Version;
                Sender = header.Sender;
                BufferSize = header.BufferSize;
                AssetBufferSize = header.AssetBufferSize;
                AssetCount = header.AssetCount;
                DeviceModelChangeTime = header.DeviceModelChangeTime;
                TestIndicator = header.TestIndicator;
                CreationTime = header.CreationTime;
            }
        }

        public virtual IMTConnectDevicesHeader ToDevicesHeader()
        {
            var header = new MTConnectDevicesHeader();
            header.InstanceId = InstanceId;
            header.Version = Version;
            header.Sender = Sender;
            header.BufferSize = BufferSize;
            header.AssetBufferSize = AssetBufferSize;
            header.AssetCount = AssetCount;
            header.DeviceModelChangeTime = DeviceModelChangeTime;
            header.TestIndicator = TestIndicator;
            header.CreationTime = CreationTime;
            return header;
        }
    }
}
