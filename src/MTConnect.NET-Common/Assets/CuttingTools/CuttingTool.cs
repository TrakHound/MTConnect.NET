// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;

namespace MTConnect.Assets.CuttingTools
{
    public partial class CuttingTool : Asset
    {
        public const string TypeId = "CuttingTool";


        public CuttingTool()
        {
            Type = TypeId;
            CuttingToolLifeCycle = new CuttingToolLifeCycle();
        }


        protected override IAsset OnProcess(Version mtconnectVersion)
        {
            if (mtconnectVersion != null && mtconnectVersion >= MTConnectVersions.Version12)
            {
                var asset = new CuttingTool();
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

        public override AssetValidationResult IsValid(Version mtconnectVersion)
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

            return new AssetValidationResult(result, message);
        }


		public override string GenerateHash()
		{
			return GenerateHash(this);
		}

		public static string GenerateHash(CuttingTool asset)
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