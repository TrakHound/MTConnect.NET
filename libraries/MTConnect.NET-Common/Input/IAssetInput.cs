// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;

namespace MTConnect.Input
{
    public interface IAssetInput
    {
        /// <summary>
        /// The UUID or Name of the Device that the Asset is associated with
        /// </summary>
        string DeviceKey { get; }

        /// <summary>
        /// The ID of the Asset
        /// </summary>
        string AssetId { get; }

        IAsset Asset { get; }

        /// <summary>
        /// The timestamp (UnixTime in Milliseconds) that the observation was recorded at
        /// </summary>
        long Timestamp { get; }

        /// <summary>
        /// An MD5 Hash of the Observation that can be used for comparison
        /// </summary>
        byte[] ChangeId { get; }

        /// <summary>
        /// An MD5 Hash of the Observation including the Timestamp that can be used for comparison
        /// </summary>
        byte[] ChangeIdWithTimestamp { get; }
    }
}
