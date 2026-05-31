// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Http
{
    /// <summary>
    /// Enumerates the response-side compression strategies that the MTConnect HTTP server applies
    /// to outgoing MTConnect documents. Unlike <see cref="HttpContentEncoding"/> which only lists
    /// codings that imply compression, this enum also includes <see cref="None"/> so that a server
    /// can be configured to send payloads uncompressed regardless of what the client advertises.
    /// </summary>
    public enum HttpResponseCompression
    {
        /// <summary>Responses are written uncompressed; no <c>Content-Encoding</c> header is added.</summary>
        None,

        /// <summary>Responses are compressed with <c>gzip</c> (RFC 1952) when the client's <c>Accept-Encoding</c> permits it.</summary>
        Gzip,

        /// <summary>Responses are compressed with the HTTP <c>deflate</c> coding when the client's <c>Accept-Encoding</c> permits it.</summary>
        Deflate,
#if NET5_0_OR_GREATER
        /// <summary>Responses are compressed with Brotli (<c>br</c>, RFC 7932) when the client's <c>Accept-Encoding</c> permits it. Only available on .NET 5.0 or newer.</summary>
        Br
#endif
    }
}
