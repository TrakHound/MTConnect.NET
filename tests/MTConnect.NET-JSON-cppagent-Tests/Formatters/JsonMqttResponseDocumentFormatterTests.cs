// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using MTConnect.Devices;
using MTConnect.Formatters;
using MTConnect.Headers;
using NUnit.Framework;

namespace MTConnect.Tests.JsonCppagent.Formatters
{
    /// <summary>
    /// Pins the JSON-cppagent MQTT Probe envelope wire shape required by
    /// the MTConnect v2.7 XSD DevicesType (Agent + Device are NAMED child
    /// elements of Devices) and the cppagent JSON v2 reference encoder.
    ///
    /// Reference: cppagent's json_printer_probe_test.cpp asserts
    ///   MTConnectDevices.Devices is a JSON object,
    ///   MTConnectDevices.Devices.Device is an array of devices,
    ///   MTConnectDevices.Devices.Agent is an array of agent meta-devices.
    /// Source URL:
    /// https://raw.githubusercontent.com/mtconnect/cppagent/main/test_package/json_printer_probe_test.cpp
    ///
    /// MTConnect v2.7 XSD DevicesType (MTConnectDevices_2.7.xsd lines
    /// 5029-5051) defines Agent (minOccurs=0, maxOccurs=1) and Device
    /// (minOccurs=1, maxOccurs=unbounded) as separate named child
    /// elements within the DevicesType sequence. The JSON v2 mapping of
    /// a complex type is a JSON object with named child keys, NOT a
    /// JSON array.
    /// </summary>
    [TestFixture]
    [Category("CppAgentMqttProbeEnvelope")]
    public class JsonMqttResponseDocumentFormatterTests
    {
        private static IDevicesResponseDocument BuildDocument(IEnumerable<IDevice> devices)
        {
            return new DevicesResponseDocument
            {
                Header = new MTConnectDevicesHeader
                {
                    InstanceId = 1,
                    Version = "2.7.0.0",
                    SchemaVersion = "2.7",
                    Sender = "test-agent",
                    AssetBufferSize = 1024,
                    AssetCount = 0,
                    BufferSize = 131072,
                    DeviceModelChangeTime = "2026-05-23T00:00:00Z",
                    TestIndicator = false,
                    CreationTime = new System.DateTime(2026, 5, 23, 0, 0, 0, System.DateTimeKind.Utc),
                },
                Devices = devices.ToArray(),
                Version = new System.Version(2, 7, 0, 0),
            };
        }

        private static Device NewAgent(string id, string name, string uuid)
        {
            return new Device
            {
                Id = id,
                Name = name,
                Uuid = uuid,
                Type = Agent.TypeId,
            };
        }

        private static Device NewDevice(string id, string name, string uuid)
        {
            return new Device
            {
                Id = id,
                Name = name,
                Uuid = uuid,
                Type = Device.TypeId,
            };
        }

        private static JsonElement GetRoot(FormatWriteResult result)
        {
            Assert.That(result.Success, Is.True, "Formatter must report success on a non-empty document.");
            Assert.That(result.Content, Is.Not.Null, "Formatter must return a content stream on success.");

            result.Content.Position = 0;
            var doc = JsonDocument.Parse(result.Content);
            return doc.RootElement.Clone();
        }


