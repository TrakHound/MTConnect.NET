// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.

// Pins TrakHound/MTConnect.NET#127 — the XML response document
// emitted by the agent contains the configured MTConnect Standard
// release in the Header `version` attribute. This is the
// wire-format-shape end of the issue: the previous bug was visible
// to consumers as `version="6.9.0.0"` in the XML payload, regardless
// of the agent's DefaultVersion.
//
// Spec sources:
//   - https://docs.mtconnect.org/ Part 1.0 §3 (Header) — defines the
//     `version` attribute on the Header element as the MTConnect
//     release the agent serves.
//   - https://schemas.mtconnect.org/schemas/MTConnectDevices_2.5.xsd
//     constrains the Header element's `version` attribute as
//     xs:string; cppagent emits a four-segment release string.

using System;
using System.IO;
using System.Linq;
using System.Reflection;
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
        public static System.Collections.Generic.IEnumerable<Version> AllSupportedVersions()
        {
            return typeof(MTConnect.MTConnectVersions)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.FieldType == typeof(Version))
                .Select(f => (Version)f.GetValue(null)!);
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
            broker.AddDevice(new Device { Uuid = "round-trip-device", Name = "RoundTripDevice" });
            return broker;
        }
    }
}
