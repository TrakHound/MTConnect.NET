// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.Files;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.Xml.Files
{
    /// <summary>
    /// XML serialization surrogate for an MTConnect <c>File</c> asset. Mirrors
    /// the on-the-wire element so the XML serializer can read and write it, then
    /// converts to and from the strongly-typed <see cref="FileAsset"/> model.
    /// </summary>
    [XmlRoot("File")]
    public class XmlFileAsset : XmlAsset
    {
        /// <summary>
        /// The name of the file.
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// The MIME media type of the file's contents.
        /// </summary>
        [XmlAttribute("mediaType")]
        public string MediaType { get; set; }

        /// <summary>
        /// The category of application that consumes the file, as the raw
        /// attribute text.
        /// </summary>
        [XmlAttribute("applicationCategory")]
        public string ApplicationCategory { get; set; }

        /// <summary>
        /// The type of application the file represents, as the raw attribute
        /// text.
        /// </summary>
        [XmlAttribute("applicationType")]
        public string ApplicationType { get; set; }

        /// <summary>
        /// The key/value properties that further describe the file.
        /// </summary>
        [XmlArray("FileProperties")]
        [XmlArrayItem("FileProperty")]
        public List<XmlFileProperty> FileProperties { get; set; }

        /// <summary>
        /// The free-form comments associated with the file.
        /// </summary>
        [XmlArray("FileComments")]
        [XmlArrayItem("FileComment")]
        public List<XmlFileComment> FileComments { get; set; }


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
        /// The lifecycle state of the file (for example <c>PRODUCTION</c> or
        /// <c>EXPERIMENTAL</c>), as the raw attribute text.
        /// </summary>
        [XmlAttribute("state")]
        public string State { get; set; }

        /// <summary>
        /// Where the file's contents can be retrieved from.
        /// </summary>
        [XmlElement("FileLocation")]
        public XmlFileLocation FileLocation { get; set; }

        /// <summary>
        /// The cryptographic signature used to verify the file's integrity.
        /// </summary>
        [XmlElement("Signature")]
        public string Signature { get; set; }

        /// <summary>
        /// The public key used to verify the file's signature.
        /// </summary>
        [XmlElement("PublicKey")]
        public string PublicKey { get; set; }

        /// <summary>
        /// The destinations the file is intended to be distributed to.
        /// </summary>
        [XmlArray("Destinations")]
        [XmlArrayItem("Destination")]
        public List<XmlDestination> Destinations { get; set; }

        /// <summary>
        /// The time the file was created, as the raw element text.
        /// </summary>
        [XmlElement("CreationTime")]
        public string CreationTime { get; set; }

        /// <summary>
        /// The time the file was last modified, as the raw element text.
        /// </summary>
        [XmlElement("ModificationTime")]
        public string ModificationTime { get; set; }


        /// <summary>
        /// Converts this surrogate into the strongly-typed
        /// <see cref="FileAsset"/>, parsing each raw value and projecting the
        /// nested collections into their model representations.
        /// </summary>
        public override IAsset ToAsset()
        {
            var asset = new FileAsset();

            asset.AssetId = AssetId;
            asset.Timestamp = Timestamp;
            asset.DeviceUuid = DeviceUuid;
            asset.Removed = Removed;

            //if (Description != null) asset.Description = Description.ToDescription();

            asset.Size = Size;
            asset.VersionId = VersionId;
            asset.State = State.ConvertEnum<FileState>();
            asset.Signature = Signature;
            asset.PublicKey = PublicKey;
            asset.CreationTime = CreationTime.ToDateTime();
            if (!string.IsNullOrEmpty(ModificationTime)) asset.ModificationTime = ModificationTime.ToDateTime();
            asset.Name = Name;
            asset.MediaType = MediaType;
            asset.ApplicationCategory = ApplicationCategory.ConvertEnum<ApplicationCategory>();
            asset.ApplicationType = ApplicationType.ConvertEnum<ApplicationType>();

            if (FileLocation != null) asset.Location = FileLocation.ToFileLocation();

            // FileProperties
            if (!FileProperties.IsNullOrEmpty())
            {
                var fileProperties = new List<IFileProperty>();
                foreach (var fileProperty in FileProperties)
                {
                    fileProperties.Add(fileProperty.ToFileProperty());
                }
                asset.FileProperties = fileProperties;
            }

            // FileComments
            if (!FileComments.IsNullOrEmpty())
            {
                var fileComments = new List<IFileComment>();
                foreach (var fileComment in FileComments)
                {
                    fileComments.Add(fileComment.ToFileComment());
                }
                asset.FileComments = fileComments;
            }

            // Destinations
            if (!Destinations.IsNullOrEmpty())
            {
                var destinations = new List<IDestination>();
                foreach (var destination in Destinations)
                {
                    destinations.Add(destination.ToDestination());
                }
                asset.Destinations = destinations;
            }

            return asset;
        }

        /// <summary>
        /// Writes the given <see cref="IFileAsset"/> to <paramref name="writer"/>
        /// as a <c>File</c> element, omitting optional values and collections
        /// that are not set.
        /// </summary>
        public static new void WriteXml(XmlWriter writer, IAsset asset)
        {
            if (asset != null)
            {
                var file = (IFileAsset)asset;

                writer.WriteStartElement("File");

                WriteCommonXml(writer, asset);

                // Write Properties
                writer.WriteAttributeString("name", file.Name);
                writer.WriteAttributeString("mediaType", file.MediaType);
                writer.WriteAttributeString("applicationCategory", file.ApplicationCategory.ToString());
                writer.WriteAttributeString("applicationType", file.ApplicationType.ToString());

                writer.WriteAttributeString("size", file.Size.ToString());
                writer.WriteAttributeString("versionId", file.VersionId);
                writer.WriteAttributeString("state", file.State.ToString());

                // CreationTime
                writer.WriteStartElement("CreationTime");
                writer.WriteString(file.CreationTime.ToString("o"));
                writer.WriteEndElement();

                // ModificationTime
                if (file.ModificationTime != null)
                {
                    writer.WriteStartElement("ModificationTime");
                    writer.WriteString(file.ModificationTime?.ToString("o"));
                    writer.WriteEndElement();
                }

                // Write Location
                if (file.Location != null)
                {
                    XmlFileLocation.WriteXml(writer, file.Location);
                }

                // Write FileComments
                if (!file.FileComments.IsNullOrEmpty())
                {
                    XmlFileComment.WriteXml(writer, file.FileComments);
                }

                // Write FileProperties
                if (!file.FileProperties.IsNullOrEmpty())
                {
                    XmlFileProperty.WriteXml(writer, file.FileProperties);
                }

                // Write Destinations
                if (!file.Destinations.IsNullOrEmpty())
                {
                    XmlDestination.WriteXml(writer, file.Destinations);
                }

                writer.WriteEndElement();
            }
        }
    }
}