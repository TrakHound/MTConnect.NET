// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.Files;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.Xml.Files
{
    [XmlRoot("File")]
    public class XmlFileAsset : XmlAsset
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("mediaType")]
        public string MediaType { get; set; }

        [XmlAttribute("applicationCategory")]
        public string ApplicationCategory { get; set; }

        [XmlAttribute("applicationType")]
        public string ApplicationType { get; set; }

        [XmlArray("FileProperties")]
        [XmlArrayItem("FileProperty")]
        public List<XmlFileProperty> FileProperties { get; set; }

        [XmlArray("FileComments")]
        [XmlArrayItem("FileComment")]
        public List<XmlFileComment> FileComments { get; set; }


        [XmlAttribute("size")]
        public int Size { get; set; }

        [XmlAttribute("versionId")]
        public string VersionId { get; set; }

        [XmlAttribute("state")]
        public string State { get; set; }

        [XmlElement("FileLocation")]
        public XmlFileLocation FileLocation { get; set; }

        [XmlElement("Signature")]
        public string Signature { get; set; }

        [XmlElement("PublicKey")]
        public string PublicKey { get; set; }

        [XmlArray("Destinations")]
        [XmlArrayItem("Destination")]
        public List<XmlDestination> Destinations { get; set; }

        [XmlElement("CreationTime")]
        public string CreationTime { get; set; }

        [XmlElement("ModificationTime")]
        public string ModificationTime { get; set; }


        public override IFileAsset ToAsset()
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

        public static void WriteXml(XmlWriter writer, IAsset asset)
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