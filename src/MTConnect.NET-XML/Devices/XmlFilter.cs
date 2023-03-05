// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.DataItems;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    public class XmlFilter
    {
        [XmlAttribute("type")]
        public DataItemFilterType Type { get; set; }

        [XmlText]
        public double Value { get; set; }


        public IFilter ToFilter()
        {
            var filter = new Filter();
            filter.Type = Type;
            filter.Value = Value;
            return filter;
        }

        public static void WriteXml(XmlWriter writer, IFilter filter)
        {
            if (filter != null)
            {
                writer.WriteStartElement("Filter");
                writer.WriteAttributeString("type", filter.Type.ToString());
                writer.WriteValue(filter.Value);
                writer.WriteEndElement();
            }
        }
    }
}