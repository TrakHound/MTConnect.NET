// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using MTConnect.Assets;
using MTConnect.Assets.Fixture;
using MTConnect.Assets.Pallet;
using MTConnect.Formatters;
using MTConnect.Headers;
using NUnit.Framework;

namespace MTConnect.Tests.Json.Assets
{
    /// <summary>
    /// Pins Pallet and Fixture in the MTConnect.NET JSON format: entity round trips of every property
    /// and measurement, the pallet's own type as <c>palletType</c> (<c>type</c> stays the asset type),
    /// unset dates and numbers left out, and both included in an Assets document.
    /// </summary>
    [TestFixture]
    [Category("JsonPhysicalAsset")]
    public class JsonPhysicalAssetTests
    {
        private static readonly DateTime Timestamp = new DateTime(2024, 5, 6, 7, 8, 9, DateTimeKind.Utc);
        private static readonly DateTime Calibrated = new DateTime(2024, 1, 2, 3, 4, 5, DateTimeKind.Utc);

        private readonly IEntityFormatter _formatter = new MTConnect.Formatters.JsonEntityFormatter();


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
                new WeightMeasurement(12.25) { Units = "KILOGRAM", NativeUnits = "POUND", Maximum = 15, Minimum = 10, Nominal = 12, SignificantDigits = 2 },
                new LoadedHeightMeasurement(410.5)
            };
            return pallet;
        }

        private string Format(IAsset asset)
        {
            var result = _formatter.Format(asset);
            Assert.That(result.Success, Is.True);
            result.Content.Seek(0, SeekOrigin.Begin);
            return new StreamReader(result.Content).ReadToEnd();
        }

        private IAsset Read(string type, string json)
        {
            var result = _formatter.CreateAsset(type, new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json)));
            Assert.That(result.Success, Is.True, string.Join("; ", result.Errors ?? Enumerable.Empty<string>()));
            return result.Content;
        }


        /// <summary>
        /// Pins the behaviour expressed by the test name: pallet round trips.
        /// </summary>
        [Test]
        public void Pallet_round_trips()
        {
            var json = Format(CreatePallet());

            var read = (PalletAsset)Read(PalletAsset.TypeId, json);

            Assert.That(read, Is.Not.Null, json);
            Assert.That(read.Type, Is.EqualTo(PalletAsset.TypeId));
            Assert.That(read.AssetId, Is.EqualTo("P1"));
            Assert.That(read.PalletType, Is.EqualTo("Machine"));
            Assert.That(read.PalletId, Is.EqualTo("pallet-1"));
            Assert.That(read.PalletNumber, Is.EqualTo(4));
            Assert.That(read.ClampingMethod, Is.EqualTo("HYDRAULIC"));
            Assert.That(read.MountingMethod, Is.EqualTo("BOLTED"));
            Assert.That(read.CalibrationDate.ToUniversalTime(), Is.EqualTo(Calibrated));
            Assert.That(read.ManufactureDate, Is.EqualTo(default(DateTime)));
            Assert.That(read.Description, Is.EqualTo("Pallet description"));

            var measurements = read.Measurements.ToList();
            Assert.That(measurements.Select(o => o.GetType()), Is.EqualTo(new[] { typeof(WeightMeasurement), typeof(LoadedHeightMeasurement) }));
            var weight = measurements[0];
            Assert.That(weight.Value, Is.EqualTo(12.25));
            Assert.That(weight.Units, Is.EqualTo("KILOGRAM"));
            Assert.That(weight.NativeUnits, Is.EqualTo("POUND"));
            Assert.That(weight.Maximum, Is.EqualTo(15));
            Assert.That(weight.Minimum, Is.EqualTo(10));
            Assert.That(weight.Nominal, Is.EqualTo(12));
            Assert.That(weight.SignificantDigits, Is.EqualTo(2));
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: pallet wire shape keeps type for the asset type.
        /// </summary>
        [Test]
        public void Pallet_wire_shape_keeps_type_for_the_asset_type()
        {
            using (var document = JsonDocument.Parse(Format(CreatePallet())))
            {
                var root = document.RootElement;
                Assert.That(root.GetProperty("type").GetString(), Is.EqualTo("Pallet"));
                Assert.That(root.GetProperty("palletType").GetString(), Is.EqualTo("Machine"));
                Assert.That(root.GetProperty("measurements")[0].GetProperty("type").GetString(), Is.EqualTo("Weight"));
                Assert.That(root.TryGetProperty("manufactureDate", out _), Is.False, "Unset dates are left out");
            }
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: fixture round trips.
        /// </summary>
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

            var read = (FixtureAsset)Read(FixtureAsset.TypeId, Format(fixture));

            Assert.That(read, Is.Not.Null);
            Assert.That(read.Type, Is.EqualTo(FixtureAsset.TypeId));
            Assert.That(read.FixtureId, Is.EqualTo("fixture-1"));
            Assert.That(read.FixtureNumber, Is.EqualTo(2));
            Assert.That(read.ClampingMethod, Is.EqualTo("MANUAL"));
            Assert.That(read.MountingMethod, Is.EqualTo("MAGNETIC"));
            Assert.That(read.NextInspectionDate.ToUniversalTime(), Is.EqualTo(Calibrated));
            Assert.That(read.Measurements.Single(), Is.InstanceOf<SwingMeasurement>());
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: assets document includes pallets and fixtures.
        /// </summary>
        [Test]
        public void Assets_document_includes_pallets_and_fixtures()
        {
            var document = new AssetsResponseDocument
            {
                Header = new MTConnectAssetsHeader { CreationTime = Timestamp, Sender = "test", Version = "2.7" },
                Version = MTConnectVersions.Version27,
                Assets = new IAsset[] { CreatePallet(), new FixtureAsset { AssetId = "X1", FixtureId = "fixture-1" } }
            };

            var result = new MTConnect.Formatters.JsonResponseDocumentFormatter().Format(document);
            Assert.That(result.Success, Is.True);
            result.Content.Seek(0, SeekOrigin.Begin);

            using (var json = JsonDocument.Parse(new StreamReader(result.Content).ReadToEnd()))
            {
                var types = json.RootElement.GetProperty("assets").EnumerateArray().Select(o => o.GetProperty("type").GetString());
                Assert.That(types, Is.EqualTo(new[] { "Pallet", "Fixture" }));
            }
        }
    }
}
