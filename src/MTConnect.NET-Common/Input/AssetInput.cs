﻿// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;
using System.Text;

namespace MTConnect.Input
{
    public class AssetInput : IAssetInput
    {
        private static readonly Encoding _utf8 = new UTF8Encoding();

        private byte[] _changeId;
        private byte[] _changeIdWithTimestamp;


        /// <summary>
        /// The UUID or Name of the Device that the Asset is associated with
        /// </summary>
        public string DeviceKey { get; set; }

        /// <summary>
        /// The ID of the Asset
        /// </summary>
        public string AssetId { get; set; }

        public IAsset Asset { get; set; }

        /// <summary>
        /// The timestamp (UnixTime in Milliseconds) that the observation was recorded at
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// An MD5 Hash of the Observation that can be used for comparison
        /// </summary>
        public byte[] ChangeId
        {
            get
            {
                if (_changeId == null) _changeId = CreateChangeId(this, false);
                return _changeId;
            }
        }

        /// <summary>
        /// An MD5 Hash of the Observation including the Timestamp that can be used for comparison
        /// </summary>
        public byte[] ChangeIdWithTimestamp
        {
            get
            {
                if (_changeIdWithTimestamp == null) _changeIdWithTimestamp = CreateChangeId(this, true);
                return _changeIdWithTimestamp;
            }
        }


        public AssetInput(IAsset asset)
        {
            if (asset != null)
            {
                AssetId = asset.AssetId;
                Asset = asset;
            }
        }

        public AssetInput(string deviceKey, IAsset asset)
        {
            if (asset != null)
            {
                DeviceKey = deviceKey;
                AssetId = asset.AssetId;
                Asset = asset;
            }
        }

        public AssetInput(IAssetInput asset)
        {
            if (asset != null)
            {
                DeviceKey = asset.DeviceKey;
                AssetId = asset.AssetId;
                Asset = asset.Asset;
                Timestamp = asset.Timestamp;
            }
        }


        private static byte[] CreateChangeId(AssetInput assetInput, bool includeTimestamp)
        {
            if (assetInput != null && assetInput.Asset != null)
            {
                var sb = new StringBuilder();

                // Add DeviceKey (if specified)
                if (!string.IsNullOrEmpty(assetInput.DeviceKey)) sb.Append($"{assetInput.DeviceKey}:::");

                // Add Asset Hash
                sb.Append($"{assetInput.Asset.GenerateHash(includeTimestamp)}::");

                // Add Timestamp
                if (includeTimestamp) sb.Append($"timestamp={assetInput.Timestamp}:");

                // Get Bytes from StringBuilder
                char[] a = new char[sb.Length];
                sb.CopyTo(0, a, 0, sb.Length);

                // Convert StringBuilder result to UTF8 MD5 Bytes
                return _utf8.GetBytes(a).ToMD5HashBytes();
            }

            return null;
        }
    }
}
