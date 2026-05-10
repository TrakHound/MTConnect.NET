// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.IO;

namespace MTConnect.Tests.XML.TestHelpers
{
    /// <summary>
    /// Locates the repository root by walking up from the test bin directory
    /// until <c>MTConnect.NET.sln</c> is found. Used by tests that need to
    /// load XSDs or other source-tree resources by path.
    /// </summary>
    internal static class RepoRootLocator
    {
        private const string SolutionMarker = "MTConnect.NET.sln";

        public static string LocateRoot()
        {
            var current = new DirectoryInfo(AppContext.BaseDirectory);

            while (current != null)
            {
                if (File.Exists(Path.Combine(current.FullName, SolutionMarker)))
                {
                    return current.FullName;
                }

                current = current.Parent;
            }

            throw new DirectoryNotFoundException(
                $"Could not locate '{SolutionMarker}' walking up from " +
                $"'{AppContext.BaseDirectory}'.");
        }
    }
}
