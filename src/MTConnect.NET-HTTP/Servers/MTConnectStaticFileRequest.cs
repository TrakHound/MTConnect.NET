// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Ceen;
using System;

namespace MTConnect.Servers
{
    public struct MTConnectStaticFileRequest
    {
        public IHttpRequest HttpRequest { get; set; }

        public string FilePath { get; set; }

        public string LocalPath { get; set; }

        public Version Version { get; set; }
    }
}
