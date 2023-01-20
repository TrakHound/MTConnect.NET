// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.Files
{
    /// <summary>
    /// An AbstractFile is an abstract Asset type model that contains the common properties of the File and FileArchetype types.
    /// </summary>
    [XmlRoot("AbstractFile")]
    public abstract class AbstractFileAsset<T> : Asset where T : IAsset
    {
        /// <summary>
        /// The name of the file
        /// </summary>
        [XmlAttribute("name")]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The mime type of the file.
        /// </summary>
        [XmlAttribute("mediaType")]
        [JsonPropertyName("mediaType")]
        public string MediaType { get; set; }

        /// <summary>
        /// The category of application that will use this file.
        /// </summary>
        [XmlAttribute("applicationCategory")]
        [JsonPropertyName("applicationCategory")]
        public ApplicationCategory ApplicationCategory { get; set; }

        /// <summary>
        /// The type of application that will use this file.
        /// </summary>
        [XmlAttribute("applicationType")]
        [JsonPropertyName("applicationType")]
        public ApplicationType ApplicationType { get; set; }

        /// <summary>
        /// FileProperties organizes one or more FileProperty entities for Files.
        /// </summary>
        [XmlArray("FileProperties")]
        [XmlArrayItem("FileProperty", typeof(FileProperty))]
        [JsonPropertyName("fileProperties")]
        public List<FileProperty> FileProperties { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public bool FilePropertiesSpecified => !FileProperties.IsNullOrEmpty();

        /// <summary>
        /// FileComments organizes one or more FileComment entities for Files.   
        /// </summary>
        [XmlArray("FileComments")]
        [XmlArrayItem("FileComment", typeof(FileComment))]
        [JsonPropertyName("fileComments")]
        public List<FileComment> FileComments { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public bool FileCommentsSpecified => !FileComments.IsNullOrEmpty();


        public override IAsset Process(Version mtconnectVersion)
        {
            if (mtconnectVersion != null && mtconnectVersion >= MTConnectVersions.Version17)
            {
                return this;
            }

            return null;
        }

        public override AssetValidationResult IsValid(Version mtconnectVersion)
        {
            var message = "";
            var result = true;

            if (string.IsNullOrEmpty(Name))
            {
                message = "Name property is Required";
                result = false;
            }
            else if (string.IsNullOrEmpty(MediaType))
            {
                message = "MediaType property is Required";
                result = false;
            }

            return new AssetValidationResult(result, message);
        }
    }
}
