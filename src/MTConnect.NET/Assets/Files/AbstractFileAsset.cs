// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.Files
{
    /// <summary>
    /// An AbstractFile is an abstract Asset type model that contains the common properties of the File and FileArchetype types.
    /// </summary>
    [XmlRoot("AbstractFile")]
    public abstract class AbstractFileAsset<T> : Asset<T> where T : IAsset
    {
        /// <summary>
        /// The name of the file
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// The mime type of the file.
        /// </summary>
        [XmlAttribute("mediaType")]
        public string MediaType { get; set; }

        /// <summary>
        /// The category of application that will use this file.
        /// </summary>
        [XmlAttribute("applicationCategory")]
        public string ApplicationCategory { get; set; }

        /// <summary>
        /// The type of application that will use this file.
        /// </summary>
        [XmlAttribute("applicationType")]
        public string ApplicationType { get; set; }

        /// <summary>
        /// FileProperties organizes one or more FileProperty entities for Files.
        /// </summary>
        [XmlArray("FileProperties")]
        [XmlArrayItem("FileProperty", typeof(FileProperty))]
        public List<FileProperty> FileProperties { get; set; }

        [XmlIgnore]
        public bool FilePropertiesSpecified => !FileProperties.IsNullOrEmpty();

        /// <summary>
        /// FileComments organizes one or more FileComment entities for Files.   
        /// </summary>
        [XmlArray("FileComments")]
        [XmlArrayItem("FileComment", typeof(FileComment))]
        public List<FileComment> FileComments { get; set; }

        [XmlIgnore]
        public bool FileCommentsSpecified => !FileComments.IsNullOrEmpty();

    }
}
