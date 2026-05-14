// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;
using MTConnect.Devices.DataItems;
using NUnit.Framework;

namespace MTConnect.Tests.Common.Devices.DataItems
{
    /// <summary>
    /// Pins the class-level contract that DataItems whose SysML
    /// <c>result</c> property is typed as a UML class generalizing from
    /// <c>DataSet</c> default to the <c>DATA_SET</c> representation
    /// rather than <c>TABLE</c>. The MTConnect SysML model encodes the
    /// canonical structured representation of a DataItem in the parent
    /// chain of its result class — <c>DataSet</c> for one-dimensional
    /// key/value rows, <c>Table</c> for two-dimensional key/value
    /// matrices. The generator must walk the result class's
    /// generalization chain rather than hard-coding <c>TABLE</c> for
    /// any class-typed result.
    ///
    /// Sources:
    /// - SysML XMI: https://github.com/mtconnect/mtconnect_sysml_model
    ///   v2.7. The result class for each pinned DataItem generalizes
    ///   from the abstract <c>DataSet</c> class:
    ///     ALARM_LIMITS         -> AlarmLimitResult       -> DataSet
    ///     ALARM_LIMIT          -> AlarmLimitResult       -> DataSet
    ///     CONTROL_LIMITS       -> ControlLimitsResult    -> DataSet
    ///     CONTROL_LIMIT        -> ControlLimitsResult    -> DataSet
    ///     LOCATION_ADDRESS     -> AddressResult          -> DataSet
    ///     LOCATION_SPATIAL_GEOGRAPHIC -> GeographicLocationResult -> DataSet
    ///     SPECIFICATION_LIMITS -> SpecificationLimitsResult -> DataSet
    ///     SPECIFICATION_LIMIT  -> SpecificationLimitResult  -> DataSet
    ///     SENSOR_ATTACHMENT    -> SensorAttachmentResult -> DataSet
    /// - XSD: https://schemas.mtconnect.org/schemas/MTConnectStreams_2.7.xsd
    ///   declares the matching DataSet-substitution observation
    ///   elements (e.g. AlarmLimitsDataSet, ControlLimitsDataSet)
    ///   that pair with each of the DataItems below.
    /// - Reference implementation: cppagent v2.7.0.7 emits these
    ///   types as DATA_SET observations (not TABLE).
    /// </summary>
    [TestFixture]
    [Category("StructuredRepresentationClassifier")]
    public class StructuredRepresentationClassifierTests
    {
        [Test]
        public void AlarmLimits_DefaultRepresentation_Is_DataSet()
        {
            Assert.That(
                AlarmLimitsDataItem.DefaultRepresentation,
                Is.EqualTo(DataItemRepresentation.DATA_SET));
        }

        [Test]
        public void AlarmLimit_DefaultRepresentation_Is_DataSet()
        {
            Assert.That(
                AlarmLimitDataItem.DefaultRepresentation,
                Is.EqualTo(DataItemRepresentation.DATA_SET));
        }

        [Test]
        public void ControlLimits_DefaultRepresentation_Is_DataSet()
        {
            Assert.That(
                ControlLimitsDataItem.DefaultRepresentation,
                Is.EqualTo(DataItemRepresentation.DATA_SET));
        }

        [Test]
        public void ControlLimit_DefaultRepresentation_Is_DataSet()
        {
            Assert.That(
                ControlLimitDataItem.DefaultRepresentation,
                Is.EqualTo(DataItemRepresentation.DATA_SET));
        }

        [Test]
        public void LocationAddress_DefaultRepresentation_Is_DataSet()
        {
            Assert.That(
                LocationAddressDataItem.DefaultRepresentation,
                Is.EqualTo(DataItemRepresentation.DATA_SET));
        }

        [Test]
        public void LocationSpatialGeographic_DefaultRepresentation_Is_DataSet()
        {
            Assert.That(
                LocationSpatialGeographicDataItem.DefaultRepresentation,
                Is.EqualTo(DataItemRepresentation.DATA_SET));
        }

        [Test]
        public void SpecificationLimits_DefaultRepresentation_Is_DataSet()
        {
            Assert.That(
                SpecificationLimitsDataItem.DefaultRepresentation,
                Is.EqualTo(DataItemRepresentation.DATA_SET));
        }

        [Test]
        public void SpecificationLimit_DefaultRepresentation_Is_DataSet()
        {
            Assert.That(
                SpecificationLimitDataItem.DefaultRepresentation,
                Is.EqualTo(DataItemRepresentation.DATA_SET));
        }

        [Test]
        public void SensorAttachment_DefaultRepresentation_Is_DataSet()
        {
            Assert.That(
                SensorAttachmentDataItem.DefaultRepresentation,
                Is.EqualTo(DataItemRepresentation.DATA_SET));
        }

        [Test]
        public void FeatureMeasurement_DefaultRepresentation_Stays_Table()
        {
            // FeatureMeasurement's result class FeatureMeasurementResult
            // generalizes from Table (not DataSet); it must remain TABLE
            // after the classifier change to prevent over-correction.
            Assert.That(
                FeatureMeasurementDataItem.DefaultRepresentation,
                Is.EqualTo(DataItemRepresentation.TABLE));
        }

        [Test]
        public void MaintenanceList_DefaultRepresentation_Stays_Table()
        {
            // MaintenanceList's result class generalizes from Table; it
            // must remain TABLE after the classifier change.
            Assert.That(
                MaintenanceListDataItem.DefaultRepresentation,
                Is.EqualTo(DataItemRepresentation.TABLE));
        }
    }
}
