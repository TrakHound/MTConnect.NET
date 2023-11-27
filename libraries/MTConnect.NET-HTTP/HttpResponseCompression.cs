// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Http
{
    public enum HttpResponseCompression
    {
        None,
        Gzip,
        Deflate,
#if NET5_0_OR_GREATER
        Br
#endif
    }
}