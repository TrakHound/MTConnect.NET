// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;

namespace MTConnect.Assets.Files
{
    public partial class FileAsset
    {
        public const string TypeId = "File";


        public FileAsset()
        {
            Type = TypeId;
        }


        protected override IAsset OnProcess(Version mtconnectVersion)
        {
            if (Size <= 0) return null;
            if (string.IsNullOrEmpty(VersionId)) return null;

            return base.OnProcess(mtconnectVersion);
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

            return new AssetValidationResult(result, message);
        }

        public override string GenerateHash()
        {
            return GenerateHash(this);
        }

        public static string GenerateHash(FileAsset asset)
        {
            if (asset != null)
            {
                var ids = new List<string>();

                ids.Add(ObjectExtensions.GetHashPropertyString(asset).ToSHA1Hash());

                // Need to include CuttingItems

                return StringFunctions.ToSHA1Hash(ids.ToArray());
            }

            return null;
        }
    }
}
