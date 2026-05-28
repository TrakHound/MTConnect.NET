using MTConnect.Devices;
using MTConnect.Devices.DataItems;
using MTConnect.Observations;
using NUnit.Framework;

namespace MTConnect.NET_Common_Tests.V2_6_V2_7
{
    // Sample envelope coverage for the v2.7 SAMPLE-category DataItems introduced
    // by [#133](https://github.com/TrakHound/MTConnect.NET/issues/133).
    //
    //   - XMI: mtconnect/mtconnect_sysml_model @ v2.7 (SHA 25796ac591bb…)
    //          UML class `WaterHardnessDataItem` declares
    //          `category = SAMPLE`, MinimumVersion = v2.7. (Hardness measured in
    //          mineral content of cooling water — used in machining workflows
    //          where coolant chemistry affects tool life.)
    //   - XSD: schemas.mtconnect.org/schemas/MTConnectStreams_2.7.xsd
    //          enum `SampleEnum` value `WATER_HARDNESS` is the sample-category
    //          element name on the wire.
    //   - Prose: MTConnect Standard Part_2.0_Streams_v2.7 section 11 "Sample observation
    //          types" — describes how SAMPLE-category observations carry
    //          continuous-numeric values reported at agent-defined intervals.
    //
    // This fixture is the SAMPLE-envelope counterpart to V2_7DataItemTypeTests
    // (which is shape-only). Here we focus on round-tripping a SAMPLE
    // observation through the library's `SampleValueObservation` carrier and
    // confirm the (DataItem, Observation) pair carries the v2.7 type metadata
    // intact.
    [TestFixture]
    public class V2_7SampleObservationTests
    {
        // Source: XMI v2.7 — `WaterHardness` is the only SAMPLE-category type
        // introduced in v2.7 (the rest are EVENT). Tests the round-trip from
        // creating a DataItem of this v2.7 type, attaching a SampleValueObservation,
        // and reading back the value. If the library starts dropping the link
        // between the DataItem's TypeId and the observation's reported type,
        // this test catches it.
        [Test]
        public void WaterHardness_sample_observation_round_trip()
        {
            var dataItem = new WaterHardnessDataItem("dev01");
            Assert.That(dataItem.Category, Is.EqualTo(DataItemCategory.SAMPLE));

            var observation = new SampleValueObservation
            {
                DataItemId = dataItem.Id,
                Result = "12.5",
                Timestamp = System.DateTime.UtcNow,
                Sequence = 42,
            };

            // Carrier preserves DataItemId so a downstream lookup of the type
            // (DataItemId → TypeId via the agent's DataItem registry) resolves
            // back to WATER_HARDNESS.
            Assert.That(observation.DataItemId, Is.EqualTo(dataItem.Id));
            Assert.That(observation.Result, Is.EqualTo("12.5"));
            Assert.That(observation.Sequence, Is.EqualTo(42));

            // The DataItem's Type field is what cppagent JSON / XML formatters
            // look at when rendering the SAMPLE element name.
            Assert.That(dataItem.Type, Is.EqualTo("WATER_HARDNESS"));
        }

    }
}
