// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace MTConnect.Servers
{
    public struct MTConnectFormatOptionsArgs
    {
        public string RequestType { get; set; }

        public string DocumentFormat { get; set; }

        public Version MTConnectVersion { get; set; }

        public int ValidationLevel { get; set; }
    }
}
