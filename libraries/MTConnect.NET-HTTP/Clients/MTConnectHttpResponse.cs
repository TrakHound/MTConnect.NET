// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
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

        public static Stream HandleContentEncoding(string contentEncoding, Stream inputStream)
        {
            if (inputStream != null && inputStream.Length > 0)
            {
                MemoryStream outputStream;
                if (inputStream.CanSeek && inputStream.Position > 0) inputStream.Seek(0, SeekOrigin.Begin);

                try
                {
                    switch (contentEncoding)
                    {
                        case Http.HttpContentEncodings.Gzip:

                            using (var encodingStream = new GZipStream(inputStream, CompressionMode.Decompress, true))
                            {
                                outputStream = new MemoryStream();
                                encodingStream.CopyTo(outputStream);
                                return outputStream;
                            }

#if NET5_0_OR_GREATER
                        case Http.HttpContentEncodings.Brotli:

                            using (var encodingStream = new BrotliStream(inputStream, CompressionMode.Decompress, true))
                            {
                                outputStream = new MemoryStream();
                                encodingStream.CopyTo(outputStream);
                                return outputStream;
                            }
#endif

                        case Http.HttpContentEncodings.Deflate:

                            using (var encodingStream = new DeflateStream(inputStream, CompressionMode.Decompress, true))
                            {
                                outputStream = new MemoryStream();
                                encodingStream.CopyTo(outputStream);
                                return outputStream;
                            }
                    }
                }
                catch { }
            }

            return inputStream;
        }
    }
}