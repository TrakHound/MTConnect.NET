// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace MTConnect.Assets
{
    public partial interface IAsset : IMTConnectEntity
    {
        /// <summary>
        /// The type for the MTConnect Asset
        /// </summary>
        string Type { get; set; }

        /// <summary>
        /// The Agent InstanceId of the Agent that produced this Asset
        /// </summary>
        long InstanceId { get; set; }


		IAsset Process(Version mtconnectVersion);

        AssetValidationResult IsValid(Version mtconnectVersion);

        string GenerateHash(bool includeTimestamp = true);
	}
}