// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MTConnect.NET_JSON_cppagent_Tests.TestHelpers
{
    /// <summary>
    /// Reflects every <c>public static readonly Version</c> field on
    /// <c>MTConnectVersions</c> so the parametric test cases stay in
    /// lock-step with the library's declared release matrix.
    /// </summary>
    internal static class VersionMatrix
    {
        public static IEnumerable All => Versions().Select(v => new TestCaseDataWrapper(v));

        public static IEnumerable<Version> Versions()
        {
            var versionsType = typeof(MTConnectVersions);
            var fields = versionsType.GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.FieldType == typeof(Version));

            foreach (var f in fields)
            {
                var value = (Version?)f.GetValue(null);
                if (value != null)
                {
                    yield return value;
                }
            }
        }
    }

    /// <summary>
    /// Wraps a <see cref="Version"/> in NUnit's <c>TestCaseData</c> so the
    /// failure / success message names the version.
    /// </summary>
    internal class TestCaseDataWrapper : NUnit.Framework.TestCaseData
    {
        public TestCaseDataWrapper(Version version) : base(version)
        {
            SetArgDisplayNames(version.ToString());
        }
    }
}
