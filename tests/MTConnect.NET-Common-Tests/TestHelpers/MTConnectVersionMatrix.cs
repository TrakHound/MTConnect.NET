// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MTConnect.Tests.Common.TestHelpers
{
    /// <summary>
    /// Discovers every public static <see cref="System.Version"/> field on
    /// <see cref="MTConnect.MTConnectVersions"/> via reflection. Adding a
    /// new constant on the production type automatically extends the
    /// parametric matrix without per-test edits.
    /// </summary>
    public static class MTConnectVersionMatrix
    {
        /// <summary>
        /// All MTConnect Standard release constants exposed by the library.
        /// </summary>
        public static IEnumerable<Version> All => typeof(MTConnect.MTConnectVersions)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(f => f.FieldType == typeof(Version))
            .Select(f => (Version)f.GetValue(null)!)
            .ToArray();
    }
}
