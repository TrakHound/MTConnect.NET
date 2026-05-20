// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Http
{
    /// <summary>
    /// Enumerates the HTTP content encodings that an MTConnect client may request from the agent
    /// through the <c>Accept-Encoding</c> request header. Each value corresponds to one of the
    /// IANA-registered HTTP content-coding tokens that the agent's HTTP transport can apply to
    /// MTConnect response payloads (<c>probe</c>, <c>current</c>, <c>sample</c>, and asset documents).
    /// </summary>
    public enum HttpContentEncoding
    {
        /// <summary>
        /// The <c>gzip</c> content coding (RFC 1952). Universally supported across .NET targets
        /// via <see cref="System.IO.Compression.GZipStream"/>.
        /// </summary>
        Gzip,

        /// <summary>
        /// The <c>deflate</c> content coding (RFC 1951 raw deflate stream as described by RFC 7230
        /// for HTTP). Supported on all .NET targets through <see cref="System.IO.Compression.DeflateStream"/>.
        /// </summary>
        Deflate,
#if NET5_0_OR_GREATER
        /// <summary>
        /// The <c>br</c> Brotli content coding (RFC 7932). Only emitted on .NET 5.0 or newer where
        /// <see cref="System.IO.Compression.BrotliStream"/> is available; older target frameworks
        /// omit this member from the enum.
        /// </summary>
        Br
#endif
    }
}
