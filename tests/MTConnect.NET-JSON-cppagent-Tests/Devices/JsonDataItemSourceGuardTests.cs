// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.IO;
using System.Text.RegularExpressions;
using MTConnect.NET_JSON_cppagent_Tests.TestHelpers;
using NUnit.Framework;

namespace MTConnect.Tests.JsonCppagent.Devices
{
    /// <summary>
    /// Source-grep regression guard. Ensures the JSON `JsonDataItem(IDataItem)`
    /// constructor in either the cppagent or the base JSON formatter does not
    /// regress to an unconditional `Name = dataItem.Name;` copy.
    ///
    /// Source authority:
    /// - Reference shape: libraries/MTConnect.NET-XML/Devices/XmlDataItem.cs
    ///   guards the `name` write with `string.IsNullOrEmpty`.
    /// - XSD: https://schemas.mtconnect.org/schemas/MTConnectDevices_2.5.xsd —
    ///   `DataItem/@name` is `use="optional"`.
    /// - Public defect tracker: https://github.com/TrakHound/MTConnect.NET/issues/138.
    /// </summary>
    [TestFixture]
    public class JsonDataItemSourceGuardTests
    {
        private static readonly string[] WatchedSources =
        {
            "libraries/MTConnect.NET-JSON-cppagent/Devices/JsonDataItem.cs",
            "libraries/MTConnect.NET-JSON/Devices/JsonDataItem.cs"
        };

        [Test]
        public void JsonDataItem_constructors_must_not_copy_Name_unconditionally()
        {
            var repoRoot = RepoRootLocator.LocateRoot();
            var unguardedCopy = new Regex(@"(?m)^\s*Name\s*=\s*dataItem\.Name\s*;\s*$");

            foreach (var relativePath in WatchedSources)
            {
                var fullPath = Path.Combine(repoRoot, relativePath);
                Assert.That(File.Exists(fullPath), Is.True, $"Watched source not found: {fullPath}");

                var src = File.ReadAllText(fullPath);
                Assert.That(unguardedCopy.IsMatch(src), Is.False,
                    $"{relativePath}: Name copy must be guarded by string.IsNullOrEmpty " +
                    "to avoid emitting an empty 'name' attribute on Probe DataItems.");
            }
        }
    }
}
