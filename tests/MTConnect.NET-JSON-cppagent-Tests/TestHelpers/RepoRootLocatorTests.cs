// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.IO;
using NUnit.Framework;

namespace MTConnect.NET_JSON_cppagent_Tests.TestHelpers
{
    /// <summary>
    /// Pins the contract for the shared
    /// <see cref="RepoRootLocator"/> helper so source-grep guards
    /// share one walker rather than each rolling their own copy.
    /// </summary>
    [TestFixture]
    public class RepoRootLocatorTests
    {
        [Test]
        public void Helper_class_is_internal_static()
        {
            var t = typeof(RepoRootLocator);
            Assert.That(t.IsAbstract && t.IsSealed, Is.True,
                "RepoRootLocator must be a static class.");
            Assert.That(t.IsNotPublic, Is.True,
                "RepoRootLocator must be internal to the test project.");
        }

        [Test]
        public void Locate_returns_directory_containing_solution_file()
        {
            var root = RepoRootLocator.LocateRoot();

            Assert.That(Directory.Exists(root), Is.True, $"Repo root does not exist: {root}");
            Assert.That(File.Exists(Path.Combine(root, "MTConnect.NET.sln")), Is.True,
                "Returned repo root must contain MTConnect.NET.sln.");
        }
    }
}
