// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Agents
{
    struct AssetCount
    {
        public string DeviceUuid { get; }

        public string AssetType { get; }

        public int Count { get; set; }

        public bool IsValid => !string.IsNullOrEmpty(DeviceUuid) && !string.IsNullOrEmpty(AssetType) && Count > 0;


        public AssetCount(string deviceUuid, string assetType, int count)
        {
            DeviceUuid = deviceUuid;
            AssetType = assetType;
            Count = count;
        }
    }
}