// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Linq;
using MTConnect.Tests.Common.TestHelpers;
using NUnit.Framework;

namespace MTConnect.Tests.Common.TestHelpers
{
    /// <summary>
    /// Pins the contract for the shared
    /// <see cref="NameBackfillRemovalOutOfScope"/> helper so the
    /// out-of-scope set lives in one place and ctor-defaults regression
    /// fixtures cannot drift as the regen track lands the back-fill
    /// removal on the remaining `*.g.cs` classes.
    /// </summary>
    [TestFixture]
    public class NameBackfillRemovalOutOfScopeTests
    {
        [Test]
        public void Helper_class_is_internal_static()
        {
            var t = typeof(NameBackfillRemovalOutOfScope);
            Assert.That(t.IsAbstract && t.IsSealed, Is.True,
                "NameBackfillRemovalOutOfScope must be a static class.");
            Assert.That(t.IsNotPublic, Is.True,
                "NameBackfillRemovalOutOfScope must be internal to the test project.");
        }

        [Test]
        public void Helper_exposes_expected_out_of_scope_component_types()
        {
            var expected = new[]
            {
                "MTConnect.Devices.Components.CuttingTorchComponent",
                "MTConnect.Devices.Components.ElectrodeComponent",
                "MTConnect.Devices.Components.PinToolComponent",
                "MTConnect.Devices.Components.ToolHolderComponent",
            };

            var actual = NameBackfillRemovalOutOfScope.ComponentTypeNames
                .OrderBy(s => s, System.StringComparer.Ordinal)
                .ToArray();

            Assert.That(actual, Is.EqualTo(expected.OrderBy(s => s, System.StringComparer.Ordinal).ToArray()));
        }

        [Test]
        public void Helper_set_uses_ordinal_comparer()
        {
            // Case-insensitive comparison would mask `cuttingtorchcomponent`
            // accidentally landing as a different generated type — pin
            // ordinal so consumers cannot regress to a looser comparer.
            Assert.That(
                NameBackfillRemovalOutOfScope.ComponentTypeNames.Contains("MTConnect.Devices.Components.CuttingTorchComponent"),
                Is.True);
            Assert.That(
                NameBackfillRemovalOutOfScope.ComponentTypeNames.Contains("MTConnect.Devices.Components.cuttingtorchcomponent"),
                Is.False,
                "Out-of-scope set must use ordinal (case-sensitive) comparison.");
        }
    }
}
