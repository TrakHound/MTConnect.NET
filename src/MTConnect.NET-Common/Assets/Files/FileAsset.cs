// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

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
    /// The File Asset is an AbstractFile with information about the File instance and its URL.
    /// </summary>
    [XmlRoot("File")]
    public class FileAsset : AbstractFileAsset<FileAsset>
    {
        public const string TypeId = "File";


        /// <summary>
        /// The size of the file in bytes.
        /// </summary>
        [XmlAttribute("size")]
        [JsonPropertyName("size")]
        public int Size { get; set; }

        /// <summary>
        /// The version identifier of the file.
        /// </summary>
        [XmlAttribute("versionId")]
        [JsonPropertyName("versionId")]
        public string VersionId { get; set; }

        /// <summary>
        /// The state of the file.
        /// </summary>
        [XmlAttribute("state")]
        [JsonPropertyName("state")]
        public FileState State { get; set; }

        /// <summary>
        /// The URL reference to the file location. 
        /// </summary>
        [XmlElement("FileLocation")]
        [JsonPropertyName("fileLocation")]
        public FileLocation FileLocation { get; set; }

        /// <summary>
        /// A secure hash of the file.
        /// </summary>
        [XmlElement("Signature")]
        [JsonPropertyName("signature")]
        public string Signature { get; set; }

        /// <summary>
        /// The public key used to verify the signature.
        /// </summary>
        [XmlElement("PublicKey")]
        [JsonPropertyName("publicKey")]
        public string PublicKey { get; set; }

        /// <summary>
        /// Destinations organizes one or more Destination elements.
        /// </summary>
        [XmlArray("Destinations")]
        [XmlArrayItem("Destination", typeof(Destination))]
        [JsonPropertyName("destinations")]
        public List<Destination> Destinations { get; set; }

        [XmlIgnore]
        public bool DestinationsSpecified => !Destinations.IsNullOrEmpty();

        /// <summary>
        /// The time the file was created.
        /// </summary>
        [XmlElement("CreationTime")]
        [JsonPropertyName("creationTime")]
        public DateTime CreationTime { get; set; }

        [XmlIgnore]
        public bool CreationTimeSpecified => CreationTime > DateTime.MinValue;

        /// <summary>
        /// The time the file was modified.
        /// </summary>
        [XmlElement("ModificationTime")]
        [JsonPropertyName("modificationTime")]
        public DateTime ModificationTime { get; set; }

        [XmlIgnore]
        public bool ModificationTimeSpecified => ModificationTime > DateTime.MinValue;


        public FileAsset()
        {
            Type = TypeId;
        }


        public override IAsset Process(Version mtconnectVersion)
        {
            if (Size <= 0) return null;
            if (string.IsNullOrEmpty(VersionId)) return null;

            return base.Process(mtconnectVersion);
        }

        public override AssetValidationResult IsValid(Version mtconnectVersion)
        {
            var baseResult = base.IsValid(mtconnectVersion);
            var message = baseResult.Message;
            var result = baseResult.IsValid;

            if (baseResult.IsValid)
            {
                if (Size <= 0)
                {
                    message = "Size property is Required and must be greater than 0";
                    result = false;
                }
                else if (string.IsNullOrEmpty(VersionId))
                {
                    message = "VersionId property is Required";
                    result = false;
                }
                else if (CreationTime <= DateTime.MinValue)
                {
                    message = "CreationTime property is Required";
                    result = false;
                }
                else if (FileLocation == null)
                {
                    message = "FileLocation is Required";
                    result = false;
                }
                else
                {
                    if (string.IsNullOrEmpty(FileLocation.Href))
                    {
                        message = "FileLocation Href property is Required";
                        result = false;
                    }
                }
            }

            return new AssetValidationResult(result, message);
        }
    }
}
