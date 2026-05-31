// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;

namespace MTConnect.Assets.CuttingTools
{
    public partial class CuttingToolAsset
    {
        /// <summary>
        /// The fixed Asset type identifier ("CuttingTool") written to the Type attribute and used to recognize this asset during deserialization.
        /// </summary>
        public const string TypeId = "CuttingTool";


        /// <summary>
        /// Initializes a new CuttingToolAsset, stamping the Asset Type with <see cref="TypeId"/> and allocating an empty life-cycle container.
        /// </summary>
        public CuttingToolAsset()
        {
            Type = TypeId;
            CuttingToolLifeCycle = new CuttingToolLifeCycle();
        }


        /// <summary>
        /// Produces a version-adjusted copy of the asset for inclusion in a response document: excluded entirely before 1.2, the device UUID is dropped at or before 1.3, the serial number defaults to the asset id when absent, and the life cycle is recursively processed. Returns null when the version predates 1.2.
        /// </summary>
        /// <param name="mtconnectVersion">The MTConnect version the response document targets.</param>
        protected override IAsset OnProcess(Version mtconnectVersion)
        {
            if (mtconnectVersion != null && mtconnectVersion >= MTConnectVersions.Version12)
            {
                var asset = new CuttingToolAsset();
                asset.AssetId = AssetId;
                asset.InstanceId = InstanceId;
                asset.Type = Type;
                asset.Timestamp = Timestamp;
                if (mtconnectVersion > MTConnectVersions.Version13) asset.DeviceUuid = DeviceUuid;
                asset.Removed = Removed;
                asset.Description = Description;
                asset.Hash = Hash;

                if (!string.IsNullOrEmpty(SerialNumber)) asset.SerialNumber = SerialNumber;
                else asset.SerialNumber = AssetId;

                asset.ToolId = ToolId;
                asset.Manufacturers = Manufacturers;
                asset.CuttingToolLifeCycle = CuttingToolLifeCycle.Process();
                asset.CuttingToolArchetypeReference = CuttingToolArchetypeReference;

                return asset;
            }

            return null;         
        }

        /// <summary>
        /// Validates that the required SerialNumber and ToolId are present, reporting the first one missing.
        /// </summary>
        /// <param name="mtconnectVersion">The MTConnect version to validate against.</param>
        public override ValidationResult IsValid(Version mtconnectVersion)
        {
            var message = "";
            var result = true;

            if (string.IsNullOrEmpty(SerialNumber))
            {
                message = "SerialNumber property is Required";
                result = false;
            }
            else if (string.IsNullOrEmpty(ToolId))
            {
                message = "ToolId property is Required";
                result = false;
            }

            return new ValidationResult(result, message);
        }


		/// <summary>
		/// Computes the content hash of this cutting tool asset; see <see cref="GenerateHash(CuttingToolAsset, bool)"/>.
		/// </summary>
		/// <param name="includeTimestamp">When true, the asset timestamp is folded into the hash.</param>
		public override string GenerateHash(bool includeTimestamp = true)
		{
			return GenerateHash(this, includeTimestamp);
		}

		/// <summary>
		/// Computes a SHA-1 content hash combining the asset's scalar properties with the hash of its life cycle; when <paramref name="includeTimestamp"/> is false the timestamp and UUID are excluded so equality is independent of when and where the asset was reported. Returns null for a null asset.
		/// </summary>
		/// <param name="asset">The cutting tool asset to hash.</param>
		/// <param name="includeTimestamp">When true, the asset timestamp is folded into the hash.</param>
		public static string GenerateHash(CuttingToolAsset asset, bool includeTimestamp = true)
		{
			if (asset != null)
			{
				var ids = new List<string>();

                if (includeTimestamp) ids.Add(ObjectExtensions.GetHashPropertyString(asset).ToSHA1Hash());
                else ids.Add(ObjectExtensions.GetHashPropertyString(asset, new string[] { nameof(Timestamp), nameof(Uuid) }).ToSHA1Hash());

                ids.Add(CuttingTools.CuttingToolLifeCycle.GenerateHash(asset.CuttingToolLifeCycle));

                return StringFunctions.ToSHA1Hash(ids.ToArray());
			}

			return null;
		}
	}
}