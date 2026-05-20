// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
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
        ulong InstanceId { get; set; }


		/// <summary>
		/// Prepares the asset for inclusion in a response document for the given MTConnect version, returning the version-adjusted asset or null to exclude it.
		/// </summary>
		/// <param name="mtconnectVersion">The MTConnect version the response document targets.</param>
		IAsset Process(Version mtconnectVersion);

        /// <summary>
        /// Validates the asset against the given MTConnect version, reporting whether it satisfies that version's constraints.
        /// </summary>
        /// <param name="mtconnectVersion">The MTConnect version to validate against.</param>
        ValidationResult IsValid(Version mtconnectVersion);

        /// <summary>
        /// Computes a content hash used to detect changes to the asset.
        /// </summary>
        /// <param name="includeTimestamp">When true, the asset timestamp is folded into the hash.</param>
        string GenerateHash(bool includeTimestamp = true);
	}
}