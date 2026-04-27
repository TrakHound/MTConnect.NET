// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Linq;
using MTConnect.Tests.JsonCppagent.TestHelpers;
using NUnit.Framework;

namespace MTConnect.Tests.JsonCppagent
{
    /// <summary>
    /// Pins the shared (envelopeKind, version) compliance matrix used by both
    /// per-envelope and cross-envelope wire-shape fixtures. Centralizing the
    /// matrix prevents per-envelope tests from drifting away from the cross-
    /// envelope E2E set as new schema versions are added.
    ///
    /// Source authority:
    /// - Reference shape: cppagent v2.7.0.7 emits `Header.schemaVersion` and
    ///   `Header.testIndicator` on every envelope.
    /// - Public defect tracker:
    ///   https://github.com/TrakHound/MTConnect.NET/issues/130 (schemaVersion),
    ///   https://github.com/TrakHound/MTConnect.NET/issues/131 (testIndicator).
    /// </summary>
    [TestFixture]
    [Category("ComplianceMatrix")]
    public class JsonHeaderWireShapeMatrixTests
    {
        [Test]
        public void Matrix_exposes_three_envelope_kinds()
        {
            var kinds = JsonHeaderWireShapeMatrix.Cases
                .Select(c => c.Arguments[0]!.ToString())
                .Distinct()
                .OrderBy(s => s)
                .ToArray();

            Assert.That(kinds, Is.EqualTo(new[] { "Assets", "Devices", "Streams" }));
        }

        [Test]
        public void Matrix_covers_v20_v23_v25_for_each_envelope()
        {
            var expectedVersions = new[] { "2.0", "2.3", "2.5" };

            foreach (var kind in new[] { "Assets", "Devices", "Streams" })
            {
                var versions = JsonHeaderWireShapeMatrix.Cases
                    .Where(c => (string)c.Arguments[0]! == kind)
                    .Select(c => (string)c.Arguments[1]!)
                    .OrderBy(v => v)
                    .ToArray();

                Assert.That(versions, Is.EqualTo(expectedVersions),
                    $"{kind} envelope must include v2.0, v2.3, and v2.5 in the compliance matrix.");
            }
        }

        [Test]
        public void Matrix_class_is_internal_static()
        {
            var t = typeof(JsonHeaderWireShapeMatrix);
            Assert.That(t.IsAbstract && t.IsSealed, Is.True,
                "JsonHeaderWireShapeMatrix must be a static class.");
            Assert.That(t.IsNotPublic, Is.True,
                "JsonHeaderWireShapeMatrix must be internal to the test project.");
        }
    }
}
