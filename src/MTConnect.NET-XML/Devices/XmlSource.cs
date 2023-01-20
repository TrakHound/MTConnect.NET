// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.DataItems;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    public class XmlSource
    {
        [XmlAttribute("componentId")]
        public string ComponentId { get; set; }

        [XmlAttribute("dataItemId")]
        public string DataItemId { get; set; }

        [XmlAttribute("compositionId")]
        public string CompositionId { get; set; }

        [XmlText]
        public string Value { get; set; }


        public ISource ToSource()
        {
            var source = new Source();
            source.ComponentId = ComponentId;
            source.DataItemId = DataItemId;
            source.CompositionId = CompositionId;
            source.Value = Value;
            return source;
        }

        public static void WriteXml(XmlWriter writer, ISource source)
        {
            if (source != null)
            {
                writer.WriteStartElement("Source");
                if (!string.IsNullOrEmpty(source.ComponentId)) writer.WriteAttributeString("componentId", source.ComponentId);
                if (!string.IsNullOrEmpty(source.CompositionId)) writer.WriteAttributeString("compositionId", source.CompositionId);
                if (!string.IsNullOrEmpty(source.DataItemId)) writer.WriteAttributeString("dataItemId", source.DataItemId);
                if (!string.IsNullOrEmpty(source.Value)) writer.WriteString(source.Value);
                writer.WriteEndElement();
            }
        }
    }
}