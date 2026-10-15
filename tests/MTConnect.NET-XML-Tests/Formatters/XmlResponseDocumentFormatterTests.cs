// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using MTConnect.Agents;
using MTConnect.Errors;
using MTConnect.Formatters;
using MTConnect.Formatters.Xml;
using NUnit.Framework;

namespace MTConnect.Tests.XML.Formatters
{
    /// <summary>
    /// Pins <see cref="XmlResponseDocumentFormatter"/>: reading still parses when XSD validation runs,
    /// non-seekable input, read failures reported as errors, stream position handling, validation-level
    /// parsing (numeric or name) and the Strict behaviour for each document type.
    /// </summary>
    [TestFixture]
    [Category("XmlResponseDocumentFormatter")]
    public class XmlResponseDocumentFormatterTests
    {
        // A schema for an unrelated namespace: validation runs (and passes) without constraining the document
        private const string UnrelatedSchema = @"<xs:schema xmlns:xs=""http://www.w3.org/2001/XMLSchema"" targetNamespace=""urn:example:unrelated"" elementFormDefault=""qualified""><xs:element name=""Other"" type=""xs:string"" /></xs:schema>";

        private readonly XmlResponseDocumentFormatter _formatter = new XmlResponseDocumentFormatter();


        private static List<KeyValuePair<string, string>> Options(string validationLevel, params string[] schemas)
        {
            var options = new List<KeyValuePair<string, string>>();
            if (validationLevel != null) options.Add(new KeyValuePair<string, string>("validationLevel", validationLevel));
            foreach (var schema in schemas) options.Add(new KeyValuePair<string, string>("schema", schema));
            return options;
        }

        // A schema for the document's own namespace that does not declare its root element, so validation fails
        private static string RejectingSchemaFor(Stream xml)
        {
            xml.Seek(0, SeekOrigin.Begin);
            var ns = XDocument.Load(xml).Root!.Name.NamespaceName;
            xml.Seek(0, SeekOrigin.Begin);

            return $@"<xs:schema xmlns:xs=""http://www.w3.org/2001/XMLSchema"" targetNamespace=""{ns}"" elementFormDefault=""qualified""><xs:element name=""Other"" type=""xs:string"" /></xs:schema>";
        }

        private static MemoryStream Copy(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            var copy = new MemoryStream();
            stream.CopyTo(copy);
            copy.Seek(0, SeekOrigin.Begin);
            return copy;
        }

        private Stream DevicesXml()
        {
            var broker = new MTConnectAgentBroker();
            var result = _formatter.Format(broker.GetDevicesResponseDocument());
            Assert.That(result.Success, Is.True);
            return result.Content;
        }

        private Stream ErrorXml()
        {
            var broker = new MTConnectAgentBroker();
            var result = _formatter.Format(broker.GetErrorResponseDocument(ErrorCode.INVALID_REQUEST, "bad request"));
            Assert.That(result.Success, Is.True);
            return result.Content;
        }


        [Test]
        public void Format_ReturnsContentPositionedAtStart()
        {
            var xml = DevicesXml();
            var schema = RejectingSchemaFor(xml);

            var broker = new MTConnectAgentBroker();
            var result = _formatter.Format(broker.GetDevicesResponseDocument(), Options("1", schema));

            Assert.That(result.Content, Is.Not.Null);
            Assert.That(result.Content.Position, Is.EqualTo(0));
        }

        [Test]
        public void Read_WithValidation_StillParsesEveryDocumentType()
        {
            var broker = new MTConnectAgentBroker();
            var options = Options("Warning", UnrelatedSchema);

            var devices = _formatter.CreateDevicesResponseDocument(Copy(DevicesXml()), options);
            Assert.That(devices.Success, Is.True, string.Join("; ", devices.Errors ?? Enumerable.Empty<string>()));
            Assert.That(devices.Messages, Does.Contain("XML Validation Successful"));

            var streamsDocument = broker.GetDeviceStreamsResponseDocument();
            var streamsXml = _formatter.Format(ref streamsDocument).Content;
            var streams = _formatter.CreateStreamsResponseDocument(Copy(streamsXml), options);
            Assert.That(streams.Success, Is.True, string.Join("; ", streams.Errors ?? Enumerable.Empty<string>()));

            var assetsXml = _formatter.Format(broker.GetAssetsResponseDocument()).Content;
            var assets = _formatter.CreateAssetsResponseDocument(Copy(assetsXml), options);
            Assert.That(assets.Success, Is.True, string.Join("; ", assets.Errors ?? Enumerable.Empty<string>()));

            var error = _formatter.CreateErrorResponseDocument(Copy(ErrorXml()), options);
            Assert.That(error.Success, Is.True, string.Join("; ", error.Errors ?? Enumerable.Empty<string>()));
        }

        [Test]
        public void Read_NonSeekableStream_Parses()
        {
            using var nonSeekable = new NonSeekableStream(Copy(DevicesXml()));

            var result = _formatter.CreateDevicesResponseDocument(nonSeekable);

            Assert.That(result.Success, Is.True, string.Join("; ", result.Errors ?? Enumerable.Empty<string>()));
            Assert.That(result.Content, Is.Not.Null);
        }

