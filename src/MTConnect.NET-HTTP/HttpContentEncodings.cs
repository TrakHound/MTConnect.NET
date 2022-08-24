// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;

namespace MTConnect.Http
{
    public static class HttpContentEncodings
    {
        public static readonly IEnumerable<HttpContentEncoding> DefaultAccept = new List<HttpContentEncoding> { HttpContentEncoding.Gzip, HttpContentEncoding.Deflate, HttpContentEncoding.Br };

        public const string Gzip = "gzip";
        public const string Deflate = "deflate";
        public const string Brotli = "br";
    }
}
