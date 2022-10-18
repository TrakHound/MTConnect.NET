// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

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
