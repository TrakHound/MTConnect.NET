using System;
using MTConnect.Devices;
using MTConnect.Devices.DataItems;
using NUnit.Framework;

namespace MTConnect.NET_Common_Tests.V2_6_V2_7
{
    // Pins every DataItem type the v2.7 SysML XMI introduces.
    //
    //   - XMI: mtconnect/mtconnect_sysml_model @ v2.7 (SHA 25796ac591bb…)
    //          UML classes under Observation Information Model > Observation Types:
    //            * BindingState (Event)            — Bonding/joining state
    //            * Depth (Event)                   — Tool / part penetration
    //            * FixtureAssetId (Event)          — Asset reference
    //            * SwingAngle (Event)              — Mill/lathe swing
    //            * SwingDiameter (Event)           — Mill/lathe swing
    //            * SwingRadius (Event)             — Mill/lathe swing
    //            * TaskAssetId (Event)             — Asset reference
    //            * WaterHardness (Sample)          — Coolant water mineral level
    //   - XSD: schemas.mtconnect.org/schemas/MTConnectStreams_2.7.xsd
    //          (each TypeId is encoded in the EventEnum / SampleEnum
    //          enumerations.)
    //   - Prose: MTConnect Standard Part_2.0_Streams_v2.7 §11/§13 "Event/Sample
    //          types" — describes intended use of each type.
    [TestFixture]
    public class V2_7DataItemTypeTests
    {
        // Categories below match what the v2.7 SysML XMI declares — the spec
        // authority. Several types that look "measurement-y" (SwingAngle, Depth,
        // etc.) are EVENT in the spec rather than SAMPLE; locking them so a
        // future regen drift is caught immediately.
        [TestCase(typeof(BindingStateDataItem), "BINDING_STATE", DataItemCategory.EVENT)]
        [TestCase(typeof(DepthDataItem), "DEPTH", DataItemCategory.EVENT)]
        [TestCase(typeof(FixtureAssetIdDataItem), "FIXTURE_ASSET_ID", DataItemCategory.EVENT)]
        [TestCase(typeof(SwingAngleDataItem), "SWING_ANGLE", DataItemCategory.EVENT)]
        [TestCase(typeof(SwingDiameterDataItem), "SWING_DIAMETER", DataItemCategory.EVENT)]
        [TestCase(typeof(SwingRadiusDataItem), "SWING_RADIUS", DataItemCategory.EVENT)]
        [TestCase(typeof(TaskAssetIdDataItem), "TASK_ASSET_ID", DataItemCategory.EVENT)]
        [TestCase(typeof(WaterHardnessDataItem), "WATER_HARDNESS", DataItemCategory.SAMPLE)]
        public void V2_7_DataItem_constructs_with_correct_metadata(
            Type dataItemType, string expectedTypeId, DataItemCategory expectedCategory)
        {
            // Wrap with Assert.DoesNotThrow so a missing parameterless ctor
            // surfaces as a clear NUnit failure with the offending type name
            // rather than a bare MissingMethodException.
            object? instance = null;
            Assert.DoesNotThrow(() => instance = Activator.CreateInstance(dataItemType),
                $"{dataItemType.Name} should have a public parameterless constructor");
            Assert.That(instance, Is.Not.Null);
            Assert.That(instance, Is.InstanceOf<DataItem>());

            var di = (DataItem)instance!;
            Assert.That(di.Type, Is.EqualTo(expectedTypeId),
                $"{dataItemType.Name}.Type should be the spec TypeId");
            Assert.That(di.Category, Is.EqualTo(expectedCategory),
                $"{dataItemType.Name}.Category should be {expectedCategory}");

            var typeIdConst = dataItemType.GetField("TypeId",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)?.GetRawConstantValue();
            Assert.That(typeIdConst, Is.EqualTo(expectedTypeId),
                $"{dataItemType.Name}.TypeId static const should match the spec TypeId");
        }
    }
}
