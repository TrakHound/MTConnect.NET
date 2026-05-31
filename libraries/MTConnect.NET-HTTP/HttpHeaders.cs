// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Http
{
    /// <summary>
    /// Canonical string constants for the HTTP request and response headers that the MTConnect
    /// HTTP transport reads and writes when negotiating document format and payload compression.
    /// Using these constants in place of raw literals keeps spelling and capitalisation consistent
    /// between client (<see cref="Clients.MTConnectHttpClient"/>) and server
    /// (<see cref="Servers.Http.MTConnectHttpServer"/>) code paths.
    /// </summary>
    public static class HttpHeaders
    {
        /// <summary>The <c>Accept</c> request header — used by clients to advertise the MTConnect document format (XML, JSON, etc.) they prefer.</summary>
        public const string Accept = "Accept";

        /// <summary>The <c>Accept-Encoding</c> request header — used by clients to advertise the content codings (gzip, deflate, br) they can decode.</summary>
        public const string AcceptEncoding = "Accept-Encoding";

        /// <summary>The <c>Content-encoding</c> response header — set by the server to identify the content coding actually applied to the payload.</summary>
        public const string ContentEncoding = "Content-encoding";

        /// <summary>The <c>Content-type</c> response header — set by the server to identify the MIME type of the MTConnect document (e.g. <c>application/xml</c>, <c>application/json</c>).</summary>
        public const string ContentType = "Content-type";

        /// <summary>The <c>Content-length</c> response header — set by the server to the size in bytes of the encoded response body.</summary>
        public const string ContentLength = "Content-length";
    }
}
