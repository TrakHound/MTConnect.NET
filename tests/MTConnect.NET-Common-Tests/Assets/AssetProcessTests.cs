// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using MTConnect.Assets.Files;
using NUnit.Framework;

namespace MTConnect.Tests.Common.Assets
{
    /// <summary>
    /// Pins <see cref="MTConnect.Assets.Asset.Process"/>: it adjusts a copy (the stored asset is never
    /// changed by a request for some version) and keeps the Hash only for 2.2 through 2.5, the versions
    /// that define it as an Asset attribute.
    /// </summary>
    [TestFixture]
    [Category("AssetProcess")]
    public class AssetProcessTests
    {
        private static FileAsset CreateFile()
        {
            var asset = new FileAsset { AssetId = "F1", Size = 1, VersionId = "1", Hash = "h", Description = "d" };
            return asset;
        }

        [Test]
        public void Process_does_not_change_the_stored_asset()
        {
            var asset = CreateFile();

            var processed = asset.Process(MTConnectVersions.Version20);

            Assert.That(processed, Is.Not.SameAs(asset));
            Assert.That(processed.Hash, Is.Null);
            Assert.That(asset.Hash, Is.EqualTo("h"), "A 2.0 request erased the stored hash");
            Assert.That(asset.Process(MTConnectVersions.Version25).Hash, Is.EqualTo("h"));
        }

        [TestCase("2.1", false)]
        [TestCase("2.2", true)]
        [TestCase("2.5", true)]
        [TestCase("2.6", false)]
        [TestCase("2.7", false)]
        public void Process_keeps_the_hash_only_where_the_version_defines_it(string version, bool kept)
        {
            var processed = CreateFile().Process(System.Version.Parse(version));

            Assert.That(processed.Hash, kept ? Is.EqualTo("h") : Is.Null);
            Assert.That(processed.Description, Is.EqualTo("d"));
        }

        [Test]
        public void Process_keeps_the_hash_on_a_cutting_tool_copy()
        {
            var tool = new CuttingToolAsset { AssetId = "T1", Hash = "h" };

            Assert.That(tool.Process(MTConnectVersions.Version25).Hash, Is.EqualTo("h"));
            Assert.That(tool.Process(MTConnectVersions.Version27).Hash, Is.Null);
            Assert.That(tool.Hash, Is.EqualTo("h"));
        }
    }
}
