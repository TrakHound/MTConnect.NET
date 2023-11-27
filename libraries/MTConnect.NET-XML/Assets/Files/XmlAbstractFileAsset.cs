// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Xml.Serialization;

namespace MTConnect.Assets.Xml.Files
{
    public abstract class XmlAbstractFileAsset
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("mediaType")]
        public string MediaType { get; set; }

        [XmlAttribute("applicationCategory")]
        public string ApplicationCategory { get; set; }

        [XmlAttribute("applicationType")]
        public string ApplicationType { get; set; }

        [XmlElement("FileProperties")]
        public List<XmlFileProperty> FileProperties { get; set; }

        [XmlElement("FileComments")]
        public List<XmlFileComment> FileComments { get; set; }
    }
}