        [Test]
        public void MqttProbeFormatter_emits_full_envelope_with_Agent_and_Device_keys()
        {
            // Arrange: one Agent + two Devices, mirroring the bug report.
            var devices = new IDevice[]
            {
                NewAgent("agent-1", "agent", "agent-1"),
                NewDevice("device-1", "VMC-3Axis", "device-1"),
                NewDevice("device-2", "Lathe", "device-2"),
            };
            var document = BuildDocument(devices);
            var formatter = new JsonMqttResponseDocumentFormatter();

            // Act
            var result = formatter.Format(document);
            var root = GetRoot(result);

            // Assert: full envelope is present.
            Assert.That(root.TryGetProperty("MTConnectDevices", out var envelope), Is.True,
                "MQTT Probe payload must wrap content in the MTConnectDevices envelope.");

            // Assert: Devices is a JSON object, not an array.
            Assert.That(envelope.TryGetProperty("Devices", out var devicesNode), Is.True,
                "MTConnectDevices envelope must include a Devices child node.");
            Assert.That(devicesNode.ValueKind, Is.EqualTo(JsonValueKind.Object),
                "Devices must be a JSON object (per XSD DevicesType complex type), not an array.");

            // Assert: Agent array with exactly one entry.
            Assert.That(devicesNode.TryGetProperty("Agent", out var agents), Is.True,
                "Devices.Agent key must be emitted when the source contains an Agent meta-device.");
            Assert.That(agents.ValueKind, Is.EqualTo(JsonValueKind.Array),
                "Devices.Agent must be a JSON array (cppagent JSON v2 shape).");
            Assert.That(agents.GetArrayLength(), Is.EqualTo(1),
                "Devices.Agent must contain exactly one entry for the single Agent in the source.");

            // Assert: Device array with exactly two entries.
            Assert.That(devicesNode.TryGetProperty("Device", out var machineDevices), Is.True,
                "Devices.Device key must be emitted when the source contains Device entries.");
            Assert.That(machineDevices.ValueKind, Is.EqualTo(JsonValueKind.Array),
                "Devices.Device must be a JSON array (cppagent JSON v2 shape).");
            Assert.That(machineDevices.GetArrayLength(), Is.EqualTo(2),
                "Devices.Device must contain both source Device entries; none may be dropped.");
        }

        [Test]
        public void MqttProbeFormatter_single_device_only_emits_Device_array_only_no_Agent_key()
        {
            // Arrange: one plain Device, no Agent.
            var document = BuildDocument(new IDevice[]
            {
                NewDevice("device-1", "VMC-3Axis", "device-1"),
            });
            var formatter = new JsonMqttResponseDocumentFormatter();

            // Act
            var root = GetRoot(formatter.Format(document));

            // Assert
            var devicesNode = root.GetProperty("MTConnectDevices").GetProperty("Devices");
            Assert.That(devicesNode.ValueKind, Is.EqualTo(JsonValueKind.Object),
                "Devices must remain a JSON object even with a single Device.");

            Assert.That(devicesNode.TryGetProperty("Device", out var machineDevices), Is.True,
                "Devices.Device must be present.");
            Assert.That(machineDevices.GetArrayLength(), Is.EqualTo(1));

            // Agent key may be absent entirely, or present as a JSON null;
            // neither shape misleads a consumer. Reject only a populated Agent array.
            if (devicesNode.TryGetProperty("Agent", out var agents))
            {
                Assert.That(agents.ValueKind, Is.EqualTo(JsonValueKind.Null),
                    "Devices.Agent must be absent or null when no Agent is in the source document.");
            }
        }

        [Test]
        public void MqttProbeFormatter_Agent_only_emits_Agent_array_only_no_Device_key()
        {
            // Arrange: one Agent, no machine Devices.
            var document = BuildDocument(new IDevice[]
            {
                NewAgent("agent-1", "agent", "agent-1"),
            });
            var formatter = new JsonMqttResponseDocumentFormatter();

            // Act
            var root = GetRoot(formatter.Format(document));

            // Assert
            var devicesNode = root.GetProperty("MTConnectDevices").GetProperty("Devices");
            Assert.That(devicesNode.ValueKind, Is.EqualTo(JsonValueKind.Object));

            Assert.That(devicesNode.TryGetProperty("Agent", out var agents), Is.True,
                "Devices.Agent must be present when the source contains only an Agent.");
            Assert.That(agents.GetArrayLength(), Is.EqualTo(1));

            // Device key may be absent or null.
            if (devicesNode.TryGetProperty("Device", out var machineDevices))
            {
                Assert.That(machineDevices.ValueKind, Is.EqualTo(JsonValueKind.Null),
                    "Devices.Device must be absent or null when no machine Device is in the source document.");
            }
        }

