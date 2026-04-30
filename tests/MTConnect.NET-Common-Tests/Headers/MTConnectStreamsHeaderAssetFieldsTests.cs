// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Headers;
using NUnit.Framework;

namespace MTConnect.Tests.Common.Headers
{
    /// <summary>
    /// Pins the MTConnect Streams Header model-side support for the
    /// `assetBufferSize` and `assetCount` attributes that the cppagent
    /// JSON v2 reference printer emits on every Streams envelope's Header.
    ///
    /// The XSD `HeaderStreamsAttributesType` derives from
    /// `HeaderAttributesType`, which carries `assetBufferSize` and
    /// `assetCount` as inherited optional attributes. Both `IMTConnectDevicesHeader`
    /// and `IMTConnectAssetsHeader` exposed these fields previously; the
    /// Streams Header was the missing surface, surfacing as a wire-format
    /// gap on Streams envelopes (operators monitoring multi-agent fleets
    /// could not see asset-buffer utilisation on Current/Sample responses).
    ///
    /// Source authority:
    /// - XSD: https://schemas.mtconnect.org/schemas/MTConnectStreams_2.7.xsd
    ///   (HeaderStreamsAttributesType / HeaderAttributesType inheritance).
    /// - Reference shape: cppagent v2.7.0.7 emits both fields on every
    ///   Streams envelope Header (printer/json_printer.cpp).
    /// </summary>
    [TestFixture]
    public class MTConnectStreamsHeaderAssetFieldsTests
    {
        [Test]
        public void IMTConnectStreamsHeader_exposes_AssetBufferSize_getter()
        {
            var headerType = typeof(IMTConnectStreamsHeader);
            var property = headerType.GetProperty(nameof(IMTConnectStreamsHeader.AssetBufferSize));
            Assert.That(property, Is.Not.Null,
                "IMTConnectStreamsHeader must declare an AssetBufferSize property to match the cppagent v2 wire shape.");
            Assert.That(property!.PropertyType, Is.EqualTo(typeof(ulong)));
        }

        [Test]
        public void IMTConnectStreamsHeader_exposes_AssetCount_getter()
        {
            var headerType = typeof(IMTConnectStreamsHeader);
            var property = headerType.GetProperty(nameof(IMTConnectStreamsHeader.AssetCount));
            Assert.That(property, Is.Not.Null,
                "IMTConnectStreamsHeader must declare an AssetCount property to match the cppagent v2 wire shape.");
            Assert.That(property!.PropertyType, Is.EqualTo(typeof(ulong)));
        }

        [Test]
        public void MTConnectStreamsHeader_round_trips_AssetBufferSize_and_AssetCount()
        {
            var header = new MTConnectStreamsHeader
            {
                AssetBufferSize = 8192,
                AssetCount = 42
            };

            Assert.That(header.AssetBufferSize, Is.EqualTo(8192UL));
            Assert.That(header.AssetCount, Is.EqualTo(42UL));
        }

        [Test]
        public void MTConnectStreamsHeader_AssetBufferSize_default_is_zero()
        {
            var header = new MTConnectStreamsHeader();
            Assert.That(header.AssetBufferSize, Is.EqualTo(0UL),
                "Default AssetBufferSize must be 0 (no asset buffer configured).");
            Assert.That(header.AssetCount, Is.EqualTo(0UL),
                "Default AssetCount must be 0 (no assets registered).");
        }
    }
}
