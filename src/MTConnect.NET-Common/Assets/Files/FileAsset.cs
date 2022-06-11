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
        public int Size { get; set; }

        /// <summary>
        /// The version identifier of the file.
        /// </summary>
        [XmlAttribute("versionId")]
        public string VersionId { get; set; }

        /// <summary>
        /// The state of the file.
        /// </summary>
        [XmlAttribute("state")]
        public string State { get; set; }

        /// <summary>
        /// A secure hash of the file.
        /// </summary>
        [XmlElement("Signature")]
        public string Signature { get; set; }

        /// <summary>
        /// The public key used to verify the signature.
        /// </summary>
        [XmlElement("PublicKey")]
        public string PublicKey { get; set; }

        /// <summary>
        /// The time the file was created.
        /// </summary>
        [XmlElement("CreationTime")]
        public DateTime CreationTime { get; set; }

        [XmlIgnore]
        public bool CreationTimeSpecified => CreationTime > DateTime.MinValue;

        /// <summary>
        /// The time the file was modified.
        /// </summary>
        [XmlElement("ModificationTime")]
        public DateTime ModificationTime { get; set; }

        [XmlIgnore]
        public bool ModificationTimeSpecified => ModificationTime > DateTime.MinValue;

        /// <summary>
        /// The URL reference to the file location. 
        /// </summary>
        [XmlElement("FileLocation")]
        public FileLocation FileLocation { get; set; }

        /// <summary>
        /// Destinations organizes one or more Destination elements.
        /// </summary>
        [XmlArray("Destinations")]
        [XmlArrayItem("Destination", typeof(Destination))]
        public List<Destination> Destinations { get; set; }

        [XmlIgnore]
        public bool DestinationsSpecified => !Destinations.IsNullOrEmpty();


        public FileAsset()
        {
            Type = TypeId;
        }
    }
}
