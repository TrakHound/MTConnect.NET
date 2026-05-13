// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json;
using MTConnect.Headers;
using MTConnect.Streams.Json;
using NUnit.Framework;

namespace MTConnect.Tests.JsonCppagent.Streams
{
    /// <summary>
    /// Pins the JSON-cppagent Streams Header behavior: every emitted
    /// `MTConnectStreams.Header` envelope must include `validation`
    /// mapped from the source `IMTConnectStreamsHeader.Validation`,
    /// matching the cppagent v2 wire shape.
    ///
    /// Source authority:
    /// - Reference shape: cppagent v2.7.0.7 emits `Header.validation`
    ///   on every Streams envelope.
    /// - Public defect tracker:
    ///   https://github.com/TrakHound/MTConnect.NET/issues/130,
    ///   https://github.com/TrakHound/MTConnect.NET/issues/131.
    /// </summary>
    [TestFixture]
    [Category("CppAgentHeaderFieldsPresent")]
    public class JsonStreamsHeaderValidationTests
    {
        [Test]
        public void Constructor_with_source_header_copies_validation()
        {
            var source = new MTConnectStreamsHeader
            {
                InstanceId = 1,
                Version = "2.5.0.0",
                SchemaVersion = "2.5",
                Sender = "agent",
                Validation = true,
            };

            var json = new JsonStreamsHeader(source);

            Assert.That(json.Validation, Is.True,
                "JsonStreamsHeader must copy Validation from the source IMTConnectStreamsHeader.");
        }

        [Test]
        public void Serialized_streams_header_emits_validation_property()
        {
            var source = new MTConnectStreamsHeader
            {
                Validation = true,
            };

            var jsonHeader = new JsonStreamsHeader(source);
            var serialized = JsonSerializer.Serialize(jsonHeader);
            using var doc = JsonDocument.Parse(serialized);

            Assert.That(doc.RootElement.TryGetProperty("validation", out var v), Is.True,
                "Serialized JsonStreamsHeader must expose 'validation' on the wire.");
            Assert.That(v.GetBoolean(), Is.True);
        }

        [Test]
        public void Reverse_mapping_round_trips_validation()
        {
            var source = new MTConnectStreamsHeader
            {
                Validation = true,
            };

            var roundTripped = new JsonStreamsHeader(source).ToStreamsHeader();

            Assert.That(roundTripped.Validation, Is.True,
                "ToStreamsHeader must preserve Validation through the round trip.");
        }
    }
}
