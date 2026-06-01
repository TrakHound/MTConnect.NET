// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using MTConnect;
using MTConnect.Devices;
using MTConnect.Formatters;
using MTConnect.Headers;
using NUnit.Framework;

namespace MTConnect.Compliance.Tests.L2_CrossImpl
{
    // Compliance gate — cppagent JSON v2 Probe envelope shape, the
    // structural contract MTConnect.NET must honour to be wire-compatible
    // with the reference C++ implementation.
    //
    // The shape is fixed by three independent authorities:
    //
    // 1. cppagent JSON v2 probe printer — its
    //    json_printer_probe_test.cpp asserts the wire shape
    //    MTConnectDevices.Devices is a JSON object with Agent[] +
    //    Device[] keys (NOT a single homogeneous array).
    //    Source URL:
    //    https://raw.githubusercontent.com/mtconnect/cppagent/main/test_package/json_printer_probe_test.cpp
    //
    // 2. MTConnect v2.7 XSD DevicesType (MTConnectDevices_2.7.xsd lines
    //    5029-5051): Agent (minOccurs=0, maxOccurs=1) + Device
    //    (minOccurs=1, maxOccurs=unbounded) declared as SEPARATE named
    //    child elements within the DevicesType sequence.
    //
    // 3. W3C XML 1.0 §2.3 — "Names are case-sensitive". Combined with
    //    the v2.7 XSD NameType (xs:string, no case facet), this forbids
    //    silent case folding anywhere on the wire path.
    //
    // This suite does NOT require Docker; it exercises the production
    // formatter against an in-process devices document and asserts the
    // emitted JSON's structural shape matches a frozen reference. The
    // Docker-bound full cross-impl parity workflow lives in
    // CppAgentParityWorkflowTests (Category=RequiresDocker,E2E).
    /// <summary>Pins the behaviour expressed by the test name: cpp agent json v2 probe envelope shape tests.</summary>
    [TestFixture]
    [Category("CppAgentJsonV2Envelope")]
    public class CppAgentJsonV2ProbeEnvelopeShapeTests
    {
        private const string FormatterId = "JSON-cppagent-mqtt";

        private static IDevicesResponseDocument BuildDocument(IEnumerable<IDevice> devices)
        {
            return new DevicesResponseDocument
            {
                Header = new MTConnectDevicesHeader
                {
                    InstanceId = 1,
                    Version = "2.7.0.0",
                    SchemaVersion = "2.7",
                    Sender = "compliance-suite",
                    AssetBufferSize = 1024,
                    AssetCount = 0,
                    BufferSize = 131072,
                    DeviceModelChangeTime = "2026-05-23T00:00:00Z",
                    TestIndicator = false,
                    CreationTime = new DateTime(2026, 5, 23, 0, 0, 0, DateTimeKind.Utc),
                },
                Devices = devices.ToArray(),
                Version = new Version(2, 7, 0, 0),
            };
        }

        private static JsonElement Emit(IDevicesResponseDocument document)
        {
            var formatter = new JsonMqttResponseDocumentFormatter();
            var result = formatter.Format(document);
            Assert.That(result.Success, Is.True,
                "Formatter must report success on a non-empty document.");

            result.Content!.Position = 0;
            using var doc = JsonDocument.Parse(result.Content);
            return doc.RootElement.Clone();
        }


        /// <summary>Pins the behaviour expressed by the test name: probe envelope matches cppagent j s o n v2 keyed object shape.</summary>
        [Test]
        public void Probe_envelope_matches_cppagent_JSON_v2_keyed_object_shape()
        {
            // Source-of-truth cppagent reference assertion (transcribed
            // from test_package/json_printer_probe_test.cpp):
            //   doc["MTConnectDevices"]["Devices"]      is a JSON object
            //   doc["MTConnectDevices"]["Devices"]["Device"]  is an array
            //   doc["MTConnectDevices"]["Devices"]["Agent"]   is an array
            // — Agent and Device are SIBLING keys, not a homogenised array.
            var document = BuildDocument(new IDevice[]
            {
                new Device { Id = "agent-1", Name = "Agent", Uuid = "agent-1", Type = Agent.TypeId },
                new Device { Id = "device-1", Name = "VMC-3Axis", Uuid = "device-1", Type = Device.TypeId },
                new Device { Id = "device-2", Name = "Lathe", Uuid = "device-2", Type = Device.TypeId },
            });

            var root = Emit(document);

            // Envelope wrapper.
            Assert.That(root.TryGetProperty("MTConnectDevices", out var envelope), Is.True,
                "MTConnectDevices wrapper missing — divergence from cppagent JSON v2.");
            Assert.That(envelope.ValueKind, Is.EqualTo(JsonValueKind.Object));

            // Devices as object, not array.
            Assert.That(envelope.TryGetProperty("Devices", out var devicesNode), Is.True,
                "MTConnectDevices.Devices missing.");
            Assert.That(devicesNode.ValueKind, Is.EqualTo(JsonValueKind.Object),
                "MTConnectDevices.Devices must be an object — flat-array shape "
                + "violates cppagent JSON v2 and XSD DevicesType.");

            // Agent[] keyed sibling.
            Assert.That(devicesNode.TryGetProperty("Agent", out var agents), Is.True,
                "Devices.Agent missing.");
            Assert.That(agents.ValueKind, Is.EqualTo(JsonValueKind.Array));
            Assert.That(agents.GetArrayLength(), Is.EqualTo(1));

            // Device[] keyed sibling.
            Assert.That(devicesNode.TryGetProperty("Device", out var machineDevices), Is.True,
                "Devices.Device missing.");
            Assert.That(machineDevices.ValueKind, Is.EqualTo(JsonValueKind.Array));
            Assert.That(machineDevices.GetArrayLength(), Is.EqualTo(2));
        }


