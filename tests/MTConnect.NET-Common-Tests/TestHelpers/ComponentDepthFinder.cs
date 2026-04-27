// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using MTConnect.Devices;
using MTConnect.Interfaces;

namespace MTConnect.Tests.Common.TestHelpers
{
    /// <summary>
    /// Walks a Device component tree depth-first and returns the depth
    /// (1-based) of the first <see cref="IComponent"/> whose
    /// <c>Type</c> equals <paramref name="targetTypeId"/>. Returns
    /// <c>-1</c> when the target is not present in the subtree.
    ///
    /// Tests that need to assert tree-depth invariants for the auto-wrap
    /// behavior in <see cref="Device.AddComponent(IComponent)"/> share
    /// this helper so the walk-up logic stays in one place.
    /// </summary>
    public static class ComponentDepthFinder
    {
        /// <summary>
        /// Returns the 1-based depth of the first component whose
        /// <c>Type</c> matches <paramref name="targetTypeId"/> in
        /// <paramref name="device"/>'s subtree. The Device's direct
        /// children are at depth 1, their children at depth 2, etc.
        /// </summary>
        public static int MeasureDepth(Device device, string targetTypeId)
        {
            if (device == null) return -1;
            return MeasureDepth(device.Components, targetTypeId, 1);
        }

        /// <summary>
        /// Returns the 1-based depth of the first component whose
        /// <c>Type</c> matches <paramref name="targetTypeId"/>, walking
        /// the supplied <paramref name="components"/> sequence and
        /// counting from <paramref name="currentDepth"/>.
        /// </summary>
        public static int MeasureDepth(
            IEnumerable<IComponent> components, string targetTypeId, int currentDepth)
        {
            if (components == null) return -1;
            foreach (var c in components)
            {
                if (c == null) continue;
                if (c.Type == targetTypeId) return currentDepth;
                var nested = MeasureDepth(c.Components, targetTypeId, currentDepth + 1);
                if (nested > 0) return nested;
            }
            return -1;
        }
    }
}
