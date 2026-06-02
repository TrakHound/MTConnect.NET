// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.NET_JSON_cppagent_Tests.TestHelpers;
using NUnit.Framework;
using System;

namespace MTConnect.NET_JSON_cppagent_Tests.Regressions
{
    /// <summary>
    /// Pinned regression for
    /// <see href="https://github.com/TrakHound/MTConnect.NET/issues/128">issue #128</see>.
    ///
    /// JSON-cppagent envelopes (<c>MTConnectStreams</c> + <c>MTConnectDevices</c>)
    /// MUST emit the configured MTConnect release as <c>schemaVersion</c>;
    /// the pre-fix code stamped a literal <c>"2.0"</c> regardless of
    /// <c>AgentConfiguration.DefaultVersion</c>.
    /// </summary>
    [TestFixture]
    public class Issue128_SchemaVersionConfiguredTests
    {
        /// <summary>Pins the behaviour expressed by the test name: streams schema version equals configured.</summary>
        /// <param name="configured">The configured.</param>
        [TestCaseSource(typeof(VersionMatrix), nameof(VersionMatrix.All))]
        public void Streams_schemaVersion_equals_configured(Version configured)
        {
            var envelope = EnvelopeFixtures.BuildStreamsEnvelope(configured);
            Assert.That(envelope.SchemaVersion, Is.EqualTo(configured.ToString()));
        }

        /// <summary>Pins the behaviour expressed by the test name: devices schema version equals configured.</summary>
        /// <param name="configured">The configured.</param>
        [TestCaseSource(typeof(VersionMatrix), nameof(VersionMatrix.All))]
        public void Devices_schemaVersion_equals_configured(Version configured)
        {
            var envelope = EnvelopeFixtures.BuildDevicesEnvelope(configured);
            Assert.That(envelope.SchemaVersion, Is.EqualTo(configured.ToString()));
        }
    }
}
