// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Files
{
    public class JsonFileAsset
    {
        [JsonPropertyName("assetId")]
        public string AssetId { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonPropertyName("deviceUuid")]
        public string DeviceUuid { get; set; }

        [JsonPropertyName("removed")]
        public bool Removed { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }


        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("mediaType")]
        public string MediaType { get; set; }

        [JsonPropertyName("applicationCategory")]
        public string ApplicationCategory { get; set; }

        [JsonPropertyName("applicationType")]
        public string ApplicationType { get; set; }

        [JsonPropertyName("fileProperties")]
        public IEnumerable<JsonFileProperty> FileProperties { get; set; }

        [JsonPropertyName("fileComments")]
        public IEnumerable<JsonFileComment> FileComments { get; set; }


        [JsonPropertyName("size")]
        public int Size { get; set; }

        [JsonPropertyName("versionId")]
        public string VersionId { get; set; }

        [JsonPropertyName("state")]
        public string State { get; set; }

        [JsonPropertyName("fileLocation")]
        public JsonFileLocation FileLocation { get; set; }

        [JsonPropertyName("signature")]
        public string Signature { get; set; }

        [JsonPropertyName("publicKey")]
        public string PublicKey { get; set; }

        [JsonPropertyName("destinations")]
        public IEnumerable<JsonDestination> Destinations { get; set; }

        [JsonPropertyName("creationTime")]
        public DateTime CreationTime { get; set; }

        [JsonPropertyName("modificationTime")]
        public DateTime ModificationTime { get; set; }


        public JsonFileAsset() { }

        public JsonFileAsset(FileAsset asset)
        {
            if (asset != null)
            {
                AssetId = asset.AssetId;
                Type = asset.Type;
                Timestamp = asset.Timestamp.ToDateTime();
                DeviceUuid = asset.DeviceUuid;
                Removed = asset.Removed;
                Description = asset.Description;

                Size = asset.Size;
                VersionId = asset.VersionId;
                State = asset.State.ToString();
                Signature = asset.Signature;
                PublicKey = asset.PublicKey;
                CreationTime = asset.CreationTime;
                ModificationTime = asset.ModificationTime;
                Name = asset.Name;
                MediaType = asset.MediaType;
                ApplicationCategory = asset.ApplicationCategory.ToString();
                ApplicationType = asset.ApplicationType.ToString();

                if (asset != null) FileLocation = new JsonFileLocation(asset.FileLocation);

                // FileProperties
                if (!asset.FileProperties.IsNullOrEmpty())
                {
                    var fileProperties = new List<JsonFileProperty>();
                    foreach (var fileProperty in asset.FileProperties)
                    {
                        fileProperties.Add(new JsonFileProperty(fileProperty));
                    }
                    FileProperties = fileProperties;
                }

                // FileComments
                if (!asset.FileComments.IsNullOrEmpty())
                {
                    var fileComments = new List<JsonFileComment>();
                    foreach (var fileComment in asset.FileComments)
                    {
                        fileComments.Add(new JsonFileComment(fileComment));
                    }
                    FileComments = fileComments;
                }
            }
        }


        public FileAsset ToFileAsset()
        {
            var asset = new FileAsset();

            asset.AssetId = AssetId;
            asset.Type = Type;
            asset.Timestamp = Timestamp.ToUnixTime();
            asset.DeviceUuid = DeviceUuid;
            asset.Removed = Removed;
            asset.Description = Description;

            asset.Size = Size;
            asset.VersionId = VersionId;
            asset.State = State.ConvertEnum<FileState>();
            asset.Signature = Signature;
            asset.PublicKey = PublicKey;
            asset.CreationTime = CreationTime;
            asset.ModificationTime = ModificationTime;
            asset.Name = Name;
            asset.MediaType = MediaType;
            asset.ApplicationCategory = ApplicationCategory.ConvertEnum<ApplicationCategory>();
            asset.ApplicationType = ApplicationType.ConvertEnum<ApplicationType>();

            if (FileLocation != null) asset.FileLocation = FileLocation.ToFileLocation();

            // FileProperties
            if (!FileProperties.IsNullOrEmpty())
            {
                var fileProperties = new List<FileProperty>();
                foreach (var fileProperty in FileProperties)
                {
                    fileProperties.Add(fileProperty.ToFileProperty());
                }
                asset.FileProperties = fileProperties;
            }

            // FileComments
            if (!FileComments.IsNullOrEmpty())
            {
                var fileComments = new List<FileComment>();
                foreach (var fileComment in FileComments)
                {
                    fileComments.Add(fileComment.ToFileComment());
                }
                asset.FileComments = fileComments;
            }

            return asset;
        }
    }
}