        [Test]
        public void MqttProbeFormatter_preserves_device_order_within_each_array()
        {
            // Arrange: three Devices in a deliberate order.
            var document = BuildDocument(new IDevice[]
            {
                NewDevice("device-c", "Cell-C", "uuid-c"),
                NewDevice("device-a", "Cell-A", "uuid-a"),
                NewDevice("device-b", "Cell-B", "uuid-b"),
            });
            var formatter = new JsonMqttResponseDocumentFormatter();

            // Act
            var root = GetRoot(formatter.Format(document));
            var machineDevices = root
                .GetProperty("MTConnectDevices")
                .GetProperty("Devices")
                .GetProperty("Device");

            // Assert: output array order matches input order verbatim.
            Assert.That(machineDevices.GetArrayLength(), Is.EqualTo(3));
            Assert.That(machineDevices[0].GetProperty("uuid").GetString(), Is.EqualTo("uuid-c"),
                "Devices.Device[0] must be the first source device.");
            Assert.That(machineDevices[1].GetProperty("uuid").GetString(), Is.EqualTo("uuid-a"),
                "Devices.Device[1] must be the second source device.");
            Assert.That(machineDevices[2].GetProperty("uuid").GetString(), Is.EqualTo("uuid-b"),
                "Devices.Device[2] must be the third source device.");
        }

        [Test]
        public void MqttProbeFormatter_emits_Header_and_MTConnectDevices_wrapper()
        {
            // Arrange
            var document = BuildDocument(new IDevice[]
            {
                NewAgent("agent-1", "agent", "agent-1"),
                NewDevice("device-1", "VMC-3Axis", "device-1"),
            });
            var formatter = new JsonMqttResponseDocumentFormatter();

            // Act
            var root = GetRoot(formatter.Format(document));

            // Assert: MTConnectDevices wrapper.
            Assert.That(root.TryGetProperty("MTConnectDevices", out var envelope), Is.True,
                "Top-level MTConnectDevices wrapper must be present.");
            Assert.That(envelope.ValueKind, Is.EqualTo(JsonValueKind.Object));

            // Assert: Header sibling of Devices.
            Assert.That(envelope.TryGetProperty("Header", out var header), Is.True,
                "MTConnectDevices envelope must include a Header sibling of Devices.");
            Assert.That(header.ValueKind, Is.EqualTo(JsonValueKind.Object),
                "Header must serialise as a JSON object.");
            Assert.That(envelope.TryGetProperty("Devices", out var devicesNode), Is.True,
                "MTConnectDevices envelope must include a Devices child.");
            Assert.That(devicesNode.ValueKind, Is.EqualTo(JsonValueKind.Object));
        }

        [Test]
        public void MqttProbeFormatter_empty_devices_returns_error_result()
        {
            // Arrange: a document with no devices.
            var document = BuildDocument(System.Array.Empty<IDevice>());
            var formatter = new JsonMqttResponseDocumentFormatter();

            // Act
            var result = formatter.Format(document);

            // Assert: preserve the pre-fix behaviour of returning an error
            // rather than a malformed-but-empty envelope.
            Assert.That(result.Success, Is.False,
                "Empty Devices must yield FormatWriteResult.Error(), not a successful empty envelope.");
        }

        [Test]
        public void MqttProbeFormatter_indented_output_option_is_respected()
        {
            // Arrange
            var document = BuildDocument(new IDevice[]
            {
                NewDevice("device-1", "VMC-3Axis", "device-1"),
            });
            var formatter = new JsonMqttResponseDocumentFormatter();
            var options = new[] { new KeyValuePair<string, string>("indentOutput", "true") };

            // Act
            var result = formatter.Format(document, options);
            result.Content.Position = 0;
            using var reader = new StreamReader(result.Content);
            var text = reader.ReadToEnd();

            // Assert: indented output contains a newline somewhere in the body.
            Assert.That(text, Does.Contain("\n"),
                "indentOutput=true must produce multi-line JSON (parity with JsonHttpResponseDocumentFormatter).");
            Assert.That(text, Does.Contain("\"MTConnectDevices\""),
                "Indented output must still wrap content in the MTConnectDevices envelope.");
        }
    }
}
