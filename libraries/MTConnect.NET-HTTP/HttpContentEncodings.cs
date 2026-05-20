// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace MTConnect.Http
{
    /// <summary>
    /// Constants and presets that describe the HTTP content codings recognised by the MTConnect
    /// HTTP transport. The string constants are the canonical lower-case tokens used in the
    /// <c>Accept-Encoding</c> and <c>Content-Encoding</c> headers, and the default-accept list is
    /// what clients advertise when they have no explicit preference.
    /// </summary>
    public static class HttpContentEncodings
    {
        /// <summary>
        /// The default <c>Accept-Encoding</c> set that an MTConnect HTTP client sends when no
        /// override is supplied. The list is ordered by preference: <c>gzip</c>, <c>deflate</c>,
        /// and on .NET 5.0+ <c>br</c>. Servers should pick the first member they can produce.
        /// </summary>
        public static readonly IEnumerable<HttpContentEncoding> DefaultAccept = new List<HttpContentEncoding>
        {
            HttpContentEncoding.Gzip,
            HttpContentEncoding.Deflate,
#if NET5_0_OR_GREATER
            HttpContentEncoding.Br
#endif
        };

        /// <summary>The literal <c>gzip</c> token used in HTTP <c>Content-Encoding</c> / <c>Accept-Encoding</c> headers.</summary>
        public const string Gzip = "gzip";

        /// <summary>The literal <c>deflate</c> token used in HTTP <c>Content-Encoding</c> / <c>Accept-Encoding</c> headers.</summary>
        public const string Deflate = "deflate";

        /// <summary>The literal <c>br</c> token (Brotli, RFC 7932) used in HTTP <c>Content-Encoding</c> / <c>Accept-Encoding</c> headers.</summary>
        public const string Brotli = "br";
    }
}
