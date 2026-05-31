// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;

namespace MTConnect.Assets.Files
{
    public partial class FileAsset
    {
        /// <summary>
        /// The fixed Asset type identifier ("File") written to the Type attribute and used to recognize this asset during deserialization.
        /// </summary>
        public new const string TypeId = "File";


        /// <summary>
        /// Initializes a new FileAsset, stamping the Asset Type with <see cref="TypeId"/>.
        /// </summary>
        public FileAsset()
        {
            Type = TypeId;
        }


        /// <summary>
        /// Excludes the asset from a response document when its required Size or VersionId are missing; otherwise defers to the base version processing.
        /// </summary>
        /// <param name="mtconnectVersion">The MTConnect version the response document targets.</param>
        protected override IAsset OnProcess(Version mtconnectVersion)
        {
            if (Size <= 0) return null;
            if (string.IsNullOrEmpty(VersionId)) return null;

            return base.OnProcess(mtconnectVersion);
        }

        /// <summary>
        /// Validates that the required File properties are present: a positive Size, a VersionId, a CreationTime, and a Location with an Href; reports the first missing field.
        /// </summary>
        /// <param name="mtconnectVersion">The MTConnect version to validate against.</param>
        public override ValidationResult IsValid(Version mtconnectVersion)
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
                else if (Location == null)
                {
                    message = "FileLocation is Required";
                    result = false;
                }
                else
                {
                    if (string.IsNullOrEmpty(Location.Href))
                    {
                        message = "FileLocation Href property is Required";
                        result = false;
                    }
                }
            }

            return new ValidationResult(result, message);
        }

        /// <summary>
        /// Computes the content hash of this file asset; see <see cref="GenerateHash(FileAsset, bool)"/>.
        /// </summary>
        /// <param name="includeTimestamp">When true, the asset timestamp is folded into the hash.</param>
        public override string GenerateHash(bool includeTimestamp = true)
        {
            return GenerateHash(this);
        }

        /// <summary>
        /// Computes a SHA-1 content hash over the asset's scalar properties, optionally excluding the timestamp so equality can be tested independently of when the asset was reported; returns null for a null asset.
        /// </summary>
        /// <param name="asset">The file asset to hash.</param>
        /// <param name="includeTimestamp">When true, the asset timestamp is folded into the hash.</param>
        public static string GenerateHash(FileAsset asset, bool includeTimestamp = true)
        {
            if (asset != null)
            {
                var ids = new List<string>();

                if (includeTimestamp) ids.Add(ObjectExtensions.GetHashPropertyString(asset).ToSHA1Hash());
                else ids.Add(ObjectExtensions.GetHashPropertyString(asset, new string[] { nameof(Timestamp) }).ToSHA1Hash());

                // Need to include CuttingItems

                return StringFunctions.ToSHA1Hash(ids.ToArray());
            }

            return null;
        }
    }
}
