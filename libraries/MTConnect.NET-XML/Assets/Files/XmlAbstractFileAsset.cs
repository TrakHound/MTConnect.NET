// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Xml.Serialization;

namespace MTConnect.Assets.Xml.Files
{
    /// <summary>
    /// Base XML serialization surrogate shared by the File and FileArchetype
    /// asset surrogates, carrying the file metadata attributes and the optional
    /// file property and comment collections.
    /// </summary>
    public abstract class XmlAbstractFileAsset
    {
        /// <summary>
        /// The file name, carried by the <c>name</c> attribute.
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// The MIME media type of the file, carried by the <c>mediaType</c>
        /// attribute.
        /// </summary>
        [XmlAttribute("mediaType")]
        public string MediaType { get; set; }

        /// <summary>
        /// The functional category of the file (for example
        /// <c>INSTALLATION</c> or <c>DESIGN</c>), carried by the
        /// <c>applicationCategory</c> attribute.
        /// </summary>
        [XmlAttribute("applicationCategory")]
        public string ApplicationCategory { get; set; }

        /// <summary>
        /// The kind of application the file targets, carried by the
        /// <c>applicationType</c> attribute.
        /// </summary>
        [XmlAttribute("applicationType")]
        public string ApplicationType { get; set; }

        /// <summary>
        /// Vendor-defined name/value file properties, serialized within the
        /// <c>FileProperties</c> element.
        /// </summary>
        [XmlElement("FileProperties")]
        public List<XmlFileProperty> FileProperties { get; set; }

        /// <summary>
        /// Timestamped comments associated with the file, serialized within the
        /// <c>FileComments</c> element.
        /// </summary>
        [XmlElement("FileComments")]
        public List<XmlFileComment> FileComments { get; set; }
    }
}