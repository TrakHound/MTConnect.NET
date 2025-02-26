// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.Files;
using MTConnect.Devices.Json;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.Files
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

        //[JsonPropertyName("description")]
        //public JsonDescription Description { get; set; }
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

        [JsonPropertyName("FileProperties")]
        public IEnumerable<JsonFileProperty> FileProperties { get; set; }

        [JsonPropertyName("FileComments")]
        public IEnumerable<JsonFileComment> FileComments { get; set; }


        [JsonPropertyName("size")]
        public int Size { get; set; }

        [JsonPropertyName("versionId")]
        public string VersionId { get; set; }

        [JsonPropertyName("state")]
        public string State { get; set; }

        [JsonPropertyName("FileLocation")]
        public JsonFileLocation FileLocation { get; set; }

        [JsonPropertyName("signature")]
        public string Signature { get; set; }

        [JsonPropertyName("publicKey")]
        public string PublicKey { get; set; }

        [JsonPropertyName("Destinations")]
        public JsonDestinationCollection Destinations { get; set; }

        [JsonPropertyName("CreationTime")]
        public DateTime CreationTime { get; set; }

        [JsonPropertyName("ModificationTime")]
        public DateTime? ModificationTime { get; set; }


        public JsonFileAsset() { }

        public JsonFileAsset(IFileAsset asset)
        {
            if (asset != null)
            {
                AssetId = asset.AssetId;
                Type = asset.Type;
                Timestamp = asset.Timestamp;
                DeviceUuid = asset.DeviceUuid;
                Removed = asset.Removed;

                if (asset.Description != null) Description = asset.Description;
                //if (asset.Description != null) Description = new JsonDescription(asset.Description);

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

                if (asset.Location != null) FileLocation = new JsonFileLocation(asset.Location);

                if (!asset.Destinations.IsNullOrEmpty()) Destinations = new JsonDestinationCollection(asset.Destinations);

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


        public IFileAsset ToFileAsset()
        {
            var asset = new FileAsset();

            asset.AssetId = AssetId;
            asset.Timestamp = Timestamp;
            asset.DeviceUuid = DeviceUuid;
            asset.Removed = Removed;

            if (Description != null) asset.Description = Description;
            //if (Description != null) asset.Description = Description.ToDescription();

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

            if (FileLocation != null) asset.Location = FileLocation.ToFileLocation();

            if (Destinations != null) asset.Destinations = Destinations.ToDestinations();

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

            return asset;
        }
    }
}