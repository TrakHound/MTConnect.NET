// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json;
using MTConnect.Devices.Json;
using MTConnect.Headers;
using MTConnect.Tests.JsonCppagent.TestHelpers;
using NUnit.Framework;

namespace MTConnect.Tests.JsonCppagent.Devices
{
    /// <summary>
    /// Pins the JSON-cppagent Devices Header behavior: every emitted
    /// `MTConnectDevices.Header` envelope must include `schemaVersion` mapped
    /// from the source `IMTConnectDevicesHeader.SchemaVersion`, plus the
    /// existing `testIndicator` regression pin.
    ///
    /// Source authority:
    /// - Reference shape: cppagent v2.7.0.7 emits `Header.schemaVersion` and
    ///   `Header.testIndicator` on every Devices envelope.
    /// - Public defect tracker:
    ///   https://github.com/TrakHound/MTConnect.NET/issues/130,
    ///   https://github.com/TrakHound/MTConnect.NET/issues/131.
    /// </summary>
    [TestFixture]
    [Category("CppAgentHeaderFieldsPresent")]
    [Category("ComplianceMatrix")]
    public class JsonDevicesHeaderSchemaVersionTests
    {
        [TestCaseSource(typeof(JsonHeaderWireShapeMatrix), nameof(JsonHeaderWireShapeMatrix.SchemaVersionCases))]
        public void Constructor_with_source_header_copies_schemaVersion(string schemaVersion)
        {
            var source = new MTConnectDevicesHeader
            {
                InstanceId = 1,
                Version = $"{schemaVersion}.0.0",
                SchemaVersion = schemaVersion,
                Sender = "agent",
            };

            var json = new JsonDevicesHeader(source);

            Assert.That(json.SchemaVersion, Is.EqualTo(schemaVersion),
                "JsonDevicesHeader must copy SchemaVersion from the source IMTConnectDevicesHeader.");
        }

        [TestCaseSource(typeof(JsonHeaderWireShapeMatrix), nameof(JsonHeaderWireShapeMatrix.SchemaVersionCases))]
        public void Serialized_devices_header_emits_schemaVersion_property(string schemaVersion)
        {
            var source = new MTConnectDevicesHeader
            {
                SchemaVersion = schemaVersion,
            };

            var jsonHeader = new JsonDevicesHeader(source);
            var serialized = JsonSerializer.Serialize(jsonHeader);
            using var doc = JsonDocument.Parse(serialized);

            Assert.That(doc.RootElement.TryGetProperty("schemaVersion", out var v), Is.True,
                "Serialized JsonDevicesHeader must expose 'schemaVersion' on the wire.");
            Assert.That(v.GetString(), Is.EqualTo(schemaVersion));
        }

        [Test]
        public void Serialized_devices_header_emits_testIndicator_property()
        {
            var source = new MTConnectDevicesHeader
            {
                TestIndicator = false,
            };

            var jsonHeader = new JsonDevicesHeader(source);
            var serialized = JsonSerializer.Serialize(jsonHeader);
            using var doc = JsonDocument.Parse(serialized);

            Assert.That(doc.RootElement.TryGetProperty("testIndicator", out var v), Is.True,
                "Serialized JsonDevicesHeader must expose 'testIndicator' on the wire.");
            Assert.That(v.GetBoolean(), Is.False);
        }

        [Test]
        public void Reverse_mapping_round_trips_schemaVersion()
        {
            var source = new MTConnectDevicesHeader
            {
                SchemaVersion = "2.5",
            };

            var roundTripped = new JsonDevicesHeader(source).ToDevicesHeader();

            Assert.That(roundTripped.SchemaVersion, Is.EqualTo("2.5"),
                "ToDevicesHeader must preserve SchemaVersion through the round trip.");
        }

        [Test]
        public void Constructor_with_null_source_does_not_throw()
        {
            var jsonHeader = new JsonDevicesHeader(null);

            Assert.That(jsonHeader.SchemaVersion, Is.Null);
        }

        [Test]
        public void Default_constructor_leaves_schemaVersion_unset()
        {
            var jsonHeader = new JsonDevicesHeader();

            Assert.That(jsonHeader.SchemaVersion, Is.Null);
        }
    }
}
