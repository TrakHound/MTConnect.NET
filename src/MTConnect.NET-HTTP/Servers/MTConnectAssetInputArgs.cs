// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Servers
{
    public struct MTConnectAssetInputArgs
    {
        public string AssetId { get; set; }

        public string AssetType { get; set; }

        public string DeviceKey { get; set; }

        public string DocumentFormat { get; set; }

        public byte[] RequestBody { get; set; }
    }
}
