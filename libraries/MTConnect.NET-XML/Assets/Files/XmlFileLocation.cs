// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.Files;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.Xml.Files
{
    /// <summary>
    /// XML serialization surrogate for a File asset <c>FileLocation</c>, the
    /// XLink that points to where the file content can be retrieved.
    /// </summary>
    public class XmlFileLocation
    {
        /// <summary>
        /// The URI of the file content, carried by the <c>href</c> attribute.
        /// </summary>
        [XmlAttribute("href")]
        public string Href { get; set; }

        /// <summary>
        /// The XLink type of the reference, carried by the <c>xlink:type</c>
        /// attribute.
        /// </summary>
        [XmlAttribute("xLinkType")]
        public string XLinkType { get; set; }


        /// <summary>
        /// Converts this surrogate to a strongly-typed <see cref="FileLocation"/>.
        /// </summary>
        public IFileLocation ToFileLocation()
        {
            var fileLocation = new FileLocation();
            fileLocation.Href = Href;
            fileLocation.XLinkType = XLinkType;
            return fileLocation;
        }

        /// <summary>
        /// Writes the <c>FileLocation</c> element, emitting the required
        /// <c>href</c> attribute and the optional <c>xlink:type</c> attribute.
        /// </summary>
        public static void WriteXml(XmlWriter writer, IFileLocation fileLocation)
        {
            if (fileLocation != null)
            {
                writer.WriteStartElement("FileLocation");
                writer.WriteAttributeString("href", fileLocation.Href);
                if (!string.IsNullOrEmpty(fileLocation.XLinkType)) writer.WriteAttributeString("xlink:type", fileLocation.XLinkType);
                writer.WriteEndElement();
            }
        }
    }
}