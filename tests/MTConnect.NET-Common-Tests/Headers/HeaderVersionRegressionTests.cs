// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.

// Pins TrakHound/MTConnect.NET#127 — every response Header.version
// equals the configured MTConnect Standard release the agent serves,
// not the library assembly version.
//
// Spec sources:
//   - https://docs.mtconnect.org/ Part 1.0 section 3 (Header), Part 2.0 section 7
//     (Streams envelope), Part 3.0 section 5 (Devices envelope), Part 4.0 section 5
//     (Assets envelope) — Header.version is the MTConnect Standard
//     release the agent serves.
//   - XSD: MTConnectDevices_<vN.M>.xsd, MTConnectStreams_<vN.M>.xsd,
//     MTConnectAssets_<vN.M>.xsd, MTConnectError_<vN.M>.xsd at
//     https://schemas.mtconnect.org — the Header element's `version`
//     attribute is xs:string formatted as the four-segment release
//     string by the cppagent reference implementation.

using System;
using System.Linq;
using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Errors;
using MTConnect.Tests.Common.TestHelpers;
using NUnit.Framework;

namespace MTConnect.Tests.Common.Headers
{
    [TestFixture]
    public class HeaderVersionRegressionTests
    {
        // Returns the canonical four-segment string an agent configured
        // for `configuredVersion` must emit in Header.version. Pads
        // build + revision with zero to match the cppagent reference
        // shape (e.g. cppagent emits `2.7.0.0` for v2.7).
        private static string ExpectedHeaderVersion(Version configuredVersion)
        {
            return new Version(
                configuredVersion.Major,
                configuredVersion.Minor,
                0,
                0).ToString();
        }

        private static MTConnectAgentBroker BuildBroker(Version configuredVersion)
        {
            var configuration = new AgentConfiguration
            {
                DefaultVersion = configuredVersion
            };
            var broker = new MTConnectAgentBroker(configuration);

            // Add a device that the broker can serve at every supported
            // MTConnect release. The default Agent device introduced in
            // v1.7 (see MTConnect.Devices.Agent.MinimumVersion) drops
            // out of the response below v1.7, which is unrelated to
            // this fixture's `Header.version` assertion. Adding a
            // bare Device keeps the response document non-null across
            // every row of the version matrix.
            broker.AddDevice(new Device { Uuid = "test-device", Name = "TestDevice" });
            return broker;
        }

        [TestCaseSource(typeof(MTConnectVersionMatrix), nameof(MTConnectVersionMatrix.All))]
        public void Devices_header_version_equals_configured_mtconnect_release(Version configuredVersion)
        {
            using var broker = BuildBroker(configuredVersion);
            var document = broker.GetDevicesResponseDocument();

            Assert.That(document, Is.Not.Null,
                "Broker must yield a Devices response document for the default agent device.");
            Assert.That(document!.Header, Is.Not.Null);
            Assert.That(
                document.Header.Version,
                Is.EqualTo(ExpectedHeaderVersion(configuredVersion)));
        }

        [TestCaseSource(typeof(MTConnectVersionMatrix), nameof(MTConnectVersionMatrix.All))]
        public void Assets_header_version_equals_configured_mtconnect_release(Version configuredVersion)
        {
            using var broker = BuildBroker(configuredVersion);
            var document = broker.GetAssetsResponseDocument();

            Assert.That(document, Is.Not.Null,
                "Broker must yield an Assets response document (empty assets list permitted).");
            Assert.That(document!.Header, Is.Not.Null);
            Assert.That(
                document.Header.Version,
                Is.EqualTo(ExpectedHeaderVersion(configuredVersion)));
        }

        [TestCaseSource(typeof(MTConnectVersionMatrix), nameof(MTConnectVersionMatrix.All))]
        public void Error_header_version_equals_configured_mtconnect_release(Version configuredVersion)
        {
            using var broker = BuildBroker(configuredVersion);
            var document = broker.GetErrorResponseDocument(ErrorCode.UNSUPPORTED, "test");

            Assert.That(document, Is.Not.Null);
            Assert.That(document!.Header, Is.Not.Null);
            Assert.That(
                document.Header.Version,
                Is.EqualTo(ExpectedHeaderVersion(configuredVersion)));
        }

        [TestCaseSource(typeof(MTConnectVersionMatrix), nameof(MTConnectVersionMatrix.All))]
        public void Devices_header_version_equals_configured_release_when_passed_explicitly(Version configuredVersion)
        {
            // Independent path: the optional `mtconnectVersion` parameter
            // overrides the broker's default and must be reflected in
            // Header.version. Pins the explicit-version overload so a
            // future regression on either path is caught.
            using var broker = BuildBroker(MTConnectVersions.Version10);
            var document = broker.GetDevicesResponseDocument(configuredVersion);

            Assert.That(document, Is.Not.Null);
            Assert.That(document!.Header, Is.Not.Null);
            Assert.That(
                document.Header.Version,
                Is.EqualTo(ExpectedHeaderVersion(configuredVersion)));
        }

        [Test]
        public void No_response_envelope_emits_the_library_assembly_version()
        {
            // Cheap paranoia check — guards against any future
            // diagnostic-style emission of MTConnectAgent.Version
            // re-leaking into a wire-format header.
            using var broker = BuildBroker(MTConnectVersions.Version25);

            var libraryVersion = typeof(MTConnectAgent).Assembly
                .GetName().Version!.ToString();

            var devices = broker.GetDevicesResponseDocument();
            var assets = broker.GetAssetsResponseDocument();
            var error = broker.GetErrorResponseDocument(ErrorCode.UNSUPPORTED, "test");

            Assert.Multiple(() =>
            {
                Assert.That(devices!.Header.Version, Is.Not.EqualTo(libraryVersion),
                    "Devices Header.version must not echo the library assembly version.");
                Assert.That(assets!.Header.Version, Is.Not.EqualTo(libraryVersion),
                    "Assets Header.version must not echo the library assembly version.");
                Assert.That(error!.Header.Version, Is.Not.EqualTo(libraryVersion),
                    "Error Header.version must not echo the library assembly version.");
            });
        }
    }
}
