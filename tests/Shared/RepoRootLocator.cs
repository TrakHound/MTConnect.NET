// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.IO;

namespace MTConnect.Tests.Shared
{
    /// <summary>
    /// Locates the repository root directory by walking up from the test
    /// assembly's bin folder until the <c>MTConnect.NET.sln</c> marker is
    /// found. Linked into individual test projects (via
    /// <c>&lt;Compile Include="..\Shared\RepoRootLocator.cs" /&gt;</c>) so
    /// every source-grep guard shares one walker rather than each rolling
    /// its own copy.
    /// </summary>
    internal static class RepoRootLocator
    {
        private const string SolutionMarker = "MTConnect.NET.sln";

        /// <summary>
        /// Walks up from <see cref="AppContext.BaseDirectory"/> until a
        /// directory containing <c>MTConnect.NET.sln</c> is found.
        /// </summary>
        /// <returns>The absolute path of the repository root.</returns>
        /// <exception cref="DirectoryNotFoundException">
        /// Thrown when no ancestor of the test bin folder contains the
        /// solution marker.
        /// </exception>
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
