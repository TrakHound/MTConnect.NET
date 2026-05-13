// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.IO;
using NUnit.Framework;

namespace MTConnect.Tests.Common.Headers
{
    /// <summary>
    /// Pins the SchemaVersion XML-doc example string on the six Header
    /// interfaces and classes. The example must reference the current
    /// MTConnect Standard release ("2.7") rather than the stale "2.5"
    /// snapshot, so consumers reading the IntelliSense don't think
    /// the agent only supports an older revision.
    ///
    /// Files covered (line 27 in each):
    /// - libraries/MTConnect.NET-Common/Headers/IMTConnectAssestsHeader.cs
    /// - libraries/MTConnect.NET-Common/Headers/IMTConnectDevicesHeader.cs
    /// - libraries/MTConnect.NET-Common/Headers/IMTConnectStreamsHeader.cs
    /// - libraries/MTConnect.NET-Common/Headers/MTConnectAssestsHeader.cs
    /// - libraries/MTConnect.NET-Common/Headers/MTConnectDevicesHeader.cs
    /// - libraries/MTConnect.NET-Common/Headers/MTConnectStreamsHeader.cs
    /// </summary>
    [TestFixture]
    public class HeaderXmlDocExampleVersionTests
    {
        private static readonly string[] HeaderFileRelativePaths =
        {
            "libraries/MTConnect.NET-Common/Headers/IMTConnectAssestsHeader.cs",
            "libraries/MTConnect.NET-Common/Headers/IMTConnectDevicesHeader.cs",
            "libraries/MTConnect.NET-Common/Headers/IMTConnectStreamsHeader.cs",
            "libraries/MTConnect.NET-Common/Headers/MTConnectAssestsHeader.cs",
            "libraries/MTConnect.NET-Common/Headers/MTConnectDevicesHeader.cs",
            "libraries/MTConnect.NET-Common/Headers/MTConnectStreamsHeader.cs",
        };

        private static string FindRepoRoot()
        {
            var dir = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);
            while (dir != null)
            {
                if (Directory.Exists(Path.Combine(dir.FullName, "libraries")) &&
                    Directory.Exists(Path.Combine(dir.FullName, "tests")))
                {
                    return dir.FullName;
                }
                dir = dir.Parent;
            }
            Assert.Fail("Could not locate repo root from test working directory.");
            return string.Empty;
        }

        [TestCaseSource(nameof(HeaderFileRelativePaths))]
        public void Header_xmldoc_example_does_not_reference_stale_version_2_5(string relativePath)
        {
            var fullPath = Path.Combine(FindRepoRoot(), relativePath);
            Assert.That(File.Exists(fullPath), Is.True, $"Header source file not found at {fullPath}.");

            var text = File.ReadAllText(fullPath);

            Assert.That(text.Contains("(for example \"2.5\")"), Is.False,
                $"{relativePath} must not pin its SchemaVersion XML-doc example to the stale \"2.5\" string.");
        }
    }
}