        /// <summary>Pins the behaviour expressed by the test name: probe envelope agent element under devices matches x s d v2 7 devices type.</summary>
        [Test]
        public void Probe_envelope_Agent_element_under_Devices_matches_XSD_v2_7_DevicesType()
        {
            // MTConnectDevices_2.7.xsd lines 5029-5051: Agent is a NAMED
            // child element of Devices, not a flat sibling. The JSON v2
            // mapping preserves that nesting: the JSON object reachable
            // by ["MTConnectDevices"]["Devices"]["Agent"] is the JSON
            // representation of the XSD <Agent> element. The Agent key
            // must NEVER appear directly under MTConnectDevices.
            var document = BuildDocument(new IDevice[]
            {
                new Device { Id = "agent-1", Name = "Agent", Uuid = "agent-1", Type = Agent.TypeId },
                new Device { Id = "device-1", Name = "VMC-3Axis", Uuid = "device-1", Type = Device.TypeId },
            });

            var root = Emit(document);
            var envelope = root.GetProperty("MTConnectDevices");

            // The pre-fix bug surfaced as Agent appearing as a flat
            // sibling of Devices, bypassing the XSD's nesting. Pin both
            // shapes:
            Assert.That(envelope.TryGetProperty("Agent", out _), Is.False,
                "Agent must NOT be a direct sibling of Devices — it is a child of Devices per XSD DevicesType.");

            var devicesNode = envelope.GetProperty("Devices");
            Assert.That(devicesNode.TryGetProperty("Agent", out _), Is.True,
                "Agent must be nested under Devices per XSD DevicesType.");
        }


        /// <summary>Pins the behaviour expressed by the test name: probe envelope agent name preserves case per w3 c x m l 1 0 section 2 3.</summary>
        [Test]
        public void Probe_envelope_Agent_name_preserves_case_per_W3C_XML_1_0_section_2_3()
        {
            // W3C XML 1.0 §2.3: "Names are case-sensitive". The MTConnect
            // v2.7 XSD types `name` as xs:string with no case facet, so
            // the agent's source `name` attribute reaches the wire
            // verbatim — no lowercase normalisation may slip in.
            const string mixedCaseAgentName = "LinuxCNC-Mixed_Case";
            var document = BuildDocument(new IDevice[]
            {
                new Device { Id = "agent-1", Name = mixedCaseAgentName, Uuid = "agent-1", Type = Agent.TypeId },
                new Device { Id = "device-1", Name = "VMC-3Axis", Uuid = "device-1", Type = Device.TypeId },
            });

            var root = Emit(document);
            var agentNode = root
                .GetProperty("MTConnectDevices")
                .GetProperty("Devices")
                .GetProperty("Agent")[0];

            Assert.That(agentNode.TryGetProperty("name", out var nameProp), Is.True,
                "Agent JSON object must expose a `name` field.");
            Assert.That(nameProp.GetString(), Is.EqualTo(mixedCaseAgentName),
                "Agent `name` must round-trip with case preserved (W3C XML 1.0 §2.3 / XSD NameType).");
        }


        /// <summary>Pins the behaviour expressed by the test name: probe envelope round trip through named formatter matches emitted shape.</summary>
        [Test]
        public void Probe_envelope_round_trip_through_named_formatter_matches_emitted_shape()
        {
            // The cppagent JSON v2 contract is symmetric: a payload the
            // emitter produced must be re-readable by the same library's
            // reader, and the parsed document must yield every source
            // device with its Type tag intact (Agent.TypeId vs
            // Device.TypeId).
            var sourceDevices = new IDevice[]
            {
                new Device { Id = "agent-1", Name = "Agent", Uuid = "agent-1", Type = Agent.TypeId },
                new Device { Id = "device-1", Name = "VMC-3Axis", Uuid = "device-1", Type = Device.TypeId },
                new Device { Id = "device-2", Name = "Lathe", Uuid = "device-2", Type = Device.TypeId },
            };
            var document = BuildDocument(sourceDevices);
            var formatter = new JsonMqttResponseDocumentFormatter();

            var writeResult = formatter.Format(document);
            Assert.That(writeResult.Success, Is.True);
            writeResult.Content!.Position = 0;

            var readResult = formatter.CreateDevicesResponseDocument(writeResult.Content);
            Assert.That(readResult.Success, Is.True,
                "Symmetric read path must accept the freshly-emitted JSON v2 envelope.");

            var roundTripped = readResult.Content.Devices.ToList();
            Assert.That(roundTripped.Count, Is.EqualTo(sourceDevices.Length),
                "Round-trip must preserve device count — no envelope entry may collapse.");

            var agentMeta = roundTripped.SingleOrDefault(d => d.Uuid == "agent-1");
            Assert.That(agentMeta, Is.Not.Null);
            Assert.That(agentMeta!.Type, Is.EqualTo(Agent.TypeId),
                "Agent vs Device typing must survive the envelope round-trip (post 15d70abe).");
        }
    }
}
