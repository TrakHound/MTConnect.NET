// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using NUnit.Framework;

namespace MTConnect.Tests.JsonCppagent.TestHelpers
{
    /// <summary>
    /// Shared compliance matrix yielding (envelopeKind, schemaVersion) tuples
    /// consumed by per-envelope and cross-envelope JSON-cppagent header
    /// wire-shape fixtures. Centralizing the matrix prevents per-envelope
    /// tests from drifting away from the cross-envelope E2E set as new
    /// schema versions are added.
    ///
    /// envelopeKind values: "Assets", "Streams", "Devices".
    /// schemaVersion values: "2.0", "2.3", "2.5".
    ///
    /// Source authority:
    /// - Reference shape: cppagent v2.7.0.7 emits `Header.schemaVersion` and
    ///   `Header.testIndicator` on every Streams / Devices / Assets envelope.
    /// - Public defect tracker:
    ///   https://github.com/TrakHound/MTConnect.NET/issues/130 (schemaVersion),
    ///   https://github.com/TrakHound/MTConnect.NET/issues/131 (testIndicator).
    /// </summary>
    internal static class JsonHeaderWireShapeMatrix
    {
        public static readonly string[] EnvelopeKinds =
        {
            "Streams",
            "Devices",
            "Assets",
        };

        public static readonly string[] SchemaVersions =
        {
            "2.0",
            "2.3",
            "2.5",
        };

        /// <summary>
        /// NUnit TestCaseSource-shaped enumeration of (envelopeKind, schemaVersion).
        /// Use as `[TestCaseSource(typeof(JsonHeaderWireShapeMatrix), nameof(Cases))]`.
        /// </summary>
        public static IEnumerable<TestCaseData> Cases
        {
            get
            {
                foreach (var kind in EnvelopeKinds)
                {
                    foreach (var version in SchemaVersions)
                    {
                        yield return new TestCaseData(kind, version)
                            .SetName($"{kind}_envelope_v{version}");
                    }
                }
            }
        }

        /// <summary>
        /// Per-envelope schema-version cases for fixtures scoped to a single
        /// envelope kind. Use as
        /// `[TestCaseSource(typeof(JsonHeaderWireShapeMatrix), nameof(SchemaVersions))]`.
        /// </summary>
        public static IEnumerable<TestCaseData> SchemaVersionCases
        {
            get
            {
                foreach (var version in SchemaVersions)
                {
                    yield return new TestCaseData(version).SetName($"v{version}");
                }
            }
        }
    }
}
