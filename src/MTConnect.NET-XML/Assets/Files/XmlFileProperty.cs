// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.Files;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.Xml.Files
{
    public class XmlFileProperty
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlText]
        public string Value { get; set; }


        public IFileProperty ToFileProperty()
        {
            var fileProperty = new FileProperty();
            fileProperty.Name = Name;
            fileProperty.Value = Value;
            return fileProperty;
        }

        public static void WriteXml(XmlWriter writer, IEnumerable<IFileProperty> fileProperties)
        {
            if (!fileProperties.IsNullOrEmpty())
            {
                writer.WriteStartElement("FileProperties");

                foreach (var fileProperty in fileProperties)
                {
                    writer.WriteStartElement("FileProperty");
                    writer.WriteAttributeString("name", fileProperty.Name);
                    writer.WriteString(fileProperty.Value);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
        }
    }
}