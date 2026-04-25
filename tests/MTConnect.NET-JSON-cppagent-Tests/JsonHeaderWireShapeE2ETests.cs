// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json;
using MTConnect.Assets.Json;
using MTConnect.Devices.Json;
using MTConnect.Headers;
using MTConnect.Streams.Json;
using NUnit.Framework;

namespace MTConnect.Tests.JsonCppagent
{
    /// <summary>
    /// End-to-end wire-shape pin: every JSON-cppagent envelope's `Header` element
    /// must carry both `schemaVersion` and `testIndicator` after serialization with
    /// the default `JsonSerializer` settings, regardless of which field on the
    /// source DTO carries a non-default value. Spot-checks the v2.0 / v2.3 / v2.5
    /// schema-version cases the plan's MQTT E2E scenarios cover.
    ///
    /// Source authority:
    /// - Reference shape: cppagent v2.7.0.7 emits `Header.schemaVersion` and
    ///   `Header.testIndicator` on every Streams / Devices / Assets envelope.
    /// - Public defect tracker:
    ///   https://github.com/TrakHound/MTConnect.NET/issues/130 (schemaVersion),
    ///   https://github.com/TrakHound/MTConnect.NET/issues/131 (testIndicator).
    /// </summary>
    [TestFixture]
    public class JsonHeaderWireShapeE2ETests
    {
        [TestCase("2.0")]
        [TestCase("2.3")]
        [TestCase("2.5")]
        public void Streams_envelope_carries_schemaVersion_for_each_supported_schema(string schemaVersion)
        {
            var source = new MTConnectStreamsHeader
            {
                InstanceId = 42,
                Version = $"{schemaVersion}.0.0",
                SchemaVersion = schemaVersion,
                Sender = "agent",
                TestIndicator = false,
            };

            var envelope = new JsonStreamsHeader(source);
            using var doc = JsonDocument.Parse(JsonSerializer.Serialize(envelope));

            Assert.That(doc.RootElement.GetProperty("schemaVersion").GetString(),
                Is.EqualTo(schemaVersion));
            Assert.That(doc.RootElement.GetProperty("testIndicator").GetBoolean(),
                Is.False);
        }

        [TestCase("2.0")]
        [TestCase("2.3")]
        [TestCase("2.5")]
        public void Devices_envelope_carries_schemaVersion_for_each_supported_schema(string schemaVersion)
        {
            var source = new MTConnectDevicesHeader
            {
                InstanceId = 42,
                Version = $"{schemaVersion}.0.0",
                SchemaVersion = schemaVersion,
                Sender = "agent",
                TestIndicator = false,
            };

            var envelope = new JsonDevicesHeader(source);
            using var doc = JsonDocument.Parse(JsonSerializer.Serialize(envelope));

            Assert.That(doc.RootElement.GetProperty("schemaVersion").GetString(),
                Is.EqualTo(schemaVersion));
            Assert.That(doc.RootElement.GetProperty("testIndicator").GetBoolean(),
                Is.False);
        }

        [TestCase("2.0")]
        [TestCase("2.3")]
        [TestCase("2.5")]
        public void Assets_envelope_carries_schemaVersion_for_each_supported_schema(string schemaVersion)
        {
            var source = new MTConnectAssetsHeader
            {
                InstanceId = 42,
                Version = $"{schemaVersion}.0.0",
                SchemaVersion = schemaVersion,
                Sender = "agent",
                TestIndicator = false,
            };

            var envelope = new JsonAssetsHeader(source);
            using var doc = JsonDocument.Parse(JsonSerializer.Serialize(envelope));

            Assert.That(doc.RootElement.GetProperty("schemaVersion").GetString(),
                Is.EqualTo(schemaVersion));
            Assert.That(doc.RootElement.GetProperty("testIndicator").GetBoolean(),
                Is.False);
        }

        [Test]
        public void Streams_envelope_round_trip_preserves_both_fields()
        {
            var source = new MTConnectStreamsHeader
            {
                SchemaVersion = "2.5",
                TestIndicator = true,
            };

            var roundTripped = new JsonStreamsHeader(source).ToStreamsHeader();

            Assert.That(roundTripped.SchemaVersion, Is.EqualTo("2.5"));
            Assert.That(roundTripped.TestIndicator, Is.True);
        }

        [Test]
        public void Devices_envelope_round_trip_preserves_both_fields()
        {
            var source = new MTConnectDevicesHeader
            {
                SchemaVersion = "2.5",
                TestIndicator = true,
            };

            var roundTripped = new JsonDevicesHeader(source).ToDevicesHeader();

            Assert.That(roundTripped.SchemaVersion, Is.EqualTo("2.5"));
            Assert.That(roundTripped.TestIndicator, Is.True);
        }

        [Test]
        public void Assets_envelope_round_trip_preserves_both_fields()
        {
            var source = new MTConnectAssetsHeader
            {
                SchemaVersion = "2.5",
                TestIndicator = true,
            };

            var roundTripped = new JsonAssetsHeader(source).ToAssetsHeader();

            Assert.That(roundTripped.SchemaVersion, Is.EqualTo("2.5"));
            Assert.That(roundTripped.TestIndicator, Is.True);
        }
    }
}
