// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;

namespace MTConnect.Clients
{
    internal class MTConnectHttpResponse
    {
        public static string GetContentHeaderValue(HttpResponseMessage response, string name)
        {
            if (response != null && response.Content != null && response.Content.Headers != null)
            {
                response.Content.Headers.TryGetValues(name, out var headers);
                if (!headers.IsNullOrEmpty())
                {
                    return headers.FirstOrDefault();
                }
            }

            return null;
        }

        public static byte[] HandleContentEncoding(string contentEncoding, byte[] bytes)
        {
            if (!string.IsNullOrEmpty(contentEncoding) && bytes != null && bytes.Length > 0)
            {
                try
                {
                    switch (contentEncoding)
                    {
                        case Http.HttpContentEncodings.Gzip:

                            using (var inputStream = new MemoryStream(bytes))
                            using (var outputStream = new MemoryStream())
                            using (var encodingStream = new GZipStream(inputStream, CompressionMode.Decompress, true))
                            {
                                encodingStream.CopyTo(outputStream);
                                return outputStream.ToArray();
                            }

#if NET5_0_OR_GREATER
                        case Http.HttpContentEncodings.Brotli:

                            using (var inputStream = new MemoryStream(bytes))
                            using (var outputStream = new MemoryStream())
                            using (var encodingStream = new BrotliStream(inputStream, CompressionMode.Decompress, true))
                            {
                                encodingStream.CopyTo(outputStream);
                                return outputStream.ToArray();
                            }
#endif

                        case Http.HttpContentEncodings.Deflate:

                            using (var inputStream = new MemoryStream(bytes))
                            using (var outputStream = new MemoryStream())
                            using (var encodingStream = new DeflateStream(inputStream, CompressionMode.Decompress, true))
                            {
                                encodingStream.CopyTo(outputStream);
                                return outputStream.ToArray();
                            }
                    }
                }
                catch { }
            }

            return bytes;
        }
    }
}