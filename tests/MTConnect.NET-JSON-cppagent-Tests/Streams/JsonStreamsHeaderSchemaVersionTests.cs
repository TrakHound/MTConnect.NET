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
    /// `MTConnectStreams.Header` envelope must include `schemaVersion`,
    /// matching the cppagent v2 wire shape, and `testIndicator` must
    /// remain on the wire (regression pin against #131 reverting).
    ///
    /// Source authority:
    /// - Reference shape: cppagent v2.7.0.7 emits `Header.schemaVersion` and
    ///   `Header.testIndicator` on every Streams envelope.
    /// - Public defect tracker:
    ///   https://github.com/TrakHound/MTConnect.NET/issues/130 (schemaVersion),
    ///   https://github.com/TrakHound/MTConnect.NET/issues/131 (testIndicator).
    /// </summary>
    [TestFixture]
    [Category("CppAgentHeaderFieldsPresent")]
    public class JsonStreamsHeaderSchemaVersionTests
    {
        [Test]
        public void Constructor_with_source_header_copies_schemaVersion()
        {
            var source = new MTConnectStreamsHeader
            {
                InstanceId = 1,
                Version = "2.5.0.0",
                SchemaVersion = "2.5",
                Sender = "agent",
            };

            var json = new JsonStreamsHeader(source);

            Assert.That(json.SchemaVersion, Is.EqualTo("2.5"),
                "JsonStreamsHeader must copy SchemaVersion from the source IMTConnectStreamsHeader.");
        }

        [Test]
        public void Serialized_streams_header_emits_schemaVersion_property()
        {
            var source = new MTConnectStreamsHeader
            {
                SchemaVersion = "2.5",
            };

            var jsonHeader = new JsonStreamsHeader(source);
            var serialized = JsonSerializer.Serialize(jsonHeader);
            using var doc = JsonDocument.Parse(serialized);

            Assert.That(doc.RootElement.TryGetProperty("schemaVersion", out var v), Is.True,
                "Serialized JsonStreamsHeader must expose 'schemaVersion' on the wire.");
            Assert.That(v.GetString(), Is.EqualTo("2.5"));
        }

        [Test]
        public void Serialized_streams_header_emits_testIndicator_property()
        {
            // Regression pin against #131: testIndicator must remain on the wire
            // even when the source flag is the default `false`.
            var source = new MTConnectStreamsHeader
            {
                TestIndicator = false,
            };

            var jsonHeader = new JsonStreamsHeader(source);
            var serialized = JsonSerializer.Serialize(jsonHeader);
            using var doc = JsonDocument.Parse(serialized);

            Assert.That(doc.RootElement.TryGetProperty("testIndicator", out var v), Is.True,
                "Serialized JsonStreamsHeader must expose 'testIndicator' on the wire.");
            Assert.That(v.GetBoolean(), Is.False);
        }

        [Test]
        public void Reverse_mapping_round_trips_schemaVersion()
        {
            var source = new MTConnectStreamsHeader
            {
                SchemaVersion = "2.5",
            };

            var roundTripped = new JsonStreamsHeader(source).ToStreamsHeader();

            Assert.That(roundTripped.SchemaVersion, Is.EqualTo("2.5"),
                "ToStreamsHeader must preserve SchemaVersion through the round trip.");
        }

        [Test]
        public void Constructor_with_null_source_does_not_throw()
        {
            // Null-source ctor branch is covered for 100% line coverage.
            var jsonHeader = new JsonStreamsHeader(null);

            Assert.That(jsonHeader.SchemaVersion, Is.Null);
        }

        [Test]
        public void Default_constructor_leaves_schemaVersion_unset()
        {
            var jsonHeader = new JsonStreamsHeader();

            Assert.That(jsonHeader.SchemaVersion, Is.Null);
        }
    }
}
