// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.

// Pins TrakHound/MTConnect.NET#127 — the XML response document
// emitted by the agent contains the configured MTConnect Standard
// release in the Header `version` attribute. This is the
// wire-format-shape end of the issue: the previous bug was visible
// to consumers as `version="6.9.0.0"` in the XML payload, regardless
// of the agent's DefaultVersion.
//
// Spec sources:
//   - https://docs.mtconnect.org/ Part 1.0 section 3 (Header) — defines the
//     `version` attribute on the Header element as the MTConnect
//     release the agent serves.
//   - https://schemas.mtconnect.org/schemas/MTConnectDevices_2.5.xsd
//     constrains the Header element's `version` attribute as
//     xs:string; cppagent emits a four-segment release string.

using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Devices.Xml;
using NUnit.Framework;

namespace MTConnect.Tests.Xml.Headers
{
    [TestFixture]
    public class HeaderVersionXmlRoundTripTests
    {
        // Versions for which the MTConnect XML wire-format library
        // (libraries/MTConnect.NET-XML) currently provides namespace
        // and schema-location mappings via Namespaces.GetDevices /
        // Schemas.GetDevices. The set is hardcoded rather than
        // reflected from MTConnect.MTConnectVersions so that adding a
        // new MTConnectVersions constant (which only declares the
        // version, not the XML namespace/schema mappings) does not
        // silently introduce a parametric test case that the XML
        // library cannot serialize. Versions added beyond this set
        // require matching entries in Namespaces.cs and Schemas.cs
        // before the test case can be added here.
        public static System.Collections.Generic.IEnumerable<Version> AllSupportedVersions()
        {
            return new[]
            {
                new Version(1, 0),
                new Version(1, 1),
                new Version(1, 2),
                new Version(1, 3),
                new Version(1, 4),
                new Version(1, 5),
                new Version(1, 6),
                new Version(1, 7),
                new Version(1, 8),
                new Version(2, 0),
                new Version(2, 1),
                new Version(2, 2),
                new Version(2, 3),
                new Version(2, 4),
                new Version(2, 5),
            };
        }

        private static string ExpectedHeaderVersion(Version configuredVersion)
        {
            return new Version(
                configuredVersion.Major,
                configuredVersion.Minor,
                0,
                0).ToString();
        }

        [TestCaseSource(nameof(AllSupportedVersions))]
        public void Devices_xml_payload_carries_configured_mtconnect_release_in_header_version(Version configuredVersion)
        {
            using var broker = BuildBroker(configuredVersion);
            var document = broker.GetDevicesResponseDocument();

            Assert.That(document, Is.Not.Null);

            using var xmlStream = XmlDevicesResponseDocument.ToXmlStream(document!);
            Assert.That(xmlStream, Is.Not.Null);

            xmlStream!.Position = 0;
            using var reader = new StreamReader(xmlStream);
            var xml = reader.ReadToEnd();

            // Use XmlReader to locate the Header element robustly
            // (XML is namespace-qualified per the MTConnect schema).
            var doc = XDocument.Parse(xml);
            var header = doc.Root!.Elements()
                .First(e => e.Name.LocalName == "Header");

            Assert.That(
                header.Attribute("version")?.Value,
                Is.EqualTo(ExpectedHeaderVersion(configuredVersion)),
                "Header.version attribute on the wire-format XML envelope must equal the configured MTConnect release.");
        }

        private static MTConnectAgentBroker BuildBroker(Version configuredVersion)
        {
            var configuration = new AgentConfiguration
            {
                DefaultVersion = configuredVersion
            };
            var broker = new MTConnectAgentBroker(configuration);
            // Construct the test Device with every required field set
            // explicitly. The Device default constructor is not guaranteed
            // to populate Id / Name / Uuid (older revisions auto-generated
            // them; newer revisions strip those defaults to honor the XSD
            // `uuid` "for entire life" identity contract). Setting them
            // here keeps the test green across both shapes.
            broker.AddDevice(new Device
            {
                Id = "round-trip-device",
                Uuid = "round-trip-device",
                Name = "RoundTripDevice",
            });
            return broker;
        }
    }
}
