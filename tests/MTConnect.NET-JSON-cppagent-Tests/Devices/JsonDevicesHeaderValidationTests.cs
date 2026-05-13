// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json;
using MTConnect.Devices.Json;
using MTConnect.Headers;
using NUnit.Framework;

namespace MTConnect.Tests.JsonCppagent.Devices
{
    /// <summary>
    /// Pins the JSON-cppagent Devices Header behavior: every emitted
    /// `MTConnectDevices.Header` envelope must include `validation`
    /// mapped from the source `IMTConnectDevicesHeader.Validation`,
    /// matching the cppagent v2 wire shape.
    ///
    /// Source authority:
    /// - Reference shape: cppagent v2.7.0.7 emits `Header.validation`
    ///   on every Devices envelope.
    /// - Public defect tracker:
    ///   https://github.com/TrakHound/MTConnect.NET/issues/130,
    ///   https://github.com/TrakHound/MTConnect.NET/issues/131.
    /// </summary>
    [TestFixture]
    [Category("CppAgentHeaderFieldsPresent")]
    public class JsonDevicesHeaderValidationTests
    {
        [Test]
        public void Constructor_with_source_header_copies_validation()
        {
            var source = new MTConnectDevicesHeader
            {
                InstanceId = 1,
                Version = "2.5.0.0",
                SchemaVersion = "2.5",
                Sender = "agent",
                Validation = true,
            };

            var json = new JsonDevicesHeader(source);

            Assert.That(json.Validation, Is.True,
                "JsonDevicesHeader must copy Validation from the source IMTConnectDevicesHeader.");
        }

        [Test]
        public void Serialized_devices_header_emits_validation_property()
        {
            var source = new MTConnectDevicesHeader
            {
                Validation = true,
            };

            var jsonHeader = new JsonDevicesHeader(source);
            var serialized = JsonSerializer.Serialize(jsonHeader);
            using var doc = JsonDocument.Parse(serialized);

            Assert.That(doc.RootElement.TryGetProperty("validation", out var v), Is.True,
                "Serialized JsonDevicesHeader must expose 'validation' on the wire.");
            Assert.That(v.GetBoolean(), Is.True);
        }

        [Test]
        public void Reverse_mapping_round_trips_validation()
        {
            var source = new MTConnectDevicesHeader
            {
                Validation = true,
            };

            var roundTripped = new JsonDevicesHeader(source).ToDevicesHeader();

            Assert.That(roundTripped.Validation, Is.True,
                "ToDevicesHeader must preserve Validation through the round trip.");
        }
    }
}
