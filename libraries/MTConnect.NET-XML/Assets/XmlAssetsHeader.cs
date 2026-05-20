// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Headers;
using System;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.Xml
{
    /// <summary>
    /// XML serialization surrogate for the <c>Header</c> of an MTConnectAssets
    /// response document. Mirrors the on-the-wire element and converts to the
    /// strongly-typed <see cref="IMTConnectAssetsHeader"/> model.
    /// </summary>
    public class XmlAssetsHeader
    {
        /// <summary>
        /// The agent instance identifier; changes whenever the agent restarts.
        /// </summary>
        [XmlAttribute("instanceId")]
        public ulong InstanceId { get; set; }

        /// <summary>
        /// The version of the MTConnect Standard the document conforms to.
        /// </summary>
        [XmlAttribute("version")]
        public string Version { get; set; }

        /// <summary>
        /// The identifier of the agent that produced the document.
        /// </summary>
        [XmlAttribute("sender")]
        public string Sender { get; set; }

        /// <summary>
        /// The maximum number of assets the agent can store.
        /// </summary>
        [XmlAttribute("assetBufferSize")]
        public ulong AssetBufferSize { get; set; }

        /// <summary>
        /// The current number of assets the agent is storing.
        /// </summary>
        [XmlAttribute("assetCount")]
        public ulong AssetCount { get; set; }

        /// <summary>
        /// The time the device model last changed, as the raw attribute text.
        /// </summary>
        [XmlAttribute("deviceModelChangeTime")]
        public string DeviceModelChangeTime { get; set; }

        /// <summary>
        /// Whether the agent is operating in a test capacity rather than
        /// production.
        /// </summary>
        [XmlAttribute("testIndicator")]
        public bool TestIndicator { get; set; }

        /// <summary>
        /// Whether the agent validates documents against the MTConnect schema.
        /// </summary>
        [XmlAttribute("validation")]
        public bool Validation { get; set; }

        /// <summary>
        /// The time the document was created.
        /// </summary>
        [XmlAttribute("creationTime")]
        public DateTime CreationTime { get; set; }


        /// <summary>
        /// Converts this surrogate into the strongly-typed
        /// <see cref="IMTConnectAssetsHeader"/>.
        /// </summary>
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
            header.Validation = Validation;
            header.CreationTime = CreationTime;
            return header;
        }

        /// <summary>
        /// Writes the given <see cref="IMTConnectAssetsHeader"/> to
        /// <paramref name="writer"/> as a <c>Header</c> element, omitting the
        /// test and validation indicators when they are not set.
        /// </summary>
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
                if (header.TestIndicator) writer.WriteAttributeString("testIndicator", header.TestIndicator.ToString());
                if (header.Validation) writer.WriteAttributeString("validation", header.Validation.ToString());
                writer.WriteAttributeString("creationTime", header.CreationTime.ToString("o"));
                writer.WriteEndElement();
            }
        }
    }
}