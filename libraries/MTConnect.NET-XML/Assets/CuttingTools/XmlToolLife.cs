// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.Xml.CuttingTools
{
    public class XmlToolLife
    {
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

        [XmlText]
        public double Value { get; set; }


        public IToolLife ToToolLife()
        {
            var toolLife = new ToolLife();
            toolLife.Value = Value;
            toolLife.Type = Type.ConvertEnum<ToolLifeType>();
            toolLife.CountDirection = CountDirection.ConvertEnum<CountDirectionType>();
            if (Warning != null) toolLife.Warning = Warning.ToDouble();
            if (Limit != null) toolLife.Limit = Limit.ToDouble();
            if (Initial != null) toolLife.Initial = Initial.ToDouble();
            return toolLife;
        }

        public static void WriteXml(XmlWriter writer, IEnumerable<IToolLife> toolLifes)
        {
            if (toolLifes != null)
            {
                foreach (var toolLife in toolLifes)
                {
                    writer.WriteStartElement("ToolLife");
                    writer.WriteAttributeString("type", toolLife.Type.ToString());
                    writer.WriteAttributeString("countDirection", toolLife.CountDirection.ToString());
                    if (toolLife.Warning != null) writer.WriteAttributeString("warning", toolLife.Warning.ToString());
                    if (toolLife.Limit != null) writer.WriteAttributeString("limit", toolLife.Limit.ToString());
                    if (toolLife.Initial != null) writer.WriteAttributeString("initial", toolLife.Initial.ToString());
                    writer.WriteString(toolLife.Value.ToString());
                    writer.WriteEndElement();
                }
            }
        }
    }
}