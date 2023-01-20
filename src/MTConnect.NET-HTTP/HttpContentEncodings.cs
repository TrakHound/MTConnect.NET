// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace MTConnect.Http
{
    public static class HttpContentEncodings
    {
        public static readonly IEnumerable<HttpContentEncoding> DefaultAccept = new List<HttpContentEncoding> 
        { 
            HttpContentEncoding.Gzip, 
            HttpContentEncoding.Deflate,
#if NET5_0_OR_GREATER
            HttpContentEncoding.Br 
#endif
        };

        public const string Gzip = "gzip";
        public const string Deflate = "deflate";
        public const string Brotli = "br";
    }
}