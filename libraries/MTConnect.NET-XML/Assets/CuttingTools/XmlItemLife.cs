// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.Xml.CuttingTools
{
    public class XmlItemLife
    {
        [XmlText]
        public double Value { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlAttribute("countDirection")]
        public string CountDirection { get; set; }

        [XmlAttribute("warning")]
        public string Warning { get; set; }

        [XmlAttribute("limit")]
        public string Limit { get; set; }

        [XmlAttribute("initial")]
        public string Initial { get; set; }


        public IItemLife ToItemLife()
        {
            var itemLife = new ItemLife();
            itemLife.Value = Value;
            itemLife.Type = Type.ConvertEnum<ToolLifeType>();
            itemLife.CountDirection = CountDirection.ConvertEnum<CountDirectionType>();
            if (Warning != null) itemLife.Warning = Warning.ToDouble();
            if (Limit != null) itemLife.Limit = Limit.ToDouble();
            if (Initial != null) itemLife.Initial = Initial.ToDouble();
            return itemLife;
        }

        public static void WriteXml(XmlWriter writer, IEnumerable<IItemLife> itemLifes)
        {
            if (itemLifes != null)
            {
                foreach (var itemLife in itemLifes)
                {
                    writer.WriteStartElement("ItemLife");
                    writer.WriteAttributeString("type", itemLife.Type.ToString());
                    writer.WriteAttributeString("countDirection", itemLife.CountDirection.ToString());
                    if (itemLife.Warning != null) writer.WriteAttributeString("warning", itemLife.Warning.ToString());
                    if (itemLife.Limit != null) writer.WriteAttributeString("limit", itemLife.Limit.ToString());
                    if (itemLife.Initial != null) writer.WriteAttributeString("initial", itemLife.Initial.ToString());
                    writer.WriteString(itemLife.Value.ToString());
                    writer.WriteEndElement();
                }
            }
        }
    }
}