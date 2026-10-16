// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MTConnect.Assets;
using MTConnect.Assets.Fixture;
using MTConnect.Assets.Pallet;
using MTConnect.Assets.Xml;
using MTConnect.Assets.Xml.Fixture;
using MTConnect.Assets.Xml.Pallet;
using MTConnect.Headers;
using NUnit.Framework;

namespace MTConnect.Tests.XML.Assets
{
    /// <summary>
    /// Pins the <c>Pallet</c> and <c>Fixture</c> XML surrogates: round trips of every element and of
    /// the measurements, the pallet's <c>Type</c> element mapping to <see cref="PalletAsset.PalletType"/>,
    /// reading them from an MTConnectAssets document, and the 2.5 layout (only <c>Measurements</c>,
    /// without <c>nominal</c>) against the 2.6+ one.
    /// </summary>
    [TestFixture]
    [Category("XmlPhysicalAsset")]
    public class XmlPhysicalAssetTests
    {
        private static readonly DateTime Timestamp = new DateTime(2024, 5, 6, 7, 8, 9, DateTimeKind.Utc);
        private static readonly DateTime Calibrated = new DateTime(2024, 1, 2, 3, 4, 5, DateTimeKind.Utc);


        private static PalletAsset CreatePallet()
        {
            var pallet = new PalletAsset
            {
                AssetId = "P1",
                DeviceUuid = "dev",
                Timestamp = Timestamp,
                PalletType = "Machine",
                PalletId = "pallet-1",
                PalletNumber = 4,
                ClampingMethod = "HYDRAULIC",
                MountingMethod = "BOLTED",
                CalibrationDate = Calibrated,
                Description = "Pallet description"
            };
            pallet.Measurements = new IMeasurement[]
            {
                new LoadedHeightMeasurement(410.5) { Units = "MILLIMETER", Nominal = 400 },
                new WeightMeasurement(12.25) { Units = "KILOGRAM", NativeUnits = "POUND", Maximum = 15, Minimum = 10, SignificantDigits = 2 }
            };
            return pallet;
        }

        private static string ToXmlString(IAsset asset)
        {
            using (var stream = XmlAsset.ToXml(asset))
            {
                Assert.That(stream, Is.Not.Null, "ToXml returned no stream");
                return new StreamReader(stream, Encoding.UTF8).ReadToEnd();
            }
        }

        private static XElement WriteInAssetsDocument(Version version, IAsset asset)
        {
            var document = new AssetsResponseDocument
            {
                Header = new MTConnectAssetsHeader { CreationTime = Timestamp, Sender = "test", Version = version.ToString() },
                Version = version,
                Assets = new[] { asset }
            };

            using (var stream = XmlAssetsResponseDocument.ToXmlStream(document))
            {
                stream.Seek(0, SeekOrigin.Begin);
                return XDocument.Load(stream).Root.Elements().First(o => o.Name.LocalName == "Assets").Elements().Single();
            }
        }

        private static string[] ChildNames(XElement element) => element.Elements().Select(o => o.Name.LocalName).ToArray();


        [Test]
        public void Surrogates_are_registered_by_asset_type()
        {
            Assert.That(XmlAsset.GetAssetType(PalletAsset.TypeId), Is.EqualTo(typeof(XmlPalletAsset)));
            Assert.That(XmlAsset.GetAssetType(FixtureAsset.TypeId), Is.EqualTo(typeof(XmlFixtureAsset)));
            Assert.That(XmlAsset.GetAssetType("Physical"), Is.Null);
        }

        [Test]
        public void Pallet_round_trips()
        {
            var xml = ToXmlString(CreatePallet());

            var read = XmlAsset.FromXml(PalletAsset.TypeId, Encoding.UTF8.GetBytes(xml)) as PalletAsset;

            Assert.That(read, Is.Not.Null, xml);
            Assert.That(read.Type, Is.EqualTo(PalletAsset.TypeId));
            Assert.That(read.AssetId, Is.EqualTo("P1"));
            Assert.That(read.PalletType, Is.EqualTo("Machine"));
            Assert.That(read.PalletId, Is.EqualTo("pallet-1"));
            Assert.That(read.PalletNumber, Is.EqualTo(4));
            Assert.That(read.ClampingMethod, Is.EqualTo("HYDRAULIC"));
            Assert.That(read.MountingMethod, Is.EqualTo("BOLTED"));
            Assert.That(read.CalibrationDate, Is.EqualTo(Calibrated));
            Assert.That(read.ManufactureDate, Is.EqualTo(default(DateTime)));
            Assert.That(read.Description, Is.EqualTo("Pallet description"));

            var measurements = read.Measurements.ToList();
            Assert.That(measurements.Select(o => o.GetType()), Is.EqualTo(new[] { typeof(WeightMeasurement), typeof(LoadedHeightMeasurement) }), "Written in schema order");

            var weight = measurements[0];
            Assert.That(weight.Value, Is.EqualTo(12.25));
            Assert.That(weight.Units, Is.EqualTo("KILOGRAM"));
            Assert.That(weight.NativeUnits, Is.EqualTo("POUND"));
            Assert.That(weight.Maximum, Is.EqualTo(15));
            Assert.That(weight.Minimum, Is.EqualTo(10));
            Assert.That(weight.SignificantDigits, Is.EqualTo(2));
            Assert.That(measurements[1].Nominal, Is.EqualTo(400));
        }

