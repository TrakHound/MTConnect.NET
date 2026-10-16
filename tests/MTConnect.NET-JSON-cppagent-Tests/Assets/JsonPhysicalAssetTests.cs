// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using MTConnect.Assets;
using MTConnect.Assets.Fixture;
using MTConnect.Assets.Pallet;
using MTConnect.Formatters;
using MTConnect.Headers;
using NUnit.Framework;

namespace MTConnect.Tests.JsonCppagent.Assets
{
    /// <summary>
    /// Pins Pallet and Fixture in the cppagent JSON v2 format. Following cppagent's
    /// <c>pallet.cpp</c> / <c>fixture.cpp</c> / <c>physical_asset.cpp</c> factories, child elements
    /// are PascalCase (<c>Type</c>, <c>PalletId</c>, <c>CalibrationDate</c>, ...) and
    /// <c>Measurements</c> groups the measurements by type name
    /// (<c>{"Weight": [{"value": ...}]}</c>), as for cutting-tool measurements.
    /// Source: https://github.com/mtconnect/cppagent/tree/main/src/mtconnect/asset
    /// </summary>
    [TestFixture]
    [Category("JsonPhysicalAsset")]
    public class JsonPhysicalAssetTests
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
                CalibrationDate = Calibrated
            };
            pallet.Measurements = new IMeasurement[]
            {
                new LoadedHeightMeasurement(410.5),
                new WeightMeasurement(12.25) { Units = "KILOGRAM", Nominal = 12, SignificantDigits = 2 },
                new WeightMeasurement(13)
            };
            return pallet;
        }

        private static string ReadAll(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            return new StreamReader(stream).ReadToEnd();
        }


        [Test]
        public void Pallet_is_written_in_the_cppagent_shape()
        {
            var result = new JsonHttpEntityFormatter().Format(CreatePallet());
            Assert.That(result.Success, Is.True);

            using (var document = JsonDocument.Parse(ReadAll(result.Content)))
            {
                var pallet = document.RootElement.GetProperty("Pallet")[0];
                Assert.That(pallet.GetProperty("assetId").GetString(), Is.EqualTo("P1"));
                Assert.That(pallet.GetProperty("Type").GetString(), Is.EqualTo("Machine"));
                Assert.That(pallet.GetProperty("PalletId").GetString(), Is.EqualTo("pallet-1"));
                Assert.That(pallet.GetProperty("PalletNumber").GetInt32(), Is.EqualTo(4));
                Assert.That(pallet.TryGetProperty("ManufactureDate", out _), Is.False, "Unset dates are left out");

                var measurements = pallet.GetProperty("Measurements");
                Assert.That(measurements.EnumerateObject().Select(o => o.Name), Is.EqualTo(new[] { "Weight", "LoadedHeight" }), "Grouped by type, in schema order");
                Assert.That(measurements.GetProperty("Weight").GetArrayLength(), Is.EqualTo(2));
                Assert.That(measurements.GetProperty("Weight")[0].GetProperty("value").GetDouble(), Is.EqualTo(12.25));
                Assert.That(measurements.GetProperty("Weight")[0].GetProperty("units").GetString(), Is.EqualTo("KILOGRAM"));
            }
        }

        [Test]
        public void Pallet_is_read_from_cppagent_json()
        {
            var json = @"{""assetId"":""P1"",""deviceUuid"":""dev"",""timestamp"":""2024-05-06T07:08:09Z"",""Type"":""Machine"",""PalletId"":""pallet-1"",""PalletNumber"":4,
                ""ClampingMethod"":""HYDRAULIC"",""MountingMethod"":""BOLTED"",""CalibrationDate"":""2024-01-02T03:04:05Z"",
                ""Measurements"":{""Weight"":[{""value"":12.25,""units"":""KILOGRAM"",""nominal"":12,""maximum"":15,""minimum"":10,""significantDigits"":2,""nativeUnits"":""POUND""}],""LoadedHeight"":[{""value"":410.5}],""Depth"":[{""value"":5}]}}";

            var result = new JsonHttpEntityFormatter().CreateAsset(PalletAsset.TypeId, new MemoryStream(Encoding.UTF8.GetBytes(json)));

            Assert.That(result.Success, Is.True, string.Join("; ", result.Errors ?? Enumerable.Empty<string>()));
            var pallet = (PalletAsset)result.Content;
            Assert.That(pallet.Type, Is.EqualTo(PalletAsset.TypeId));
            Assert.That(pallet.PalletType, Is.EqualTo("Machine"));
            Assert.That(pallet.PalletId, Is.EqualTo("pallet-1"));
            Assert.That(pallet.PalletNumber, Is.EqualTo(4));
            Assert.That(pallet.ClampingMethod, Is.EqualTo("HYDRAULIC"));
            Assert.That(pallet.MountingMethod, Is.EqualTo("BOLTED"));
            Assert.That(pallet.CalibrationDate.ToUniversalTime(), Is.EqualTo(Calibrated));

            var measurements = pallet.Measurements.ToList();
            Assert.That(measurements.Select(o => Measurement.GetTypeName(o)), Is.EqualTo(new[] { "Weight", "LoadedHeight", "Depth" }));
            Assert.That(measurements[0], Is.InstanceOf<WeightMeasurement>());
            Assert.That(measurements[0].Value, Is.EqualTo(12.25));
            Assert.That(measurements[0].Units, Is.EqualTo("KILOGRAM"));
            Assert.That(measurements[0].NativeUnits, Is.EqualTo("POUND"));
            Assert.That(measurements[0].Nominal, Is.EqualTo(12));
            Assert.That(measurements[0].Maximum, Is.EqualTo(15));
            Assert.That(measurements[0].Minimum, Is.EqualTo(10));
            Assert.That(measurements[0].SignificantDigits, Is.EqualTo(2));
            Assert.That(measurements[1], Is.InstanceOf<LoadedHeightMeasurement>());
        }

        [Test]
        public void Fixture_is_read_from_cppagent_json()
        {
            var json = @"{""assetId"":""X1"",""deviceUuid"":""dev"",""timestamp"":""2024-05-06T07:08:09Z"",""FixtureId"":""fixture-1"",""FixtureNumber"":2,
                ""ClampingMethod"":""MANUAL"",""MountingMethod"":""MAGNETIC"",""NextInspectionDate"":""2024-01-02T03:04:05Z"",""Measurements"":{""Swing"":[{""value"":300}]}}";

            var result = new JsonHttpEntityFormatter().CreateAsset(FixtureAsset.TypeId, new MemoryStream(Encoding.UTF8.GetBytes(json)));

            Assert.That(result.Success, Is.True, string.Join("; ", result.Errors ?? Enumerable.Empty<string>()));
            var fixture = (FixtureAsset)result.Content;
            Assert.That(fixture.Type, Is.EqualTo(FixtureAsset.TypeId));
            Assert.That(fixture.FixtureId, Is.EqualTo("fixture-1"));
            Assert.That(fixture.FixtureNumber, Is.EqualTo(2));
            Assert.That(fixture.ClampingMethod, Is.EqualTo("MANUAL"));
            Assert.That(fixture.MountingMethod, Is.EqualTo("MAGNETIC"));
            Assert.That(fixture.NextInspectionDate.ToUniversalTime(), Is.EqualTo(Calibrated));
            Assert.That(fixture.Measurements.Single(), Is.InstanceOf<SwingMeasurement>());
        }

        [Test]
        public void Assets_document_round_trips_pallets_and_fixtures()
        {
            var document = new AssetsResponseDocument
            {
                Header = new MTConnectAssetsHeader { CreationTime = Timestamp, Sender = "test", Version = "2.7", InstanceId = 1 },
                Version = MTConnectVersions.Version27,
                Assets = new IAsset[] { CreatePallet(), new FixtureAsset { AssetId = "X1", DeviceUuid = "dev", Timestamp = Timestamp, FixtureId = "fixture-1" } }
            };

            var formatter = new JsonHttpResponseDocumentFormatter();
            var written = formatter.Format(document);
            Assert.That(written.Success, Is.True);
            written.Content.Seek(0, SeekOrigin.Begin);

            var read = formatter.CreateAssetsResponseDocument(written.Content);

            Assert.That(read.Success, Is.True);
            var assets = read.Content.Assets.ToList();
            Assert.That(assets.Select(o => o.Type), Is.EquivalentTo(new[] { PalletAsset.TypeId, FixtureAsset.TypeId }));

            var pallet = (IPalletAsset)assets.Single(o => o.Type == PalletAsset.TypeId);
            Assert.That(pallet.PalletType, Is.EqualTo("Machine"));
            Assert.That(pallet.Measurements.Count(), Is.EqualTo(3));
            Assert.That(((IFixtureAsset)assets.Single(o => o.Type == FixtureAsset.TypeId)).FixtureId, Is.EqualTo("fixture-1"));
        }
    }
}
