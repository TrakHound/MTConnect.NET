// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.Files;
using MTConnect.Devices.Json;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.Files
{
    /// <summary>
    /// JSON serialization surrogate for an MTConnect <c>File</c> asset in the
    /// cppagent-compatible JSON shape. Converts to and from the strongly-typed
    /// <see cref="FileAsset"/> model.
    /// </summary>
    public class JsonFileAsset
    {
        /// <summary>
        /// The unique identifier of the asset.
        /// </summary>
        [JsonPropertyName("assetId")]
        public string AssetId { get; set; }

        /// <summary>
        /// The asset type identifier, <c>File</c>.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// The timestamp at which the asset was last reported.
        /// </summary>
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The UUID of the device the asset is associated with.
        /// </summary>
        [JsonPropertyName("deviceUuid")]
        public string DeviceUuid { get; set; }

        /// <summary>
        /// Whether the asset has been removed from the agent.
        /// </summary>
        [JsonPropertyName("removed")]
        public bool Removed { get; set; }

        //[JsonPropertyName("description")]
        //public JsonDescription Description { get; set; }
        /// <summary>
        /// The free-form description of the asset.
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }


        /// <summary>
        /// The name of the file.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The MIME media type of the file's contents.
        /// </summary>
        [JsonPropertyName("mediaType")]
        public string MediaType { get; set; }

        /// <summary>
        /// The category of application the file is associated with, serialized
        /// as the enumeration name.
        /// </summary>
        [JsonPropertyName("applicationCategory")]
        public string ApplicationCategory { get; set; }

        /// <summary>
        /// The type of application the file is associated with, serialized as
        /// the enumeration name.
        /// </summary>
        [JsonPropertyName("applicationType")]
        public string ApplicationType { get; set; }

        /// <summary>
        /// The key/value properties describing the file.
        /// </summary>
        [JsonPropertyName("FileProperties")]
        public IEnumerable<JsonFileProperty> FileProperties { get; set; }

        /// <summary>
        /// The comments associated with the file.
        /// </summary>
        [JsonPropertyName("FileComments")]
        public IEnumerable<JsonFileComment> FileComments { get; set; }


        /// <summary>
        /// The size of the file in bytes.
        /// </summary>
        [JsonPropertyName("size")]
        public int Size { get; set; }

        /// <summary>
        /// The version identifier of the file.
        /// </summary>
        [JsonPropertyName("versionId")]
        public string VersionId { get; set; }

        /// <summary>
        /// The state of the file, serialized as the enumeration name.
        /// </summary>
        [JsonPropertyName("state")]
        public string State { get; set; }

        /// <summary>
        /// The location the file can be retrieved from.
        /// </summary>
        [JsonPropertyName("FileLocation")]
        public JsonFileLocation FileLocation { get; set; }

        /// <summary>
        /// The cryptographic signature used to verify the file.
        /// </summary>
        [JsonPropertyName("signature")]
        public string Signature { get; set; }

        /// <summary>
        /// The public key used to verify the file's <see cref="Signature"/>.
        /// </summary>
        [JsonPropertyName("publicKey")]
        public string PublicKey { get; set; }

        /// <summary>
        /// The destinations the file is intended to be transferred to, wrapped
        /// in a counted container.
        /// </summary>
        [JsonPropertyName("Destinations")]
        public JsonDestinationCollection Destinations { get; set; }

        /// <summary>
        /// The time the file was created.
        /// </summary>
        [JsonPropertyName("CreationTime")]
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// The time the file was last modified, when known.
        /// </summary>
        [JsonPropertyName("ModificationTime")]
        public DateTime? ModificationTime { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonFileAsset() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IFileAsset"/>, converting enumerations to strings and
        /// each file property, comment, and destination.
        /// </summary>
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


        /// <summary>
        /// Converts this surrogate to a strongly-typed <see cref="IFileAsset"/>,
        /// parsing enumerations and converting each file property, comment, and
        /// destination.
        /// </summary>
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