        [Test]
        public void Pallet_type_is_written_as_the_Type_element()
        {
            var element = XElement.Parse(ToXmlString(CreatePallet()));

            Assert.That(element.Name.LocalName, Is.EqualTo("Pallet"));
            Assert.That(element.Elements().Single(o => o.Name.LocalName == "Type").Value, Is.EqualTo("Machine"));
            Assert.That(element.Attribute("type"), Is.Null);
        }

        [Test]
        public void Fixture_round_trips()
        {
            var fixture = new FixtureAsset
            {
                AssetId = "X1",
                DeviceUuid = "dev",
                Timestamp = Timestamp,
                FixtureId = "fixture-1",
                FixtureNumber = 2,
                ClampingMethod = "MANUAL",
                MountingMethod = "MAGNETIC",
                NextInspectionDate = Calibrated
            };
            fixture.Measurements = new IMeasurement[] { new SwingMeasurement(300) };

            var xml = ToXmlString(fixture);
            var read = XmlAsset.FromXml(FixtureAsset.TypeId, Encoding.UTF8.GetBytes(xml)) as FixtureAsset;

            Assert.That(read, Is.Not.Null, xml);
            Assert.That(read.Type, Is.EqualTo(FixtureAsset.TypeId));
            Assert.That(read.FixtureId, Is.EqualTo("fixture-1"));
            Assert.That(read.FixtureNumber, Is.EqualTo(2));
            Assert.That(read.ClampingMethod, Is.EqualTo("MANUAL"));
            Assert.That(read.MountingMethod, Is.EqualTo("MAGNETIC"));
            Assert.That(read.NextInspectionDate, Is.EqualTo(Calibrated));
            Assert.That(read.Measurements.Single(), Is.InstanceOf<SwingMeasurement>());
            Assert.That(read.Measurements.Single().Value, Is.EqualTo(300));
        }

        [Test]
        public void Unknown_measurement_types_are_kept_by_name()
        {
            var xml = @"<Fixture assetId=""X1"" deviceUuid=""dev"" timestamp=""2024-05-06T07:08:09Z""><Measurements><Depth units=""MILLIMETER"">5</Depth></Measurements></Fixture>";

            var read = XmlAsset.FromXml(FixtureAsset.TypeId, Encoding.UTF8.GetBytes(xml)) as FixtureAsset;

            var measurement = read?.Measurements?.Single() as Measurement;
            Assert.That(measurement?.Type, Is.EqualTo("Depth"));
            Assert.That(measurement.Value, Is.EqualTo(5));
            Assert.That(XElement.Parse(ToXmlString(read)).Descendants().Any(o => o.Name.LocalName == "Depth"), Is.True);
        }

        [Test]
        public void Version_2_5_documents_carry_only_the_measurements()
        {
            var element = WriteInAssetsDocument(MTConnectVersions.Version25, CreatePallet());

            Assert.That(ChildNames(element), Is.EqualTo(new[] { "Measurements" }));
            Assert.That(element.Descendants().Any(o => o.Attribute("nominal") != null), Is.False);
        }

        [Test]
        public void Version_2_7_documents_carry_the_full_content()
        {
            var element = WriteInAssetsDocument(MTConnectVersions.Version27, CreatePallet());

            Assert.That(ChildNames(element), Is.EqualTo(new[] { "Type", "PalletId", "PalletNumber", "ClampingMethod", "MountingMethod", "CalibrationDate", "Measurements", "Description" }));
            Assert.That(element.Descendants().Any(o => o.Attribute("nominal")?.Value == "400"), Is.True);
        }

        [Test]
        public void Physical_assets_are_excluded_from_documents_before_2_5()
        {
            Assert.That(CreatePallet().Process(MTConnectVersions.Version24), Is.Null);
        }

        [Test]
        public void Assets_document_reads_pallets_and_fixtures()
        {
            var xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<MTConnectAssets xmlns=""urn:mtconnect.org:MTConnectAssets:2.7"">
  <Header creationTime=""2024-05-06T07:08:09Z"" sender=""test"" instanceId=""1"" version=""2.7"" assetBufferSize=""10"" assetCount=""2"" />
  <Assets>
    <Pallet assetId=""P1"" deviceUuid=""dev"" timestamp=""2024-05-06T07:08:09Z""><Type>Machine</Type><Measurements><Weight units=""KILOGRAM"">12</Weight></Measurements></Pallet>
    <Fixture assetId=""X1"" deviceUuid=""dev"" timestamp=""2024-05-06T07:08:09Z""><FixtureId>fixture-1</FixtureId></Fixture>
  </Assets>
</MTConnectAssets>";

            var document = XmlAssetsResponseDocument.FromXml(Encoding.UTF8.GetBytes(xml));

            Assert.That(document?.Assets?.Select(o => o.Type), Is.EqualTo(new[] { PalletAsset.TypeId, FixtureAsset.TypeId }));
            var pallet = (IPalletAsset)document.Assets.First();
            Assert.That(pallet.PalletType, Is.EqualTo("Machine"));
            Assert.That(pallet.Measurements.Single().Value, Is.EqualTo(12));
            Assert.That(((IFixtureAsset)document.Assets.Last()).FixtureId, Is.EqualTo("fixture-1"));
        }
    }
}
