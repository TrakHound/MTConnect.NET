// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.IO;

namespace MTConnect.Tests.SysMLImport.TestHelpers
{
    /// <summary>
    /// Locates the repository root by walking up from the test bin directory
    /// until <c>MTConnect.NET.sln</c> is found. Used by tests that read
    /// Scriban templates, source files, or generated <c>.g.cs</c> outputs
    /// from the source tree.
    /// </summary>
    internal static class RepoRootLocator
    {
        private const string SolutionMarker = "MTConnect.NET.sln";

        /// <summary>Walks up from the test assembly's <see cref="AppContext.BaseDirectory"/>
        /// until a directory containing <c>MTConnect.NET.sln</c> is found; throws
        /// <see cref="DirectoryNotFoundException"/> otherwise.</summary>
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
