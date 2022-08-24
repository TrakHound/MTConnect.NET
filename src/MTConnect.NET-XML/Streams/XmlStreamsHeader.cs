// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Headers;
using System;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Streams.Xml
{
    internal class XmlStreamsHeader
    {
        [XmlAttribute("instanceId")]
        public long InstanceId { get; set; }

        [XmlAttribute("version")]
        public string Version { get; set; }

        [XmlAttribute("sender")]
        public string Sender { get; set; }

        [XmlAttribute("bufferSize")]
        public long BufferSize { get; set; }

        [XmlAttribute("firstSequence")]
        public long FirstSequence { get; set; }

        [XmlAttribute("lastSequence")]
        public long LastSequence { get; set; }

        [XmlAttribute("nextSequence")]
        public long NextSequence { get; set; }

        [XmlAttribute("deviceModelChangeTime")]
        public string DeviceModelChangeTime { get; set; }

        [XmlAttribute("testIndicator")]
        public string TestIndicator { get; set; }

        [XmlAttribute("creationTime")]
        public DateTime CreationTime { get; set; }


        public virtual IMTConnectStreamsHeader ToDevicesHeader()
        {
            var header = new MTConnectStreamsHeader();
            header.InstanceId = InstanceId;
            header.Version = Version;
            header.Sender = Sender;
            header.BufferSize = BufferSize;
            header.FirstSequence = FirstSequence;
            header.LastSequence = LastSequence;
            header.NextSequence = NextSequence;
            header.DeviceModelChangeTime = DeviceModelChangeTime;
            header.TestIndicator = TestIndicator;
            header.CreationTime = CreationTime;
            return header;
        }


        public static IMTConnectStreamsHeader ReadXml(XmlReader reader)
        {
            var header = new MTConnectStreamsHeader();
            header.InstanceId = reader.GetAttribute("instanceId").ToLong();
            header.Version = reader.GetAttribute("version");
            header.Sender = reader.GetAttribute("sender");
            header.BufferSize = reader.GetAttribute("bufferSize").ToLong();
            header.FirstSequence = reader.GetAttribute("firstSequence").ToLong();
            header.LastSequence = reader.GetAttribute("lastSequence").ToLong();
            header.NextSequence = reader.GetAttribute("nextSequence").ToLong();
            header.DeviceModelChangeTime = reader.GetAttribute("deviceModelChangeTime");
            header.TestIndicator = reader.GetAttribute("testIndicator");
            header.CreationTime = reader.GetAttribute("creationTime").ToDateTime();
            return header;
        }

        public static void WriteXml(XmlWriter writer, IMTConnectStreamsHeader header)
        {
            if (header != null)
            {
                writer.WriteStartElement("Header");
                writer.WriteAttributeString("instanceId", header.InstanceId.ToString());
                writer.WriteAttributeString("version", header.Version.ToString());
                writer.WriteAttributeString("sender", header.Sender);
                writer.WriteAttributeString("bufferSize", header.BufferSize.ToString());
                writer.WriteAttributeString("firstSequence", header.FirstSequence.ToString());
                writer.WriteAttributeString("lastSequence", header.LastSequence.ToString());
                writer.WriteAttributeString("nextSequence", header.NextSequence.ToString());
                writer.WriteAttributeString("deviceModelChangeTime", header.DeviceModelChangeTime);
                if (!string.IsNullOrEmpty(header.TestIndicator)) writer.WriteAttributeString("testIndicator", header.TestIndicator);
                writer.WriteAttributeString("creationTime", header.CreationTime.ToString("o"));
                writer.WriteEndElement();
            }
        }
    }
}
