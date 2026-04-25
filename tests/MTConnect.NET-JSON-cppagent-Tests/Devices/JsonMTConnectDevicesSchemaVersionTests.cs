// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.NET_JSON_cppagent_Tests.TestHelpers;
using NUnit.Framework;
using System;

namespace MTConnect.NET_JSON_cppagent_Tests.Devices
{
    /// <summary>
    /// Asserts that the cppagent JSON-MQTT Devices envelope emits the
    /// configured MTConnect release as <c>schemaVersion</c> instead of
    /// the hardcoded <c>"2.0"</c> literal.
    ///
    /// Pre-fix: every case fails with <c>Expected "&lt;version&gt;" / But was "2.0"</c>.
    /// Post-fix: every case passes; the wire output matches cppagent's
    /// two-segment format (e.g. <c>"2.5"</c> for v2.5).
    /// </summary>
    [TestFixture]
    [Category("SchemaVersionFromConfiguration")]
    public class JsonMTConnectDevicesSchemaVersionTests
    {
        [TestCaseSource(typeof(VersionMatrix), nameof(VersionMatrix.All))]
        public void Devices_envelope_schemaVersion_equals_configured_release(Version configured)
        {
            var envelope = EnvelopeFixtures.BuildDevicesEnvelope(configured);

            Assert.That(
                envelope.SchemaVersion,
                Is.EqualTo(configured.ToString()),
                "Devices.schemaVersion must mirror AgentConfiguration.DefaultVersion (issue #128).");
        }
    }
}
