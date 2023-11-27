// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.Files;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.Xml.Files
{
    public class XmlFileLocation
    {
        [XmlAttribute("href")]
        public string Href { get; set; }

        [XmlAttribute("xLinkType")]
        public string XLinkType { get; set; }


        public IFileLocation ToFileLocation()
        {
            var fileLocation = new FileLocation();
            fileLocation.Href = Href;
            fileLocation.XLinkType = XLinkType;
            return fileLocation;
        }

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