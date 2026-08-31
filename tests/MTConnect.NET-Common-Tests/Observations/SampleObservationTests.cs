// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using MTConnect.Devices;
using MTConnect.Devices.DataItems;
using MTConnect.Observations;
using MTConnect.Tests.Common.TestHelpers;
using NUnit.Framework;

namespace MTConnect.NET_Common_Tests.Observations
{
    // Version-gated Sample-envelope round-trip assertions for the
    // SAMPLE-category DataItems introduced across MTConnect Standard
    // versions (currently WaterHardness at v2.7).
    //
    //   - XMI: https://github.com/mtconnect/mtconnect_sysml_model @ tag v2.7
    //          UML class `WaterHardnessDataItem` declares
    //          `category = SAMPLE`, MinimumVersion = v2.7. (Hardness is
    //          measured in mineral content of cooling water — used in
    //          machining workflows where coolant chemistry affects tool
    //          life.)
    //   - XSD: https://schemas.mtconnect.org/schemas/MTConnectStreams_2.7.xsd
    //          enum `SampleEnum` value `WATER_HARDNESS` is the
    //          sample-category element name on the wire.
    //   - Prose: MTConnect Standard Part_2.0_Streams_v2.7 section 11
    //          "Sample observation types" — describes how SAMPLE-category
    //          observations carry continuous-numeric values reported at
    //          agent-defined intervals.
    //
    // Every fixture below is matrix-parameterised over
    // MTConnectVersionMatrix.All per plan Design Decision D1
    // (2026-08-19). Assume.That gates each row to versions where the
    // sample type shipped.
    /// <summary>Pins the behavior expressed by the test name: sample observation tests.</summary>
    [TestFixture]
    public class SampleObservationTests
    {
        // Source: XMI v2.7 — `WaterHardness` is the only SAMPLE-category
        // type introduced in v2.7 (the rest are EVENT). Tests the
        // round-trip from creating a DataItem of this v2.7 type, attaching
        // a SampleValueObservation, and reading back the value. If the
        // library starts dropping the link between the DataItem's TypeId
        // and the observation's reported type, this test catches it.
        /// <summary>Pins the behavior expressed by the test name: water hardness sample observation round trip.</summary>
        /// <param name="v">The MTConnect Standard version under test.</param>
        [TestCaseSource(typeof(MTConnectVersionMatrix), nameof(MTConnectVersionMatrix.All))]
        public void WaterHardness_sample_observation_round_trip(Version v)
        {
            Assume.That(v, Is.GreaterThanOrEqualTo(MTConnectVersions.Version27),
                "WaterHardness was introduced in MTConnect v2.7.");

            var dataItem = new WaterHardnessDataItem("dev01");
            Assert.That(dataItem.Category, Is.EqualTo(DataItemCategory.SAMPLE));

            var observation = new SampleValueObservation
            {
                DataItemId = dataItem.Id,
                Result = "12.5",
                Timestamp = System.DateTime.UtcNow,
                Sequence = 42,
            };

            // Carrier preserves DataItemId so a downstream lookup of the
            // type (DataItemId -> TypeId via the agent's DataItem registry)
            // resolves back to WATER_HARDNESS.
            Assert.That(observation.DataItemId, Is.EqualTo(dataItem.Id));
            Assert.That(observation.Result, Is.EqualTo("12.5"));
            Assert.That(observation.Sequence, Is.EqualTo(42));

            // The DataItem's Type field is what cppagent JSON / XML
            // formatters look at when rendering the SAMPLE element name.
            Assert.That(dataItem.Type, Is.EqualTo("WATER_HARDNESS"));
        }
    }
}
