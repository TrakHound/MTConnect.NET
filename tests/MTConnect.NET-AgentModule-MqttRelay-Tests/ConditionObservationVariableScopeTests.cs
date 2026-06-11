// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace MTConnect.AgentModule.MqttRelay.Tests
{
    // Pins the rename PR #194 lands on Module.cs to resolve the
    // CS0136 shadowed-local diagnostic that fires under net4x. The
    // original code in `AgentObservationAdded` declared an inner
    // `var observation` inside a `foreach` loop that lived in the
    // same scope as the handler's outer `observation` parameter:
    //
    //     private async void AgentObservationAdded(object sender,
    //         IObservation observation)            // <- outer name
    //     {
    //         ...
    //         foreach (var observation in conditionObservations)  // <- shadow
    //         { ... }
    //     }
    //
    // The C# 5+ language spec section 7.6.2.1 forbids a local
    // variable declaration from shadowing an enclosing
    // parameter/local of the same name. The .NET 5+ compiler emits
    // a warning that TreatWarningsAsErrors escalates to an error on
    // net8.0, but the project's net4x roslyn pinned compiler emits
    // CS0136 directly. PR #194 renames the inner declaration to
    // `condObservation` (or similar non-shadowing name) so the
    // multi-TFM build path stays green.
    //
    // This fixture reads the Module.cs source via reflection on the
    // assembly's location and scans the relevant block, asserting
    // that no `foreach (var observation` declaration remains inside
    // the handler. A future contributor who re-introduces the
    // shadow fails this test under net8.0 — the test TFM that the
    // CI matrix exercises — instead of waiting for the next Release
    // pack to surface the failure under net4x.
    /// <summary>Pins the rename that resolves the net4x CS0136 shadow.</summary>
    [TestFixture]
    [Category("MultiTfmCompat")]
    public class ConditionObservationVariableScopeTests
    {
        /// <summary>Module.cs AgentObservationAdded must not declare an inner `observation` loop variable.</summary>
        [Test]
        public void AgentObservationAdded_does_not_declare_inner_observation_loop_variable()
        {
            // Locate the Module.cs source by walking up from the test
            // assembly to the repo root, then descending into the
            // module's source tree. The path is stable in-tree;
            // CI runs with the same layout.
            var moduleSourcePath = FindModuleSourcePath();
            Assert.That(moduleSourcePath, Is.Not.Null,
                "Could not locate Module.cs for MTConnect.NET-AgentModule-MqttRelay.");

            var source = File.ReadAllText(moduleSourcePath!);

            // Extract the AgentObservationAdded handler body. The
            // method ends at the matching close-brace of the
            // `try { ... } finally { ... }` pair, which is the last
            // brace at column 8 before the next `private` member.
            var startIndex = source.IndexOf(
                "private async void AgentObservationAdded",
                StringComparison.Ordinal);
            Assert.That(startIndex, Is.GreaterThanOrEqualTo(0),
                "AgentObservationAdded handler not found in Module.cs.");

            // Find the start of the next private member after the
            // handler so we scope the scan to AgentObservationAdded's
            // body and don't reach into AgentAssetAdded.
            var nextMemberIndex = source.IndexOf(
                "private async void AgentAssetAdded",
                startIndex, StringComparison.Ordinal);
            Assert.That(nextMemberIndex, Is.GreaterThan(startIndex),
                "Next handler AgentAssetAdded not found after AgentObservationAdded; " +
                "Module.cs structure has changed.");

            var handlerBody = source.Substring(startIndex, nextMemberIndex - startIndex);

            // Assert: the handler must NOT contain the shadowing
            // declaration `foreach (var observation in`. The renamed
            // form uses a different identifier such as
            // `condObservation`.
            var shadowingPatterns = new[]
            {
                "foreach (var observation in",
                "foreach(var observation in",
            };
            foreach (var pattern in shadowingPatterns)
            {
                Assert.That(
                    handlerBody.Contains(pattern, StringComparison.Ordinal),
                    Is.False,
                    $"AgentObservationAdded contains `{pattern}` which shadows " +
                    "the outer `observation` parameter and fails CS0136 under net4x. " +
                    "Rename the inner loop variable.");
            }
        }

        private static string FindModuleSourcePath()
        {
            // The test assembly's location lives under
            //   tests/MTConnect.NET-AgentModule-MqttRelay-Tests/bin/...
            // walk up to find the repo root marker, then descend.
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir != null)
            {
                var candidate = Path.Combine(
                    dir.FullName,
                    "agent", "Modules", "MTConnect.NET-AgentModule-MqttRelay", "Module.cs");
                if (File.Exists(candidate)) return candidate;
                dir = dir.Parent;
            }
            return null;
        }
    }
}
