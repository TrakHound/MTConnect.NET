// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.Xml.CuttingTools
{
    public class XmlCuttingToolArchetypeReference
    {
        [XmlAttribute("source")]
        public string Source { get; set; }

        [XmlText]
        public string Value { get; set; }


        public ICuttingToolArchetypeReference ToCuttingToolArchetypeReference()
        {
            var location = new CuttingToolArchetypeReference();
            location.Source = Source;
            location.Value = Value;
            return location;
        }

        public static void WriteXml(XmlWriter writer, ICuttingToolArchetypeReference cuttingToolArchetypeReference)
        {
            if (cuttingToolArchetypeReference != null)
            {
                writer.WriteStartElement("CuttingToolArchetypeReference");
                if (!string.IsNullOrEmpty(cuttingToolArchetypeReference.Source)) writer.WriteAttributeString("source", cuttingToolArchetypeReference.Source);
                writer.WriteString(cuttingToolArchetypeReference.Value);
                writer.WriteEndElement();
            }
        }
    }
}