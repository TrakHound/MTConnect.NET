// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.Xml.CuttingTools
{
    public class XmlCuttingToolDefinition
    {
        [XmlAttribute("format")]
        public string Format { get; set; }


        public ICuttingToolDefinition ToCuttingToolDefinition()
        {
            var cuttingToolDefinition = new CuttingToolDefinition();
            cuttingToolDefinition.Format = Format.ConvertEnum<FormatType>();
            return cuttingToolDefinition;
        }

        public static void WriteXml(XmlWriter writer, ICuttingToolDefinition cuttingToolDefinition)
        {
            if (cuttingToolDefinition != null)
            {
                writer.WriteStartElement("CuttingToolDefinition");
                writer.WriteAttributeString("format", cuttingToolDefinition.Format.ToString());
                writer.WriteEndElement();
            }
        }
    }
}