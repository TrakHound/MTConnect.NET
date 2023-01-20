// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Headers;
using System;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.Xml
{
    public class XmlAssetsHeader
    {
        [XmlAttribute("instanceId")]
        public long InstanceId { get; set; }

        [XmlAttribute("version")]
        public string Version { get; set; }

        [XmlAttribute("sender")]
        public string Sender { get; set; }

        [XmlAttribute("assetBufferSize")]
        public long AssetBufferSize { get; set; }

        [XmlAttribute("assetCount")]
        public long AssetCount { get; set; }

        [XmlAttribute("deviceModelChangeTime")]
        public string DeviceModelChangeTime { get; set; }

        [XmlAttribute("testIndicator")]
        public string TestIndicator { get; set; }

        [XmlAttribute("creationTime")]
        public DateTime CreationTime { get; set; }


        public virtual IMTConnectAssetsHeader ToErrorHeader()
        {
            var header = new MTConnectAssetsHeader();
            header.InstanceId = InstanceId;
            header.Version = Version;
            header.Sender = Sender;
            header.AssetBufferSize = AssetBufferSize;
            header.AssetCount = AssetCount;
            header.DeviceModelChangeTime = DeviceModelChangeTime;
            header.TestIndicator = TestIndicator;
            header.CreationTime = CreationTime;
            return header;
        }

        public static void WriteXml(XmlWriter writer, IMTConnectAssetsHeader header)
        {
            if (header != null)
            {
                writer.WriteStartElement("Header");
                writer.WriteAttributeString("instanceId", header.InstanceId.ToString());
                writer.WriteAttributeString("version", header.Version.ToString());
                writer.WriteAttributeString("sender", header.Sender);
                writer.WriteAttributeString("assetBufferSize", header.AssetBufferSize.ToString());
                writer.WriteAttributeString("assetCount", header.AssetCount.ToString());
                writer.WriteAttributeString("deviceModelChangeTime", header.DeviceModelChangeTime);
                if (!string.IsNullOrEmpty(header.TestIndicator)) writer.WriteAttributeString("testIndicator", header.TestIndicator);
                writer.WriteAttributeString("creationTime", header.CreationTime.ToString("o"));
                writer.WriteEndElement();
            }
        }
    }
}
