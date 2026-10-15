// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Configurations;
using MTConnect.Http;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace MTConnect.Servers.Http
{
    /// <summary>
    /// Response compression shared by whole responses and streaming chunks: picks the content coding
    /// both the server configuration and the client's Accept-Encoding allow, and compresses a body with it.
    /// </summary>
    internal static class HttpContentEncoder
    {
        /// <summary>
        /// Returns the content coding to use (gzip, then br, then deflate), or <c>null</c> to send the body uncompressed.
        /// </summary>
        public static string Select(IHttpServerConfiguration configuration, IEnumerable<string> acceptEncodings)
        {
            if (configuration == null || configuration.ResponseCompression.IsNullOrEmpty() || acceptEncodings.IsNullOrEmpty()) return null;

            if (configuration.ResponseCompression.Contains(HttpResponseCompression.Gzip) && acceptEncodings.Contains(HttpContentEncodings.Gzip))
            {
                return HttpContentEncodings.Gzip;
            }

#if NET5_0_OR_GREATER
            if (configuration.ResponseCompression.Contains(HttpResponseCompression.Br) && acceptEncodings.Contains(HttpContentEncodings.Brotli))
            {
                return HttpContentEncodings.Brotli;
            }
#endif

            if (configuration.ResponseCompression.Contains(HttpResponseCompression.Deflate) && acceptEncodings.Contains(HttpContentEncodings.Deflate))
            {
                return HttpContentEncodings.Deflate;
            }

            return null;
        }

        /// <summary>
        /// Compresses <paramref name="content"/> (from its start) with <paramref name="contentEncoding"/> and returns
        /// a new stream positioned at its start, or <paramref name="content"/> itself when there is no coding.
        /// </summary>
        /// <remarks>
        /// The Content-Encoding header is written from the same value, so a compression failure propagates
        /// rather than falling back to sending the uncompressed body.
        /// </remarks>
        public static Stream Encode(Stream content, string contentEncoding)
        {
            if (content == null || string.IsNullOrEmpty(contentEncoding)) return content;

            // A formatter (e.g. after XML validation) can leave the stream positioned at its end
            if (content.CanSeek && content.Position > 0) content.Seek(0, SeekOrigin.Begin);

            var outputStream = new MemoryStream();

            switch (contentEncoding)
            {
                case HttpContentEncodings.Gzip:
                    using (var zip = new GZipStream(outputStream, CompressionMode.Compress, true))
                    {
                        content.CopyTo(zip);
                    }
                    break;

#if NET5_0_OR_GREATER
                case HttpContentEncodings.Brotli:
                    using (var zip = new BrotliStream(outputStream, CompressionMode.Compress, true))
                    {
                        content.CopyTo(zip);
                    }
                    break;
#endif

                case HttpContentEncodings.Deflate:
                    using (var zip = new DeflateStream(outputStream, CompressionMode.Compress, true))
                    {
                        content.CopyTo(zip);
                    }
                    break;

                default:
                    // Select only returns the codings above
                    outputStream.Dispose();
                    return content;
            }

            outputStream.Seek(0, SeekOrigin.Begin);
            return outputStream;
        }
    }
}
