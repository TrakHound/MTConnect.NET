// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.Xml.CuttingTools
{
    public class XmlReconditionCount
    {
        [XmlAttribute("maximumCount")]
        public string MaximumCount { get; set; }

        [XmlText]
        public string Value { get; set; }


        public IReconditionCount ToReconditionCount()
        {
            var reconditionCount = new ReconditionCount();
            if (MaximumCount != null) reconditionCount.MaximumCount = MaximumCount.ToInt();
            reconditionCount.Value = Value.ToInt();
            return reconditionCount;
        }

        public static void WriteXml(XmlWriter writer, IReconditionCount reconditionCount)
        {
            if (reconditionCount != null)
            {
                writer.WriteStartElement("ReconditionCount");
                if (reconditionCount.MaximumCount != null) writer.WriteAttributeString("maximumCount", reconditionCount.MaximumCount.ToString());
                writer.WriteString(reconditionCount.Value.ToString());
                writer.WriteEndElement();
            }
        }
    }
}