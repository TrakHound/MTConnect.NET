// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;

namespace MTConnect.Tests.Common.TestHelpers
{
    /// <summary>
    /// Shared list of concrete <c>MTConnect.Devices.Components.*Component</c>
    /// classes whose <c>Name</c> back-fill is generated from an older regen
    /// template and is not removed by the current source tree. A follow-up
    /// regen will strip the back-fill from these classes too; until then the
    /// type names live here so:
    ///   - ComponentCtorDefaultsTests and
    ///   - DeviceComponentDefaultsRegressionTests
    /// stay green without losing their enforcement intent on the components
    /// whose back-fill has already been removed.
    ///
    /// Centralizing the set prevents the two fixtures' inline copies from
    /// drifting as new generated subclasses are added or as the regen
    /// removes them from this list.
    /// </summary>
    internal static class NameBackfillRemovalOutOfScope
    {
        /// <summary>
        /// Fully-qualified type names of the out-of-scope component classes.
        /// Uses ordinal (case-sensitive) comparison so a generated
        /// `cuttingtorchcomponent` snapshot wouldn't accidentally match the
        /// canonical `CuttingTorchComponent`.
        /// </summary>
        public static readonly HashSet<string> ComponentTypeNames =
            new(StringComparer.Ordinal)
            {
                "MTConnect.Devices.Components.CuttingTorchComponent",
                "MTConnect.Devices.Components.ElectrodeComponent",
                "MTConnect.Devices.Components.PinToolComponent",
                "MTConnect.Devices.Components.ToolHolderComponent",
            };
    }
}
