// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Headers;
using System;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Streams.Xml
{
    internal class XmlStreamsHeader
    {
        [XmlAttribute("instanceId")]
        public ulong InstanceId { get; set; }

        [XmlAttribute("version")]
        public string Version { get; set; }

        [XmlAttribute("sender")]
        public string Sender { get; set; }

        [XmlAttribute("bufferSize")]
        public ulong BufferSize { get; set; }

        [XmlAttribute("firstSequence")]
        public ulong FirstSequence { get; set; }

        [XmlAttribute("lastSequence")]
        public ulong LastSequence { get; set; }

        [XmlAttribute("nextSequence")]
        public ulong NextSequence { get; set; }

        [XmlAttribute("deviceModelChangeTime")]
        public string DeviceModelChangeTime { get; set; }

        [XmlAttribute("testIndicator")]
        public bool TestIndicator { get; set; }

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
            header.InstanceId = reader.GetAttribute("instanceId").ToULong();
            header.Version = reader.GetAttribute("version");
            header.Sender = reader.GetAttribute("sender");
            header.BufferSize = reader.GetAttribute("bufferSize").ToULong();
            header.FirstSequence = reader.GetAttribute("firstSequence").ToULong();
            header.LastSequence = reader.GetAttribute("lastSequence").ToULong();
            header.NextSequence = reader.GetAttribute("nextSequence").ToULong();
            header.DeviceModelChangeTime = reader.GetAttribute("deviceModelChangeTime");
            header.TestIndicator = reader.GetAttribute("testIndicator").ToBoolean();
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
                if (header.TestIndicator) writer.WriteAttributeString("testIndicator", header.TestIndicator.ToString());
                writer.WriteAttributeString("creationTime", header.CreationTime.ToString("o"));
                writer.WriteEndElement();
            }
        }
    }
}