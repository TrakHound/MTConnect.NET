// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using MTConnect.Http;
using MTConnect.Servers.Http;
using NUnit.Framework;

namespace MTConnect.Tests.Http.Servers
{
    /// <summary>
    /// Pins the multipart chunk encoding used by <see cref="MTConnectHttpServerStream"/> for
    /// sample / current streaming: a chunk advertised with a Content-encoding must actually be
    /// compressed with it (it used to fall back to the uncompressed body), and the formatter's
    /// stream position (left at the end by XML validation) must not truncate the chunk.
    /// The helpers are private, so they are reached by reflection.
    /// </summary>
    [TestFixture]
    [Category("HttpServerStreamChunkEncoding")]
    public class HttpServerStreamChunkEncodingTests
    {
        private const string Body = "<MTConnectStreams><Streams><DeviceStream name=\"d1\" uuid=\"d1\" /></Streams></MTConnectStreams>";

        private static readonly MethodInfo _encodeChunk = typeof(MTConnectHttpServerStream)
            .GetMethod("EncodeChunk", BindingFlags.NonPublic | BindingFlags.Static)!;

        private static readonly MethodInfo _createMultipartChunk = typeof(MTConnectHttpServerStream)
            .GetMethod("CreateMultipartChunk", BindingFlags.NonPublic | BindingFlags.Static)!;

        private static MemoryStream FormattedBody(bool positionedAtEnd)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(Body));
            if (positionedAtEnd) stream.Seek(0, SeekOrigin.End);
            return stream;
        }

        private static Stream Encode(Stream content, string? contentEncoding)
        {
            return (Stream)_encodeChunk.Invoke(null, new object?[] { content, contentEncoding })!;
        }

        private static Stream Decode(string contentEncoding, Stream encoded)
        {
            if (encoded.CanSeek) encoded.Seek(0, SeekOrigin.Begin);

            Stream decoder;
            switch (contentEncoding)
            {
                case HttpContentEncodings.Gzip: decoder = new GZipStream(encoded, CompressionMode.Decompress, true); break;
                case HttpContentEncodings.Deflate: decoder = new DeflateStream(encoded, CompressionMode.Decompress, true); break;
                case HttpContentEncodings.Brotli: decoder = new BrotliStream(encoded, CompressionMode.Decompress, true); break;
                default: throw new ArgumentException(contentEncoding);
            }

            var output = new MemoryStream();
            using (decoder) decoder.CopyTo(output);
            return output;
        }

        private static string ReadAll(Stream stream)
        {
            if (stream.CanSeek) stream.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(stream, Encoding.UTF8, false, 1024, leaveOpen: true);
            return reader.ReadToEnd();
        }

        private static string[] Encodings()
        {
            return new[] { HttpContentEncodings.Gzip, HttpContentEncodings.Deflate, HttpContentEncodings.Brotli };
        }


        [Test]
        public void Helpers_Exist()
        {
            Assert.That(_encodeChunk, Is.Not.Null);
            Assert.That(_createMultipartChunk, Is.Not.Null);
        }

        [TestCaseSource(nameof(Encodings))]
        public void EncodeChunk_RoundTripsThroughClientDecoding(string contentEncoding)
        {
            var encoded = Encode(FormattedBody(positionedAtEnd: true), contentEncoding);

            // Must really be compressed, not the original bytes
            Assert.That(ReadAll(encoded), Is.Not.EqualTo(Body));

            var decoded = Decode(contentEncoding, encoded);
            Assert.That(ReadAll(decoded), Is.EqualTo(Body));
        }

        [TestCaseSource(nameof(Encodings))]
        public void EncodeChunk_ReturnsStreamPositionedAtStart(string contentEncoding)
        {
            var encoded = Encode(FormattedBody(positionedAtEnd: false), contentEncoding);

            Assert.That(encoded.Position, Is.EqualTo(0));
            Assert.That(encoded.Length, Is.GreaterThan(0));
        }

        [Test]
        public void EncodeChunk_NoEncoding_ReturnsContentUnchanged()
        {
            var content = FormattedBody(positionedAtEnd: false);

            Assert.That(Encode(content, null), Is.SameAs(content));
        }

        [Test]
        public void CreateMultipartChunk_GzipPart_AdvertisesEncodingAndCarriesCompressedBody()
        {
            Stream body = Encode(FormattedBody(positionedAtEnd: true), HttpContentEncodings.Gzip);
            var compressedLength = body.Length;

            var args = new object?[] { body, "BOUNDARY", "application/xml", HttpContentEncodings.Gzip };
            var chunk = (MemoryStream)_createMultipartChunk.Invoke(null, args)!;
            var bytes = chunk.ToArray();

            // Split the part headers from the body at the blank line
            var separator = Encoding.ASCII.GetBytes("\r\n\r\n");
            var headerEnd = IndexOf(bytes, separator);
            Assert.That(headerEnd, Is.GreaterThan(0));

            var headers = Encoding.ASCII.GetString(bytes, 0, headerEnd);
            Assert.That(headers, Does.Contain("Content-encoding: gzip"));
            Assert.That(headers, Does.Contain("Content-length: " + (compressedLength + 3)));

            var partBody = new MemoryStream(bytes, headerEnd + separator.Length, (int)compressedLength);
            var decoded = Decode(HttpContentEncodings.Gzip, partBody);
            Assert.That(ReadAll(decoded), Is.EqualTo(Body));
        }

        private static int IndexOf(byte[] haystack, byte[] needle)
        {
            for (var i = 0; i <= haystack.Length - needle.Length; i++)
            {
                var match = true;
                for (var j = 0; j < needle.Length; j++)
                {
                    if (haystack[i + j] != needle[j]) { match = false; break; }
                }
                if (match) return i;
            }

            return -1;
        }
    }
}
