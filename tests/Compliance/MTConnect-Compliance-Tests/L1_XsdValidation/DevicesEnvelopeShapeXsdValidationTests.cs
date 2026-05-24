// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using MTConnect;
using MTConnect.Devices;
using MTConnect.Devices.Xml;
using MTConnect.Headers;
using NUnit.Framework;

namespace MTConnect.Compliance.Tests.L1_XsdValidation
{
    // Compliance gate — multi-device Probe envelope MUST validate against
    // MTConnectDevices_2.7.xsd. The XSD's DevicesType (lines 5029-5051)
    // declares Agent (minOccurs=0, maxOccurs=1) + Device (minOccurs=1,
    // maxOccurs=unbounded) as separate named child elements within the
    // DevicesType sequence — a flat-array shape under Devices does not
    // schema-validate. This is the load-bearing assertion that grounds
    // the JSON v2 keyed-object envelope: the JSON shape mirrors the
    // XSD's element nesting; the XML shape is the canonical reference.
    //
    // SchemaLoadTests already pins that the v2.7 XSD itself loads cleanly
    // (and pre-seeds the W3C xlink + xml schemas it imports). This suite
    // re-uses the same loader pattern, then validates a sample Probe
    // document built by the production XML formatter.
    //
    // The v2.7 MTConnectDevices XSD carries XSD 1.1 constructs that .NET's
    // BCL XmlSchemaSet (XSD 1.0 only) refuses to compile. The loader runs
    // the schema source through MTConnect.XsdPreprocessor.StripXsd11Constructs
    // — the same code path the production XmlValidator uses — to drop those
    // constructs before the BCL reader sees them. The W3C xml.xsd and
    // xlink.xsd imports are XSD-1.0-compatible already and are added
    // unmodified.
    //
    // Source authority:
    //   - MTConnect v2.7 XSD MTConnectDevices_2.7.xsd lines 5029-5051
    //     (DevicesType complex type).
    //   - W3C XML Schema 1.0 — recommends element-sequence validation.
    [TestFixture]
    [Category("CppAgentJsonV2Envelope")]
    public class DevicesEnvelopeShapeXsdValidationTests
    {
        private const string ResourcePrefix = "MTConnect.Compliance.Tests.Schemas.";
        private const string XlinkResourceName = ResourcePrefix + "w3c.xlink.xsd";
        private const string XmlResourceName = ResourcePrefix + "w3c.xml.xsd";
        private const string DevicesV27Resource = ResourcePrefix + "v2_7.MTConnectDevices_2.7.xsd";

        private const string XlinkNamespace = "http://www.w3.org/1999/xlink";
        private const string XmlNamespace = "http://www.w3.org/XML/1998/namespace";

        private static XmlSchemaSet LoadDevicesV27Schema()
        {
            // Defense-in-depth: never resolve external schema locations
            // from disk or network. The W3C imports the MTConnect XSD
            // references (xlink + xml) are pre-seeded into the set so
            // targetNamespace matching resolves them in-memory.
            var schemaSet = new XmlSchemaSet
            {
                XmlResolver = null,
            };

            var asm = typeof(DevicesEnvelopeShapeXsdValidationTests).Assembly;

            using (var xlinkStream = asm.GetManifestResourceStream(XlinkResourceName)
                ?? throw new InvalidOperationException("Embedded W3C xlink schema not found."))
            {
                using var reader = XmlReader.Create(xlinkStream, new XmlReaderSettings { XmlResolver = null });
                schemaSet.Add(XlinkNamespace, reader);
            }

            using (var xmlStream = asm.GetManifestResourceStream(XmlResourceName)
                ?? throw new InvalidOperationException("Embedded W3C xml schema not found."))
            {
                using var reader = XmlReader.Create(xmlStream, new XmlReaderSettings { XmlResolver = null });
                schemaSet.Add(XmlNamespace, reader);
            }

            // The v2.7 MTConnectDevices XSD uses XSD 1.1 constructs the BCL
            // XmlSchemaSet (XSD 1.0 only) rejects. Run the source through
            // the same XsdPreprocessor the production XmlValidator uses so
            // tests and library share one source of truth for the
            // 1.0-compatible subset.
            string devicesXsdSource;
            using (var devicesStream = asm.GetManifestResourceStream(DevicesV27Resource)
                ?? throw new InvalidOperationException("Embedded MTConnectDevices_2.7 schema not found."))
            using (var srcReader = new StreamReader(devicesStream))
            {
                devicesXsdSource = srcReader.ReadToEnd();
            }

            var preprocessed = XsdPreprocessor.StripXsd11Constructs(devicesXsdSource);

            using (var preprocessedReader = new StringReader(preprocessed))
            using (var reader = XmlReader.Create(preprocessedReader, new XmlReaderSettings { XmlResolver = null }))
            {
                schemaSet.Add(targetNamespace: null, reader);
            }

            schemaSet.Compile();
            return schemaSet;
        }

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


        [Test]
        [Category("XsdLoadStrict")]
        public void Multi_device_probe_envelope_validates_against_MTConnectDevices_2_7_xsd()
        {
            // Tagged XsdLoadStrict because the v2.7 MTConnectDevices XSD
            // uses XSD 1.1 features that .NET's XSD-1.0-only XmlSchemaSet
            // cannot compile without preprocessing. The loader above runs
            // the source through MTConnect.XsdPreprocessor so the BCL
            // reader sees only the 1.0-compatible subset. Companion tests
            // in PR #165 pin the same envelope shape via XDocument
            // structural assertions that don't depend on the preprocessor.
            // Arrange: schema set + the production-emitted XML envelope.
            var schemas = LoadDevicesV27Schema();
            var document = BuildMultiDeviceDocument();

            using var xmlStream = XmlDevicesResponseDocument.ToXmlStream(
                document,
                extendedSchemas: null,
                styleSheet: null,
                indent: false,
                outputComments: false);
            Assert.That(xmlStream, Is.Not.Null);
            xmlStream!.Position = 0;

            // Act + Assert: validation errors must be empty.
            var errors = new System.Collections.Generic.List<string>();
            var readerSettings = new XmlReaderSettings
            {
                Schemas = schemas,
                ValidationType = ValidationType.Schema,
                ValidationFlags = XmlSchemaValidationFlags.ReportValidationWarnings,
                XmlResolver = null,
            };
            readerSettings.ValidationEventHandler += (sender, args) =>
            {
                errors.Add($"[{args.Severity}] {args.Message}");
            };

            using (var reader = XmlReader.Create(xmlStream, readerSettings))
            {
                while (reader.Read()) { /* drain */ }
            }

            Assert.That(errors, Is.Empty,
                "Multi-device Probe envelope failed XSD validation against v2.7 DevicesType:\n"
                + string.Join("\n", errors));
        }
    }
}