        [Test]
        public void Read_RestoresSeekableStreamPosition_SoItCanBeReadAgain()
        {
            var content = Copy(ErrorXml());

            // Reading the wrong document type fails, then the same stream reads as an Error document
            var asStreams = _formatter.CreateStreamsResponseDocument(content);
            Assert.That(content.Position, Is.EqualTo(0));

            var asError = _formatter.CreateErrorResponseDocument(content);
            Assert.That(asError.Success, Is.True);
            Assert.That(asError.Content.Errors.Single().ErrorCode, Is.EqualTo(ErrorCode.INVALID_REQUEST));
        }

        [Test]
        public void Read_InvalidXml_ReportsErrors()
        {
            var content = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("<MTConnectDevices><Unclosed></MTConnectDevices>"));

            var result = _formatter.CreateDevicesResponseDocument(content);

            Assert.That(result.Success, Is.False);
            Assert.That(result.Errors, Is.Not.Empty);
        }

        [Test]
        public void Read_EmptyOrNullContent_ReportsError()
        {
            Assert.That(_formatter.CreateDevicesResponseDocument(null).Errors, Is.Not.Empty);
            Assert.That(_formatter.CreateDevicesResponseDocument(new MemoryStream()).Errors, Is.Not.Empty);
        }

        [TestCase("1", false)]
        [TestCase("Warning", false)]
        [TestCase("2", true)]
        [TestCase("strict", true)]
        public void Read_ValidationFailure_IsWarningOrErrorByLevel(string level, bool expectError)
        {
            var xml = DevicesXml();
            var schema = RejectingSchemaFor(xml);

            var result = _formatter.CreateDevicesResponseDocument(Copy(xml), Options(level, schema));

            Assert.That(result.Content, Is.Not.Null, "the document is still parsed");
            Assert.That((expectError ? result.Errors : result.Warnings), Is.Not.Empty);
            Assert.That((expectError ? result.Warnings : result.Errors) ?? Enumerable.Empty<string>(), Is.Empty);
        }

        [TestCase("1", true)]
        [TestCase("Warning", true)]
        [TestCase("2", false)]
        [TestCase("Strict", false)]
        public void Format_Devices_ValidationFailure_StrictRejects(string level, bool expectContent)
        {
            var xml = DevicesXml();
            var schema = RejectingSchemaFor(xml);

            var broker = new MTConnectAgentBroker();
            var result = _formatter.Format(broker.GetDevicesResponseDocument(), Options(level, schema));

            Assert.That(result.Success, Is.EqualTo(expectContent));
            Assert.That(result.Content != null, Is.EqualTo(expectContent));
        }

        [Test]
        public void Format_Error_StrictValidationFailure_IsStillReturnedAsWarning()
        {
            var schema = RejectingSchemaFor(ErrorXml());

            var broker = new MTConnectAgentBroker();
            var result = _formatter.Format(broker.GetErrorResponseDocument(ErrorCode.INVALID_REQUEST, "bad request"), Options("Strict", schema));

            Assert.That(result.Content, Is.Not.Null);
            Assert.That(result.Warnings, Is.Not.Empty);
        }

        [Test]
        public void Format_ValidationWithoutSchema_DoesNotClaimValidation()
        {
            var broker = new MTConnectAgentBroker();
            var result = _formatter.Format(broker.GetDevicesResponseDocument(), Options("Strict"));

            Assert.That(result.Success, Is.True);
            Assert.That(result.Messages ?? Enumerable.Empty<string>(), Does.Not.Contain("XML Validation Successful"));
        }

        [Test]
        public void Format_ConcurrentValidation_WithCachedSchemas_IsConsistent()
        {
            var xml = DevicesXml();
            var rejecting = RejectingSchemaFor(xml);
            var broker = new MTConnectAgentBroker();

            var outcomes = new bool[64];
            Parallel.For(0, outcomes.Length, i =>
            {
                // Alternate between a passing and a failing schema configuration
                var schema = i % 2 == 0 ? UnrelatedSchema : rejecting;
                var result = _formatter.Format(broker.GetDevicesResponseDocument(), Options("Strict", schema));
                outcomes[i] = result.Success;
            });

            for (var i = 0; i < outcomes.Length; i++)
            {
                Assert.That(outcomes[i], Is.EqualTo(i % 2 == 0), "iteration " + i);
            }
        }


        private sealed class NonSeekableStream : Stream
        {
            private readonly Stream _inner;

            public NonSeekableStream(Stream inner) { _inner = inner; }

            public override bool CanRead => true;
            public override bool CanSeek => false;
            public override bool CanWrite => false;
            public override long Length => throw new NotSupportedException();
            public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
            public override void Flush() { }
            public override int Read(byte[] buffer, int offset, int count) => _inner.Read(buffer, offset, count);
            public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
            public override void SetLength(long value) => throw new NotSupportedException();
            public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

            protected override void Dispose(bool disposing)
            {
                if (disposing) _inner.Dispose();
                base.Dispose(disposing);
            }
        }
    }
}
