// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json;
using MTConnect.Assets.Json;
using MTConnect.Headers;
using MTConnect.Tests.JsonCppagent.TestHelpers;
using NUnit.Framework;

namespace MTConnect.Tests.JsonCppagent.Assets
{
    /// <summary>
    /// Pins the JSON-cppagent Assets Header behavior: every emitted
    /// `MTConnectAssets.Header` envelope must include `schemaVersion` mapped
    /// from the source `IMTConnectAssetsHeader.SchemaVersion`, plus the
    /// existing `testIndicator` regression pin.
    ///
    /// Source authority:
    /// - Reference shape: cppagent v2.7.0.7 emits `Header.schemaVersion` and
    ///   `Header.testIndicator` on every Assets envelope.
    /// - Public defect tracker:
    ///   https://github.com/TrakHound/MTConnect.NET/issues/130,
    ///   https://github.com/TrakHound/MTConnect.NET/issues/131.
    /// </summary>
    [TestFixture]
    [Category("CppAgentHeaderFieldsPresent")]
    [Category("ComplianceMatrix")]
    public class JsonAssetsHeaderSchemaVersionTests
    {
        [TestCaseSource(typeof(JsonHeaderWireShapeMatrix), nameof(JsonHeaderWireShapeMatrix.SchemaVersionCases))]
        public void Constructor_with_source_header_copies_schemaVersion(string schemaVersion)
        {
            var source = new MTConnectAssetsHeader
            {
                InstanceId = 1,
                Version = $"{schemaVersion}.0.0",
                SchemaVersion = schemaVersion,
                Sender = "agent",
            };

            var json = new JsonAssetsHeader(source);

            Assert.That(json.SchemaVersion, Is.EqualTo(schemaVersion),
                "JsonAssetsHeader must copy SchemaVersion from the source IMTConnectAssetsHeader.");
        }

        [TestCaseSource(typeof(JsonHeaderWireShapeMatrix), nameof(JsonHeaderWireShapeMatrix.SchemaVersionCases))]
        public void Serialized_assets_header_emits_schemaVersion_property(string schemaVersion)
        {
            var source = new MTConnectAssetsHeader
            {
                SchemaVersion = schemaVersion,
            };

            var jsonHeader = new JsonAssetsHeader(source);
            var serialized = JsonSerializer.Serialize(jsonHeader);
            using var doc = JsonDocument.Parse(serialized);

            Assert.That(doc.RootElement.TryGetProperty("schemaVersion", out var v), Is.True,
                "Serialized JsonAssetsHeader must expose 'schemaVersion' on the wire.");
            Assert.That(v.GetString(), Is.EqualTo(schemaVersion));
        }

        [Test]
        public void Serialized_assets_header_emits_testIndicator_property()
        {
            var source = new MTConnectAssetsHeader
            {
                TestIndicator = false,
            };

            var jsonHeader = new JsonAssetsHeader(source);
            var serialized = JsonSerializer.Serialize(jsonHeader);
            using var doc = JsonDocument.Parse(serialized);

            Assert.That(doc.RootElement.TryGetProperty("testIndicator", out var v), Is.True,
                "Serialized JsonAssetsHeader must expose 'testIndicator' on the wire.");
            Assert.That(v.GetBoolean(), Is.False);
        }

        [Test]
        public void Reverse_mapping_round_trips_schemaVersion()
        {
            var source = new MTConnectAssetsHeader
            {
                SchemaVersion = "2.5",
            };

            var roundTripped = new JsonAssetsHeader(source).ToAssetsHeader();

            Assert.That(roundTripped.SchemaVersion, Is.EqualTo("2.5"),
                "ToAssetsHeader must preserve SchemaVersion through the round trip.");
        }

        [Test]
        public void Constructor_with_null_source_does_not_throw()
        {
            var jsonHeader = new JsonAssetsHeader(null);

            Assert.That(jsonHeader.SchemaVersion, Is.Null);
        }

        [Test]
        public void Default_constructor_leaves_schemaVersion_unset()
        {
            var jsonHeader = new JsonAssetsHeader();

            Assert.That(jsonHeader.SchemaVersion, Is.Null);
        }
    }
}
