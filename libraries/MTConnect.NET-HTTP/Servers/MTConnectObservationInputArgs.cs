// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Servers
{
    public struct MTConnectObservationInputArgs
    {
        public string DeviceKey { get; set; }

        public string DataItemKey { get; set;  }

        public string Value { get; set; }
    }
}
