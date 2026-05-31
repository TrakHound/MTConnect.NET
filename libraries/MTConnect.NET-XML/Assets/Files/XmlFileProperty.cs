// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.Files;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.Xml.Files
{
    /// <summary>
    /// XML serialization surrogate for a single <c>FileProperty</c>, a
    /// vendor-defined name/value pair describing a File asset.
    /// </summary>
    public class XmlFileProperty
    {
        /// <summary>
        /// The property name, carried by the <c>name</c> attribute.
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// The property value, carried as the element's text content.
        /// </summary>
        [XmlText]
        public string Value { get; set; }


        /// <summary>
        /// Converts this surrogate to a strongly-typed <see cref="FileProperty"/>.
        /// </summary>
        public IFileProperty ToFileProperty()
        {
            var fileProperty = new FileProperty();
            fileProperty.Name = Name;
            fileProperty.Value = Value;
            return fileProperty;
        }

        /// <summary>
        /// Writes the <c>FileProperties</c> container with one
        /// <c>FileProperty</c> element per property; nothing is written when
        /// the collection is empty.
        /// </summary>
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