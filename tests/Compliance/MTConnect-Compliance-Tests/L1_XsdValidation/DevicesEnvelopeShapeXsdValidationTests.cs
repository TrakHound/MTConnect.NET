// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Text;
using MTConnect;
using MTConnect.Devices;
using MTConnect.Devices.Xml;
using MTConnect.Headers;
using NUnit.Framework;

namespace MTConnect.Compliance.Tests.L1_XsdValidation
{
    // Compliance gate — multi-device Probe envelope MUST place <Agent>
    // and <Device> as named child elements of <Devices> per the v2.7 XSD
    // DevicesType (lines 5029-5051): Agent (minOccurs=0, maxOccurs=1) +
    // Device (minOccurs=1, maxOccurs=unbounded). A flat-array shape
    // under Devices does not schema-validate.
    //
    // The XSD-validating sibling test (which loads the v2.7 XSD through
    // MTConnect.XsdPreprocessor) lives on PR #166 alongside the
    // preprocessor itself. The structural assertions below run under the
    // default sweep — they use XDocument-style string inspection on the
    // emitted XML, so they don't depend on the preprocessor.
    //
    // Source authority:
    //   - MTConnect v2.7 XSD MTConnectDevices_2.7.xsd lines 5029-5051
    //     (DevicesType complex type).
    //   - W3C XML 1.0 §2.3 (Names are case-sensitive).
    /// <summary>Pins the behaviour expressed by the test name: devices envelope shape xsd validation tests.</summary>
    [TestFixture]
    [Category("CppAgentJsonV2Envelope")]
    public class DevicesEnvelopeShapeXsdValidationTests
    {
        private static IDevicesResponseDocument BuildMultiDeviceDocument()
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
                Devices = new IDevice[]
                {
                    new Device { Id = "agent-1", Name = "Agent", Uuid = "agent-1", Type = Agent.TypeId },
                    new Device { Id = "device-1", Name = "VMC-3Axis", Uuid = "device-1", Type = Device.TypeId },
                    new Device { Id = "device-2", Name = "Lathe", Uuid = "device-2", Type = Device.TypeId },
                },
                Version = new Version(2, 7, 0, 0),
            };
        }


        /// <summary>Pins the behaviour expressed by the test name: probe envelope has agent and device under devices per v2 7 devices type.</summary>
        [Test]
        public void Probe_envelope_has_Agent_and_Device_under_Devices_per_v2_7_DevicesType()
        {
            // The XSD-level analogue of the JSON v2 shape test: the XML
            // envelope must place <Agent> and <Device> as named child
            // elements of <Devices>, not flat siblings.
            var document = BuildMultiDeviceDocument();
            using var xmlStream = XmlDevicesResponseDocument.ToXmlStream(
                document,
                extendedSchemas: null,
                styleSheet: null,
                indent: false,
                outputComments: false);
            Assert.That(xmlStream, Is.Not.Null);
            xmlStream!.Position = 0;

            using var reader = new StreamReader(xmlStream, Encoding.UTF8);
            var xml = reader.ReadToEnd();

            // Element ordering follows the XSD sequence: <Agent> then
            // <Device>. We do not care here whether the XML is indented;
            // only the relative element nesting.
            var devicesIdx = xml.IndexOf("<Devices>", StringComparison.Ordinal);
            Assert.That(devicesIdx, Is.GreaterThanOrEqualTo(0), "<Devices> element missing.");

            var devicesClose = xml.IndexOf("</Devices>", devicesIdx, StringComparison.Ordinal);
            Assert.That(devicesClose, Is.GreaterThan(devicesIdx), "<Devices> not closed.");

            var inside = xml.Substring(devicesIdx, devicesClose - devicesIdx);
            Assert.That(inside, Does.Contain("<Agent "),
                "<Agent> must be nested inside <Devices> per XSD DevicesType.");
            Assert.That(inside, Does.Contain("<Device "),
                "<Device> must be nested inside <Devices> per XSD DevicesType.");
        }


        /// <summary>Pins the behaviour expressed by the test name: agent name attribute is case sensitive per w3 c x m l 1 0 section 2 3.</summary>
        [Test]
        public void Agent_name_attribute_is_case_sensitive_per_W3C_XML_1_0_section_2_3()
        {
            // W3C XML 1.0 §2.3: "Names are case-sensitive". The XSD types
            // `name` as xs:string (no case facet), so the source Agent
            // name reaches the wire byte-for-byte. This guards against
            // any silent normalisation between the in-memory device and
            // the emitted XML.
            const string mixedCaseAgentName = "LinuxCNC-Mixed_Case";
            var document = new DevicesResponseDocument
            {
                Header = new MTConnectDevicesHeader
                {
                    InstanceId = 1,
                    Version = "2.7.0.0",
                    SchemaVersion = "2.7",
                    Sender = "compliance-suite",
                    CreationTime = new DateTime(2026, 5, 23, 0, 0, 0, DateTimeKind.Utc),
                },
                Devices = new IDevice[]
                {
                    new Device { Id = "agent-1", Name = mixedCaseAgentName, Uuid = "agent-1", Type = Agent.TypeId },
                    new Device { Id = "device-1", Name = "VMC-3Axis", Uuid = "device-1", Type = Device.TypeId },
                },
                Version = new Version(2, 7, 0, 0),
            };

            using var xmlStream = XmlDevicesResponseDocument.ToXmlStream(
                document,
                extendedSchemas: null,
                styleSheet: null,
                indent: false,
                outputComments: false);
            Assert.That(xmlStream, Is.Not.Null);
            xmlStream!.Position = 0;

            using var reader = new StreamReader(xmlStream, Encoding.UTF8);
            var xml = reader.ReadToEnd();

            Assert.That(xml, Does.Contain($"name=\"{mixedCaseAgentName}\""),
                "Agent name attribute must round-trip with case preserved (W3C XML 1.0 §2.3).");
        }
    }
}
