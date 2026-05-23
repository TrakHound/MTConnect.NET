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
            return BuildDocument(devices, new System.Version(2, 7, 0, 0));
        }

        /// <summary>
        /// Builds a Probe-shaped DevicesResponseDocument anchored at a
        /// specific MTConnect Standard release. Both <c>document.Version</c>
        /// and <c>Header.SchemaVersion</c> are derived from the same source
        /// so the parametric envelope-shape tests pin a single coherent
        /// version across the response envelope and its Header sibling.
        /// </summary>
        private static IDevicesResponseDocument BuildDocument(IEnumerable<IDevice> devices, System.Version version)
        {
            var schemaVersion = version.Major + "." + version.Minor;
            return new DevicesResponseDocument
            {
                Header = new MTConnectDevicesHeader
                {
                    InstanceId = 1,
                    Version = version.ToString(),
                    SchemaVersion = schemaVersion,
                    Sender = "test-agent",
                    AssetBufferSize = 1024,
                    AssetCount = 0,
                    BufferSize = 131072,
                    DeviceModelChangeTime = "2026-05-23T00:00:00Z",
                    TestIndicator = false,
                    CreationTime = new System.DateTime(2026, 5, 23, 0, 0, 0, System.DateTimeKind.Utc),
                },
                Devices = devices.ToArray(),
                Version = version,
            };
        }

        /// <summary>
        /// Every MTConnect Standard version this library advertises support
        /// for, sourced from <c>MTConnectVersions</c>
        /// (libraries/MTConnect.NET-Common/MTConnectVersions.cs). Range is
        /// v1.0 through v2.7 inclusive. Adding a new <c>VersionNM</c>
        /// constant there extends this list automatically on the next
        /// reflection scan, so the parametric envelope-shape gate never
        /// silently drops a release.
        /// </summary>
        public static System.Collections.Generic.IEnumerable<System.Version> AllSupportedMTConnectVersions()
        {
            var fields = typeof(MTConnectVersions).GetFields(
                System.Reflection.BindingFlags.Public
                | System.Reflection.BindingFlags.Static);
            foreach (var field in fields)
            {
                if (field.FieldType != typeof(System.Version)) continue;
                if (!field.Name.StartsWith("Version", System.StringComparison.Ordinal)) continue;
                if (field.GetValue(null) is System.Version v)
                {
                    yield return v;
                }
            }
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


        // ---------------------------------------------------------------
        // Read-path symmetry: CreateDevicesResponseDocument must deserialise
        // the same envelope shape that Format emits. Pre-fix the reader
        // routed through JsonDeviceContainer (single-device round-trip),
        // which collapsed every multi-device envelope to a one-element list
        // and erased Agent vs Device typing. These tests pin the symmetric
        // contract: write-then-read survives the device count, identity,
        // and Type-tag intact.
        // ---------------------------------------------------------------

        [Test]
        public void MqttProbeFormatter_round_trip_envelope_preserves_multi_device_shape()
        {
            // Arrange: one Agent + two Devices.
            var sourceDevices = new IDevice[]
            {
                NewAgent("agent-1", "agent", "agent-1"),
                NewDevice("device-1", "VMC-3Axis", "device-1"),
                NewDevice("device-2", "Lathe", "device-2"),
            };
            var document = BuildDocument(sourceDevices);
            var formatter = new JsonMqttResponseDocumentFormatter();

            // Act: Format -> stream -> CreateDevicesResponseDocument.
            var writeResult = formatter.Format(document);
            Assert.That(writeResult.Success, Is.True, "Format must succeed for a non-empty source.");
            writeResult.Content.Position = 0;

            var readResult = formatter.CreateDevicesResponseDocument(writeResult.Content);

            // Assert: round-trip success.
            Assert.That(readResult.Success, Is.True,
                "CreateDevicesResponseDocument must succeed on a payload Format just produced.");
            Assert.That(readResult.Content, Is.Not.Null,
                "Read result content (the parsed document) must not be null on success.");

            var roundTripped = readResult.Content;
            Assert.That(roundTripped.Devices, Is.Not.Null,
                "Round-tripped document must expose a non-null Devices collection.");

            var roundTrippedList = roundTripped.Devices.ToList();
            Assert.That(roundTrippedList.Count, Is.EqualTo(sourceDevices.Length),
                "Round-tripped Devices count must match the source — no envelope entry may be dropped.");

            // Identity preservation: every source UUID is present after round-trip.
            var roundTrippedUuids = roundTrippedList.Select(d => d.Uuid).ToList();
            foreach (var source in sourceDevices)
            {
                Assert.That(roundTrippedUuids, Contains.Item(source.Uuid),
                    $"Source UUID '{source.Uuid}' must survive the envelope round-trip.");
            }
        }

        [Test]
        public void MqttProbeFormatter_round_trip_envelope_preserves_Agent_Device_typing()
        {
            // Arrange: one Agent + one Device. The read path must distinguish
            // them by Type so consumers can re-derive the cppagent v2 keyed
            // shape (Agent[] vs Device[]) without losing identity.
            var document = BuildDocument(new IDevice[]
            {
                NewAgent("agent-1", "agent", "agent-1"),
                NewDevice("device-1", "VMC-3Axis", "device-1"),
            });
            var formatter = new JsonMqttResponseDocumentFormatter();

            // Act
            var writeResult = formatter.Format(document);
            writeResult.Content.Position = 0;
            var readResult = formatter.CreateDevicesResponseDocument(writeResult.Content);

            // Assert: typing is preserved through round-trip.
            Assert.That(readResult.Success, Is.True);
            var roundTripped = readResult.Content.Devices.ToList();

            var agentMeta = roundTripped.SingleOrDefault(d => d.Uuid == "agent-1");
            Assert.That(agentMeta, Is.Not.Null,
                "The Agent meta-device must be recoverable by its source UUID after round-trip.");
            Assert.That(agentMeta!.Type, Is.EqualTo(Agent.TypeId),
                "Round-tripped Agent meta-device must retain Type == Agent.TypeId, "
                + "not collapse to the generic Device.TypeId.");

            var machineDevice = roundTripped.SingleOrDefault(d => d.Uuid == "device-1");
            Assert.That(machineDevice, Is.Not.Null,
                "The machine Device must be recoverable by its source UUID after round-trip.");
            Assert.That(machineDevice!.Type, Is.EqualTo(Device.TypeId),
                "Round-tripped machine Device must retain Type == Device.TypeId.");
        }

        [Test]
        public void MqttProbeFormatter_preserves_Agent_name_case_in_envelope_output()
        {
            // Arrange: an Agent whose source `name` attribute is mixed-case,
            // mirroring cppagent's reference fixture convention (e.g. "LinuxCNC").
            // The cppagent JSON v2 serialiser emits the `name` attribute
            // verbatim — MTConnect.NET's formatter must do the same.
            var document = BuildDocument(new IDevice[]
            {
                NewAgent("agent-1", "LinuxCNC", "agent-1"),
            });
            var formatter = new JsonMqttResponseDocumentFormatter();

            // Act
            var root = GetRoot(formatter.Format(document));
            var agents = root
                .GetProperty("MTConnectDevices")
                .GetProperty("Devices")
                .GetProperty("Agent");
            var agentNode = agents[0];

            // Assert: the `name` JSON field equals the source value byte-for-byte.
            Assert.That(agentNode.TryGetProperty("name", out var nameProp), Is.True,
                "Agent JSON object must expose a `name` field.");
            Assert.That(nameProp.GetString(), Is.EqualTo("LinuxCNC"),
                "Agent `name` must be emitted verbatim — no lowercase / case normalisation.");
        }


        // ---------------------------------------------------------------
        // Parametric multi-version envelope-shape contract.
        //
        // The bug report's central artefact citations require every
        // supported MTConnect release to honour the same envelope shape:
        //  * cppagent JSON v2 probe printer (json_printer_probe_test.cpp)
        //  * MTConnect v2.7 XSD DevicesType (MTConnectDevices_2.7.xsd
        //    lines 5029-5051) — Agent + Device as separate named child
        //    elements of Devices
        //  * SysML v2.7 Agent block (Profile:normative)
        //
        // The Format(IDevicesResponseDocument, options) signature is not
        // version-parameterised at the API surface; the version travels
        // on document.Version + Header.SchemaVersion. These tests pin
        // that the envelope key shape (MTConnectDevices.Devices is a
        // JSON object with Agent[] + Device[] keys) and the Agent name
        // case preservation are invariant across the v1.0 → v2.7 range
        // sourced from MTConnectVersions.cs. If a future release adds a
        // version-discriminating branch to the formatter, these tests
        // become the regression gate that surfaces silent shape drift on
        // older releases.
        // ---------------------------------------------------------------

        /// <remarks>
        /// Pins the contract from bug report lines 13-15 (TL;DR) across
        /// every supported MTConnect release — multi-version variant of
        /// MqttProbeFormatter_emits_full_envelope_with_Agent_and_Device_keys.
        /// </remarks>
        [TestCaseSource(nameof(AllSupportedMTConnectVersions))]
        public void MqttProbeFormatter_envelope_shape_holds_for_version(System.Version version)
        {
            // Arrange: one Agent + two Devices, same shape as the master
            // test, anchored at the supplied MTConnect Standard release.
            var devices = new IDevice[]
            {
                NewAgent("agent-1", "Agent", "agent-1"),
                NewDevice("device-1", "VMC-3Axis", "device-1"),
                NewDevice("device-2", "Lathe", "device-2"),
            };
            var document = BuildDocument(devices, version);
            var formatter = new JsonMqttResponseDocumentFormatter();

            // Act
            var root = GetRoot(formatter.Format(document));

            // Assert: envelope key shape is invariant across all versions.
            var envelope = root.GetProperty("MTConnectDevices");
            var devicesNode = envelope.GetProperty("Devices");
            Assert.That(devicesNode.ValueKind, Is.EqualTo(JsonValueKind.Object),
                $"Devices must be a JSON object for v{version} (XSD DevicesType complex type).");

            Assert.That(devicesNode.TryGetProperty("Agent", out var agents), Is.True,
                $"Devices.Agent key must be emitted for v{version} when the source contains an Agent.");
            Assert.That(agents.GetArrayLength(), Is.EqualTo(1),
                $"Devices.Agent must contain exactly one entry for v{version}.");

            Assert.That(devicesNode.TryGetProperty("Device", out var machineDevices), Is.True,
                $"Devices.Device key must be emitted for v{version} when the source contains Devices.");
            Assert.That(machineDevices.GetArrayLength(), Is.EqualTo(2),
                $"Devices.Device must contain both source entries for v{version}; none may be dropped.");
        }

        /// <remarks>
        /// Pins the contract from bug report lines 13-15 (TL;DR) — Agent
        /// name case preservation across every supported MTConnect release.
        /// </remarks>
        [TestCaseSource(nameof(AllSupportedMTConnectVersions))]
        public void MqttProbeFormatter_Agent_name_case_preserved_for_version(System.Version version)
        {
            // Arrange: a mixed-case Agent name with hyphen and underscore,
            // exercising every code-point class the XSD NameType permits.
            const string mixedCaseName = "LinuxCNC-Mixed_Case";
            var document = BuildDocument(
                new IDevice[] { NewAgent("agent-1", mixedCaseName, "agent-1") },
                version);
            var formatter = new JsonMqttResponseDocumentFormatter();

            // Act
            var root = GetRoot(formatter.Format(document));
            var agents = root
                .GetProperty("MTConnectDevices")
                .GetProperty("Devices")
                .GetProperty("Agent");
            var agentNode = agents[0];

            // Assert: the `name` JSON field is byte-identical to the source
            // for every supported release — no case folding anywhere in
            // the version range. W3C XML 1.0 §2.3 ("Names are case-
            // sensitive") plus the XSD NameType (xs:string, no case facet)
            // make any lowercase normalisation a wire-shape regression.
            Assert.That(agentNode.TryGetProperty("name", out var nameProp), Is.True,
                $"Agent JSON object must expose a `name` field for v{version}.");
            Assert.That(nameProp.GetString(), Is.EqualTo(mixedCaseName),
                $"Agent `name` must be emitted verbatim for v{version} — no case normalisation.");
        }

        /// <remarks>
        /// Pins the contract from bug report lines 13-15 (TL;DR) — read-path
        /// envelope round-trip across every supported MTConnect release.
        /// Multi-version variant of
        /// MqttProbeFormatter_round_trip_envelope_preserves_multi_device_shape +
        /// MqttProbeFormatter_round_trip_envelope_preserves_Agent_Device_typing.
        /// </remarks>
        [TestCaseSource(nameof(AllSupportedMTConnectVersions))]
        public void MqttProbeFormatter_round_trip_envelope_for_version(System.Version version)
        {
            // Arrange: one Agent + two Devices.
            var sourceDevices = new IDevice[]
            {
                NewAgent("agent-1", "Agent", "agent-1"),
                NewDevice("device-1", "VMC-3Axis", "device-1"),
                NewDevice("device-2", "Lathe", "device-2"),
            };
            var document = BuildDocument(sourceDevices, version);
            var formatter = new JsonMqttResponseDocumentFormatter();

            // Act: Format → stream → CreateDevicesResponseDocument.
            var writeResult = formatter.Format(document);
            Assert.That(writeResult.Success, Is.True,
                $"Format must succeed for v{version} on a non-empty source.");
            writeResult.Content.Position = 0;

            var readResult = formatter.CreateDevicesResponseDocument(writeResult.Content);

            // Assert: device count survives the envelope round-trip.
            Assert.That(readResult.Success, Is.True,
                $"CreateDevicesResponseDocument must succeed for v{version} on the freshly written payload.");
            var roundTripped = readResult.Content.Devices.ToList();
            Assert.That(roundTripped.Count, Is.EqualTo(sourceDevices.Length),
                $"Round-tripped Devices count must match the source for v{version} — no envelope entry may be dropped.");

            // Assert: Agent vs Device typing survives — the central read-path
            // regression that 15d70abe addressed.
            var agentMeta = roundTripped.SingleOrDefault(d => d.Uuid == "agent-1");
            Assert.That(agentMeta, Is.Not.Null,
                $"Agent meta-device must be recoverable for v{version} by source UUID.");
            Assert.That(agentMeta!.Type, Is.EqualTo(Agent.TypeId),
                $"Round-tripped Agent meta-device must retain Agent.TypeId for v{version}.");

            var machineDevice = roundTripped.SingleOrDefault(d => d.Uuid == "device-1");
            Assert.That(machineDevice, Is.Not.Null,
                $"Machine Device must be recoverable for v{version} by source UUID.");
            Assert.That(machineDevice!.Type, Is.EqualTo(Device.TypeId),
                $"Round-tripped machine Device must retain Device.TypeId for v{version}.");
        }
    }
}
