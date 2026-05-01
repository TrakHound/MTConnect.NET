// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Linq;
using System.Reflection;
using MTConnect.Devices;
using MTConnect.Devices.DataItems;
using NUnit.Framework;

namespace MTConnect.Tests.Common.Devices.DataItems
{
    /// <summary>
    /// Pins the class-level contract that AssetCountDataItem defaults to
    /// the DATA_SET representation. The MTConnect Standard models
    /// ASSET_COUNT as a key/value map of asset-type to count, which is
    /// the canonical DATA_SET shape; the generated DataItem class must
    /// carry that default so consumers constructing the DataItem
    /// directly observe a spec-conformant representation without having
    /// to override it post-construction.
    ///
    /// Sources:
    /// - SysML XMI: https://github.com/mtconnect/mtconnect_sysml_model
    ///   UML class AssetCount, UML ID
    ///   _19_0_3_68e0225_1640602520420_217627_44; the matching
    ///   EventEnum::ASSET_COUNT literal carries the spec marker
    ///   "{{term(data set)}} of the number of {{termplural(Asset)}} of
    ///   a given type for a {{term(Device)}}".
    /// - XSD:
    ///   https://schemas.mtconnect.org/schemas/MTConnectStreams_2.7.xsd
    ///   declares AssetCountDataSet with substitutionGroup="Event"
    ///   carrying AssetCountEntry rows.
    /// - Prose: MTConnect Standard Part 4 - Assets Information Model
    ///   describes ASSET_COUNT as a per-type asset tally.
    /// - Reference implementation: cppagent v2.7.0.7 emits the
    ///   AssetCountDataSet observation element with
    ///   representation="DATA_SET".
    /// </summary>
    [TestFixture]
    [Category("AssetCountIsDataSet")]
    public class AssetCountDataItemDefaultRepresentationTests
    {
        [Test]
        public void DefaultRepresentation_Const_Equals_DataSet()
        {
            Assert.That(
                AssetCountDataItem.DefaultRepresentation,
                Is.EqualTo(DataItemRepresentation.DATA_SET),
                "AssetCountDataItem.DefaultRepresentation must equal DATA_SET");
        }

        [Test]
        public void Parameterless_Constructor_Stamps_DataSet_Representation()
        {
            var item = new AssetCountDataItem();

            Assert.That(
                item.Representation,
                Is.EqualTo(DataItemRepresentation.DATA_SET),
                "the parameterless constructor must stamp Representation = DATA_SET");
        }

        [Test]
        public void DeviceId_Constructor_Stamps_DataSet_Representation()
        {
            var item = new AssetCountDataItem("Device-1");

            Assert.That(
                item.Representation,
                Is.EqualTo(DataItemRepresentation.DATA_SET),
                "the (deviceId) constructor must stamp Representation = DATA_SET");
        }

        [Test]
        public void DeviceId_Constructor_Builds_V2_Conformant_Id()
        {
            var item = new AssetCountDataItem("Device-1");

            // The id must follow CreateId(parentId, name) shape:
            // it starts with the supplied deviceId, contains the
            // canonical NameId, and uses '_' as the separator.
            Assert.That(item.Id, Is.Not.Null.And.Not.Empty);
            Assert.That(item.Id, Does.StartWith("Device-1"));
            Assert.That(item.Id, Does.Contain(AssetCountDataItem.NameId));
            Assert.That(item.Id, Is.EqualTo($"Device-1_{AssetCountDataItem.NameId}"));
        }

        [Test]
        public void MinimumVersion_Reports_Version20()
        {
            var item = new AssetCountDataItem();

            // ASSET_COUNT was introduced in MTConnect v2.0; the
            // generated DataItem must surface that version through the
            // MinimumVersion override so consumers can gate features
            // by introduction version. Pinning this assertion keeps
            // the override exercised under coverage in addition to
            // pinning the spec-introduction year for ASSET_COUNT.
            Assert.That(item.MinimumVersion, Is.EqualTo(MTConnectVersions.Version20));
        }

        [Test]
        public void DefaultRepresentation_Field_Is_Public_Const_DataSet()
        {
            var field = typeof(AssetCountDataItem).GetField(
                nameof(AssetCountDataItem.DefaultRepresentation),
                BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            Assert.That(field, Is.Not.Null,
                "AssetCountDataItem must expose a public static DefaultRepresentation field");
            Assert.That(field!.IsPublic, Is.True,
                "DefaultRepresentation must be public");
            Assert.That(field.IsStatic, Is.True,
                "DefaultRepresentation must be static");
            Assert.That(field.IsLiteral || field.IsInitOnly, Is.True,
                "DefaultRepresentation must be const or readonly");
            Assert.That(field.FieldType, Is.EqualTo(typeof(DataItemRepresentation)),
                "DefaultRepresentation must be typed DataItemRepresentation");

            // GetRawConstantValue on a const enum returns the
            // underlying integer; GetValue(null) returns the boxed
            // enum. Read the boxed enum to keep the assertion typed.
            var value = field.GetValue(null);
            Assert.That(value, Is.EqualTo(DataItemRepresentation.DATA_SET),
                "DefaultRepresentation reflective value must equal DATA_SET");
        }
    }
}
