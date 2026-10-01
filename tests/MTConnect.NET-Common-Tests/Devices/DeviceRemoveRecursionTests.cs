// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MTConnect.Devices;
using MTConnect.Devices.DataItems;
using NUnit.Framework;

namespace MTConnect.NET_Common_Tests.Devices
{
    /// <summary>
    /// Direct unit coverage FLOOR for <see cref="Device.RemoveComposition(string)"/>
    /// and <see cref="Device.RemoveDataItem(string)"/> — the two overrides
    /// PR #219 commit be42f52b made recursive / top-level-aware to close
    /// F-TEST-BUG-1 (nested Composition unremovable) and F-TEST-BUG-2
    /// (top-level Device DataItem unremovable). The existing
    /// <c>DeviceValidationLevelEnumArmTests</c> exercises the fix through
    /// <c>MTConnect.Agents.MTConnectAgent.NormalizeDevice</c>; this
    /// fixture pins the SAME methods against the coverage-FLOOR shapes
    /// the audit brief for cycle-2 explicitly listed — top-level
    /// Device.Compositions collection, great-grandchild Component depth,
    /// idempotent no-op on missing ID, and empty-tree safety.
    ///
    /// A regression that reverts either method to its pre-fix shape
    /// (skipping the top-level collection, or dropping the recursion)
    /// fails these tests before the higher-level NormalizeDevice fixture
    /// can flag it — smaller failing surface, faster diagnosis.
    /// </summary>
    [TestFixture]
    [Category("DeviceRemoveRecursion")]
    public class DeviceRemoveRecursionTests
    {
        // --------------------------------------------------------------
        // RemoveComposition — every code path in the recursive override.
        // --------------------------------------------------------------

        /// <summary>
        /// Pins that a Composition placed directly on <c>Device.Compositions</c>
        /// (the collection the base <see cref="Component"/> exposes on every
        /// Device / Component / Composition holder) is dropped by
        /// <see cref="Device.RemoveComposition"/>. The recursive override
        /// added by be42f52b handles this via the leading top-level branch
        /// before descending; that branch would silently skip if the
        /// implementation ever regressed to component-only recursion.
        /// </summary>
        [Test]
        public void RemoveComposition_drops_top_level_Composition_directly_on_Device()
        {
            var device = new Device { Id = "d1", Uuid = "d1", Name = "d1", Type = Device.TypeId };
            device.AddComposition(new Composition { Id = "top-comp", Name = "top-comp", Type = "Generic" });

            Assert.That(device.Compositions.Any(c => c.Id == "top-comp"), Is.True,
                "precondition — top-level Composition must be present before removal.");

            device.RemoveComposition("top-comp");

            Assert.That(device.Compositions == null || !device.Compositions.Any(c => c.Id == "top-comp"), Is.True,
                "Device.RemoveComposition must drop a Composition placed directly on Device.Compositions.");
        }

        /// <summary>
        /// Pins great-grandchild-depth Composition removal:
        /// Device → Component → subComponent → subsubComponent → Composition.
        /// The recursive <c>RemoveComposition(IComponent, string)</c> helper
        /// descends via <c>component.Components</c>; a regression to a
        /// single-level walk would fail this at depth 3.
        /// </summary>
        [Test]
        public void RemoveComposition_recurses_into_great_grandchild_Component()
        {
            var device = new Device { Id = "d1", Uuid = "d1", Name = "d1", Type = Device.TypeId };

            var l1 = new Component { Id = "l1", Name = "l1", Type = "Axes" };
            var l2 = new Component { Id = "l2", Name = "l2", Type = "Linear" };
            var l3 = new Component { Id = "l3", Name = "l3", Type = "Motor" };
            l3.AddComposition(new Composition { Id = "deep-comp", Name = "deep-comp", Type = "Generic" });
            l2.AddComponent(l3);
            l1.AddComponent(l2);
            device.AddComponent(l1);

            Assert.That(device.GetCompositions().Any(c => c.Id == "deep-comp"), Is.True,
                "precondition — great-grandchild Composition must be reachable via GetCompositions before removal.");

            device.RemoveComposition("deep-comp");

            Assert.That(device.GetCompositions() == null || !device.GetCompositions().Any(c => c.Id == "deep-comp"), Is.True,
                "Device.RemoveComposition must recurse through Component→subComponent→subsubComponent to drop a deeply nested Composition — matches the shape of RemoveComponent recursion the fix mirrors.");
        }

        /// <summary>
        /// Pins idempotency: calling <see cref="Device.RemoveComposition"/>
        /// with an ID that does not exist anywhere in the tree is a
        /// silent no-op — no throw, no mutation of the surviving
        /// Compositions collection. The recursive override must not
        /// throw on missing IDs because <c>NormalizeDevice</c> calls it
        /// per-invalid-Composition and any exception would abort device
        /// onboarding for the remaining valid children.
        /// </summary>
        [Test]
        public void RemoveComposition_nonexistent_id_is_idempotent_no_op()
        {
            var device = new Device { Id = "d1", Uuid = "d1", Name = "d1", Type = Device.TypeId };
            var host = new Component { Id = "host", Name = "host", Type = "Axes" };
            host.AddComposition(new Composition { Id = "keep-me", Name = "keep-me", Type = "Generic" });
            device.AddComponent(host);

            Assert.DoesNotThrow(() => device.RemoveComposition("does-not-exist"),
                "RemoveComposition on a non-existent ID must be a silent no-op — NormalizeDevice loops over invalid Compositions and would abort onboarding on any throw here.");

            Assert.That(device.GetCompositions().Any(c => c.Id == "keep-me"), Is.True,
                "RemoveComposition on a non-existent ID must not mutate the surviving Compositions.");
        }

        /// <summary>
        /// Pins that <see cref="Device.RemoveComposition"/> is safe against
        /// a Device with no Components and no Compositions at all — both
        /// early-return-on-empty branches inside the override must exit
        /// without throwing. This is a boundary case the FLOOR requires
        /// (§1.0d-trigies-novodecies documented input classes: empty).
        /// </summary>
        [Test]
        public void RemoveComposition_on_empty_device_tree_is_safe()
        {
            var device = new Device { Id = "d1", Uuid = "d1", Name = "d1", Type = Device.TypeId };

            Assert.DoesNotThrow(() => device.RemoveComposition("nothing"),
                "RemoveComposition must exit safely when the Device has neither Components nor Compositions.");
        }

        // --------------------------------------------------------------
        // RemoveDataItem — every code path in the top-level-restoring override.
        // --------------------------------------------------------------

        /// <summary>
        /// Pins that a DataItem attached directly to <see cref="Device.DataItems"/>
        /// is dropped by <see cref="Device.RemoveDataItem"/>. Before the
        /// F-TEST-BUG-2 fix (Device.cs:1049) the override walked ONLY child
        /// Components and silently skipped the Device's own DataItems
        /// collection, so a generic DataItem on the Device itself was
        /// unremovable and <c>NormalizeDevice.Remove</c> was a lie.
        /// </summary>
        [Test]
        public void RemoveDataItem_drops_top_level_DataItem_on_Device()
        {
            var device = new Device { Id = "d1", Uuid = "d1", Name = "d1", Type = Device.TypeId };
            device.AddDataItem(new DataItem { Id = "top-di", Type = "Generic", Category = DataItemCategory.EVENT });

            Assert.That(device.DataItems.Any(d => d.Id == "top-di"), Is.True,
                "precondition — top-level DataItem must be present on Device before removal.");

            device.RemoveDataItem("top-di");

            Assert.That(device.DataItems == null || !device.DataItems.Any(d => d.Id == "top-di"), Is.True,
                "Device.RemoveDataItem must drop a DataItem placed directly on Device.DataItems (F-TEST-BUG-2 fix — the override previously skipped this collection).");
        }

        /// <summary>
        /// Pins deep-Component DataItem removal:
        /// Device → Component → subComponent → subsubComponent → DataItem.
        /// The override iterates <see cref="Device.GetComponents()"/> which is
        /// recursive and returns a flat list of every Component at any
        /// depth; the DataItem must therefore be removed regardless of
        /// how deep the owning Component sits.
        /// </summary>
        [Test]
        public void RemoveDataItem_recurses_into_great_grandchild_Component()
        {
            var device = new Device { Id = "d1", Uuid = "d1", Name = "d1", Type = Device.TypeId };

            var l1 = new Component { Id = "l1", Name = "l1", Type = "Axes" };
            var l2 = new Component { Id = "l2", Name = "l2", Type = "Linear" };
            var l3 = new Component { Id = "l3", Name = "l3", Type = "Motor" };
            l3.AddDataItem(new DataItem { Id = "deep-di", Type = "Generic", Category = DataItemCategory.EVENT });
            l2.AddComponent(l3);
            l1.AddComponent(l2);
            device.AddComponent(l1);

            Assert.That(device.GetDataItems().Any(d => d.Id == "deep-di"), Is.True,
                "precondition — great-grandchild DataItem must be reachable via GetDataItems before removal.");

            device.RemoveDataItem("deep-di");

            Assert.That(device.GetDataItems() == null || !device.GetDataItems().Any(d => d.Id == "deep-di"), Is.True,
                "Device.RemoveDataItem must reach any-depth Component DataItems via the recursive GetComponents() flat list.");
        }

        /// <summary>
        /// Pins idempotency on a non-existent ID: no throw, no mutation
        /// of the surviving DataItems. NormalizeDevice invokes this
        /// per-invalid-DataItem and cannot tolerate an exception path
        /// on missing IDs.
        /// </summary>
        [Test]
        public void RemoveDataItem_nonexistent_id_is_idempotent_no_op()
        {
            var device = new Device { Id = "d1", Uuid = "d1", Name = "d1", Type = Device.TypeId };
            device.AddDataItem(new DataItem { Id = "keep-me", Type = "Generic", Category = DataItemCategory.EVENT });

            Assert.DoesNotThrow(() => device.RemoveDataItem("does-not-exist"),
                "RemoveDataItem on a non-existent ID must be a silent no-op.");

            Assert.That(device.DataItems.Any(d => d.Id == "keep-me"), Is.True,
                "RemoveDataItem on a non-existent ID must not mutate the surviving DataItems.");
        }

        /// <summary>
        /// Pins that <see cref="Device.RemoveDataItem"/> is safe against
        /// a Device with no DataItems and no Components — both
        /// early-return branches inside the override must exit cleanly.
        /// </summary>
        [Test]
        public void RemoveDataItem_on_empty_device_tree_is_safe()
        {
            var device = new Device { Id = "d1", Uuid = "d1", Name = "d1", Type = Device.TypeId };

            Assert.DoesNotThrow(() => device.RemoveDataItem("nothing"),
                "RemoveDataItem must exit safely when the Device has neither DataItems nor Components.");
        }

        // --------------------------------------------------------------
        // Sibling isolation — the recursive RemoveAll(o => o.Id == id)
        // predicate must only match the requested ID and leave every
        // sibling untouched. A regression to a permissive predicate
        // (for example RemoveAll(o => true) inside a wrong overload)
        // would strip every sibling and would pass the single-child
        // tests above; the sibling-isolation shape catches that class.
        // --------------------------------------------------------------

        /// <summary>
        /// Pins that <see cref="Device.RemoveComposition"/> called against
        /// one of two siblings on the top-level <c>Device.Compositions</c>
        /// collection drops only the requested Composition and leaves the
        /// sibling intact.
        /// </summary>
        [Test]
        public void RemoveComposition_top_level_leaves_sibling_Compositions_intact()
        {
            var device = new Device { Id = "d1", Uuid = "d1", Name = "d1", Type = Device.TypeId };
            device.AddComposition(new Composition { Id = "drop-me", Name = "drop-me", Type = "Generic" });
            device.AddComposition(new Composition { Id = "keep-me", Name = "keep-me", Type = "Generic" });

            device.RemoveComposition("drop-me");

            Assert.That(device.Compositions.Any(c => c.Id == "drop-me"), Is.False,
                "RemoveComposition must drop the requested top-level Composition.");
            Assert.That(device.Compositions.Any(c => c.Id == "keep-me"), Is.True,
                "RemoveComposition must not drop siblings of the requested Composition — the RemoveAll predicate is ID-scoped.");
        }

        /// <summary>
        /// Pins the same sibling-isolation invariant on the recursive
        /// nested branch: two Compositions attached to a child Component,
        /// only one requested for removal.
        /// </summary>
        [Test]
        public void RemoveComposition_nested_leaves_sibling_Compositions_intact()
        {
            var device = new Device { Id = "d1", Uuid = "d1", Name = "d1", Type = Device.TypeId };
            var host = new Component { Id = "host", Name = "host", Type = "Axes" };
            host.AddComposition(new Composition { Id = "drop-nested", Name = "drop-nested", Type = "Generic" });
            host.AddComposition(new Composition { Id = "keep-nested", Name = "keep-nested", Type = "Generic" });
            device.AddComponent(host);

            device.RemoveComposition("drop-nested");

            Assert.That(device.GetCompositions().Any(c => c.Id == "drop-nested"), Is.False);
            Assert.That(device.GetCompositions().Any(c => c.Id == "keep-nested"), Is.True,
                "the nested recursion must not drop siblings.");
        }

        /// <summary>
        /// Pins that a Composition with the same ID present at BOTH the
        /// top-level Device.Compositions AND on a nested child Component
        /// is dropped from both locations — RemoveComposition is depth-
        /// unlimited, not first-match. This pins the recursion contract
        /// against a regression that returns on the first match.
        /// </summary>
        [Test]
        public void RemoveComposition_removes_id_from_every_depth_it_appears()
        {
            var device = new Device { Id = "d1", Uuid = "d1", Name = "d1", Type = Device.TypeId };
            device.AddComposition(new Composition { Id = "dup-id", Name = "dup-id-top", Type = "Generic" });
            var host = new Component { Id = "host", Name = "host", Type = "Axes" };
            host.AddComposition(new Composition { Id = "dup-id", Name = "dup-id-nested", Type = "Generic" });
            device.AddComponent(host);

            Assert.That(device.GetCompositions().Count(c => c.Id == "dup-id"), Is.EqualTo(2),
                "precondition — the same ID must be present at both depths.");

            device.RemoveComposition("dup-id");

            var remaining = device.GetCompositions();
            Assert.That(remaining == null || !remaining.Any(c => c.Id == "dup-id"), Is.True,
                "RemoveComposition must remove EVERY occurrence of the ID — top-level AND nested — not just the first match.");
        }

        /// <summary>
        /// Pins that <see cref="Device.RemoveDataItem"/> called against one of
        /// two siblings on the top-level <c>Device.DataItems</c> collection
        /// drops only the requested DataItem and leaves the sibling intact.
        /// </summary>
        [Test]
        public void RemoveDataItem_top_level_leaves_sibling_DataItems_intact()
        {
            var device = new Device { Id = "d1", Uuid = "d1", Name = "d1", Type = Device.TypeId };
            device.AddDataItem(new DataItem { Id = "drop-me", Type = "Generic", Category = DataItemCategory.EVENT });
            device.AddDataItem(new DataItem { Id = "keep-me", Type = "Generic", Category = DataItemCategory.EVENT });

            device.RemoveDataItem("drop-me");

            Assert.That(device.DataItems.Any(d => d.Id == "drop-me"), Is.False,
                "RemoveDataItem must drop the requested top-level DataItem.");
            Assert.That(device.DataItems.Any(d => d.Id == "keep-me"), Is.True,
                "RemoveDataItem must not drop siblings of the requested DataItem.");
        }

        /// <summary>
        /// Pins the same sibling-isolation invariant for the nested branch:
        /// two DataItems attached to a child Component, only one requested
        /// for removal.
        /// </summary>
        [Test]
        public void RemoveDataItem_nested_leaves_sibling_DataItems_intact()
        {
            var device = new Device { Id = "d1", Uuid = "d1", Name = "d1", Type = Device.TypeId };
            var host = new Component { Id = "host", Name = "host", Type = "Axes" };
            host.AddDataItem(new DataItem { Id = "drop-nested", Type = "Generic", Category = DataItemCategory.EVENT });
            host.AddDataItem(new DataItem { Id = "keep-nested", Type = "Generic", Category = DataItemCategory.EVENT });
            device.AddComponent(host);

            device.RemoveDataItem("drop-nested");

            Assert.That(device.GetDataItems().Any(d => d.Id == "drop-nested"), Is.False);
            Assert.That(device.GetDataItems().Any(d => d.Id == "keep-nested"), Is.True,
                "nested-branch removal must not drop siblings.");
        }

        /// <summary>
        /// Pins that a DataItem ID present at BOTH the top-level
        /// Device.DataItems AND on a nested child Component is dropped
        /// from both locations — RemoveDataItem visits every depth,
        /// not just the first match.
        /// </summary>
        [Test]
        public void RemoveDataItem_removes_id_from_every_depth_it_appears()
        {
            var device = new Device { Id = "d1", Uuid = "d1", Name = "d1", Type = Device.TypeId };
            device.AddDataItem(new DataItem { Id = "dup-id", Type = "Generic", Category = DataItemCategory.EVENT });
            var host = new Component { Id = "host", Name = "host", Type = "Axes" };
            host.AddDataItem(new DataItem { Id = "dup-id", Type = "Generic", Category = DataItemCategory.EVENT });
            device.AddComponent(host);

            Assert.That(device.GetDataItems().Count(d => d.Id == "dup-id"), Is.EqualTo(2),
                "precondition — the same ID must be present at both depths.");

            device.RemoveDataItem("dup-id");

            var remaining = device.GetDataItems();
            Assert.That(remaining == null || !remaining.Any(d => d.Id == "dup-id"), Is.True,
                "RemoveDataItem must remove EVERY occurrence of the ID — top-level AND nested.");
        }

        /// <summary>
        /// Pins the intermediate depth-2 Composition-removal branch that
        /// sits BETWEEN the top-level (depth-1) and great-grandchild
        /// (depth-3) coverage above: Device → Component → Composition
        /// directly on the child Component. This is the shape the
        /// original NormalizeDevice-Remove path used before the recursive
        /// fix; a regression to a two-level walk would still pass the
        /// depth-1 top-level test and might pass the depth-3 test via a
        /// different code path — the depth-2 test pins the exact single
        /// level of recursion.
        /// </summary>
        [Test]
        public void RemoveComposition_removes_depth_2_Composition_on_direct_child_Component()
        {
            var device = new Device { Id = "d1", Uuid = "d1", Name = "d1", Type = Device.TypeId };
            var child = new Component { Id = "child", Name = "child", Type = "Axes" };
            child.AddComposition(new Composition { Id = "mid-comp", Name = "mid-comp", Type = "Generic" });
            device.AddComponent(child);

            Assert.That(device.GetCompositions().Any(c => c.Id == "mid-comp"), Is.True,
                "precondition — the depth-2 Composition must be reachable via GetCompositions.");

            device.RemoveComposition("mid-comp");

            var remaining = device.GetCompositions();
            Assert.That(remaining == null || !remaining.Any(c => c.Id == "mid-comp"), Is.True,
                "RemoveComposition must reach depth-2 Compositions attached to a direct child Component.");
        }

        // --------------------------------------------------------------
        // Cycle-guard coverage — a cyclic Component graph (A→B→A) must
        // terminate the recursive walk instead of stack-overflowing. Two
        // paths carry a cycle guard: RemoveComposition and RemoveDataItem
        // on both Device and Component. Each fixture below exercises one
        // path directly; a regression to the unguarded pre-fix shape
        // fails as a `StackOverflowException` (process abort), which
        // NUnit surfaces as fixture-level failure.
        // --------------------------------------------------------------

        /// <summary>
        /// Pins that <see cref="Device.RemoveComposition(string)"/> terminates on a
        /// cyclic Component graph. The audit brief (cycle-1 finding H1) required a
        /// visited-Id set threaded through the recursive walk so a
        /// <c>A.Components ∋ B</c>, <c>B.Components ∋ A</c> shape does not
        /// stack-overflow the process.
        /// </summary>
        [Test]
        public void RemoveComposition_terminates_on_cyclic_Component_graph()
        {
            var device = new Device { Id = "d1", Uuid = "d1", Name = "d1", Type = Device.TypeId };
            var a = new Component { Id = "A", Name = "A", Type = "Axes" };
            var b = new Component { Id = "B", Name = "B", Type = "Axes" };
            device.AddComponent(a);
            a.AddComponent(b);
            // Force the cycle by direct assignment — AddComponent would reset Parent
            // linkages but the recursive walk only reads .Components.
            b.Components = new List<IComponent> { a };

            // A no-op removal (nothing to remove) must still traverse the cyclic graph
            // to exhaustion without recursing forever.
            Assert.DoesNotThrow(() => device.RemoveComposition("missing"),
                "Device.RemoveComposition must terminate on a cyclic Component graph — no StackOverflowException.");
        }

        /// <summary>
        /// Pins that <see cref="Device.RemoveDataItem(string)"/> terminates on a
        /// cyclic Component graph. Sibling of the RemoveComposition cycle test —
        /// the inline recursive walk replaces the previous <see cref="Device.GetComponents()"/>
        /// flatten, which was itself unguarded.
        /// </summary>
        [Test]
        public void RemoveDataItem_terminates_on_cyclic_Component_graph()
        {
            var device = new Device { Id = "d1", Uuid = "d1", Name = "d1", Type = Device.TypeId };
            var a = new Component { Id = "A", Name = "A", Type = "Axes" };
            var b = new Component { Id = "B", Name = "B", Type = "Axes" };
            device.AddComponent(a);
            a.AddComponent(b);
            b.Components = new List<IComponent> { a };

            Assert.DoesNotThrow(() => device.RemoveDataItem("missing"),
                "Device.RemoveDataItem must terminate on a cyclic Component graph — no StackOverflowException.");
        }

        /// <summary>
        /// Pins that <see cref="Device.RemoveComponent(string)"/> terminates on a
        /// cyclic Component graph. Sibling of the RemoveComposition /
        /// RemoveDataItem cycle tests — dime cycle-2 finding H1-C2 called out
        /// this Remove* variant as still lacking the visited-Id + depth-cap
        /// guard the other two variants received in cycle-1 H1. The callsite
        /// on Strict validation is <c>MTConnectAgent.NormalizeDevice</c>
        /// (<c>obj.RemoveComponent(genericComponent.Id)</c>); a cyclic
        /// Component graph coming through that path would stack-overflow the
        /// process without this guard.
        /// </summary>
        [Test]
        public void RemoveComponent_terminates_on_cyclic_Component_graph()
        {
            var device = new Device { Id = "d1", Uuid = "d1", Name = "d1", Type = Device.TypeId };
            var a = new Component { Id = "A", Name = "A", Type = "Axes" };
            var b = new Component { Id = "B", Name = "B", Type = "Axes" };
            device.AddComponent(a);
            a.AddComponent(b);
            b.Components = new List<IComponent> { a };

            Assert.DoesNotThrow(() => device.RemoveComponent("missing"),
                "Device.RemoveComponent must terminate on a cyclic Component graph — no StackOverflowException.");
        }

        /// <summary>
        /// Pins that <see cref="Component.RemoveComposition(string)"/> is now
        /// recursive across nested child Components AND terminates on a cyclic
        /// graph — the audit brief finding M2 called out this sibling site as
        /// still non-recursive after the Device.cs fix. A regression that
        /// re-strips the recursive walk fails the depth-2 removal; a regression
        /// that reintroduces the walk without the cycle guard fails the
        /// termination assertion.
        /// </summary>
        [Test]
        public void Component_RemoveComposition_reaches_nested_and_terminates_on_cycle()
        {
            var root = new Component { Id = "root", Name = "root", Type = "Axes" };
            var child = new Component { Id = "child", Name = "child", Type = "Axes" };
            child.AddComposition(new Composition { Id = "nested-comp", Name = "nested-comp", Type = "Generic" });
            root.AddComponent(child);

            root.RemoveComposition("nested-comp");

            Assert.That(
                child.Compositions == null || !child.Compositions.Any(c => c.Id == "nested-comp"),
                Is.True,
                "Component.RemoveComposition must reach a Composition nested on a direct child Component.");

            // Cycle graph: root→cycleA→cycleB→cycleA
            var cycleA = new Component { Id = "cycleA", Name = "cycleA", Type = "Axes" };
            var cycleB = new Component { Id = "cycleB", Name = "cycleB", Type = "Axes" };
            root.AddComponent(cycleA);
            cycleA.AddComponent(cycleB);
            cycleB.Components = new List<IComponent> { cycleA };

            Assert.DoesNotThrow(() => root.RemoveComposition("missing"),
                "Component.RemoveComposition must terminate on a cyclic Component graph — no StackOverflowException.");
        }

        /// <summary>
        /// Pins that <see cref="Component.RemoveDataItem(string)"/> terminates on
        /// a cyclic Component graph. Sibling of the Component.RemoveComposition
        /// cycle test — the inline recursive walk replaces the previous
        /// <see cref="Component.GetComponents()"/> flatten, which was itself
        /// unguarded.
        /// </summary>
        [Test]
        public void Component_RemoveDataItem_terminates_on_cyclic_Component_graph()
        {
            var root = new Component { Id = "root", Name = "root", Type = "Axes" };
            var a = new Component { Id = "A", Name = "A", Type = "Axes" };
            var b = new Component { Id = "B", Name = "B", Type = "Axes" };
            root.AddComponent(a);
            a.AddComponent(b);
            b.Components = new List<IComponent> { root };

            Assert.DoesNotThrow(() => root.RemoveDataItem("missing"),
                "Component.RemoveDataItem must terminate on a cyclic Component graph — no StackOverflowException.");
        }

        // --------------------------------------------------------------
        // MaxComponentWalkDepth = 1024 — belt-and-braces depth ceiling.
        // The visited-Id HashSet catches ordinary cycles; the depth
        // ceiling defends against pathological cases the HashSet cannot
        // prune (for example every Component in the cycle carrying a
        // null Id so nothing gets added to the set). A deep linear
        // chain of unique-Id Components exercises the depth ceiling
        // exclusively — the HashSet always .Add-returns true so only
        // the `depth > MaxComponentWalkDepth` early-return terminates.
        //
        // The loop walks depth 1..1024 (inclusive) and returns early
        // at depth 1025 — placing a Composition on the component at
        // depth 1000 verifies the walk reaches within the ceiling
        // (removed), placing another at depth 1030 verifies the walk
        // does NOT reach past the ceiling (survives). A regression
        // that raises or removes the ceiling makes the depth-1030
        // arm fail; a regression that lowers the ceiling makes the
        // depth-1000 arm fail. Two-arm construction pins the exact
        // ceiling value.
        // --------------------------------------------------------------

        private static Device BuildLinearDeepDevice(int chainLength, int shallowIndex, int deepIndex,
            out string shallowTargetId, out string deepTargetId,
            out Component shallowLeaf, out Component deepLeaf)
        {
            shallowTargetId = "target-shallow";
            deepTargetId = "target-deep";
            var device = new Device { Id = "d1", Uuid = "d1", Name = "d1", Type = Device.TypeId };
            Component parent = null!;
            Component capturedShallow = null!;
            Component capturedDeep = null!;
            for (var i = 0; i < chainLength; i++)
            {
                var c = new Component { Id = $"c-{i}", Name = $"c-{i}", Type = "Axes" };
                if (i == 0) device.AddComponent(c);
                else parent!.AddComponent(c);
                parent = c;

                if (i == shallowIndex)
                {
                    c.AddComposition(new Composition
                    {
                        Id = shallowTargetId,
                        Name = shallowTargetId,
                        Type = "Generic"
                    });
                    capturedShallow = c;
                }
                if (i == deepIndex)
                {
                    c.AddComposition(new Composition
                    {
                        Id = deepTargetId,
                        Name = deepTargetId,
                        Type = "Generic"
                    });
                    capturedDeep = c;
                }
            }
            shallowLeaf = capturedShallow;
            deepLeaf = capturedDeep;
            return device;
        }

        /// <summary>
        /// Pins <c>MaxComponentWalkDepth = 1024</c> on <see cref="Device.RemoveComposition(string)"/>.
        /// Component c-999 sits at depth 1000 from Device (within the ceiling) and
        /// c-1029 sits at depth 1030 (past the ceiling). A single Remove call must
        /// process the shallow arm (removed) and skip the deep arm (survives)
        /// because the recursive helper returns early at depth 1025 — the frame
        /// on c-1024 is entered but bails before touching its children.
        /// </summary>
        [Test]
        public void RemoveComposition_depth_ceiling_removes_within_1024_leaves_past_1024_intact()
        {
            var device = BuildLinearDeepDevice(chainLength: 1030,
                shallowIndex: 999, deepIndex: 1029,
                out var shallowId, out var deepId,
                out var shallowLeaf, out var deepLeaf);

            // Precondition — both targets are present before the removal.
            Assert.That(shallowLeaf.Compositions.Any(c => c.Id == shallowId), Is.True,
                "precondition — the shallow target Composition must be attached at depth 1000.");
            Assert.That(deepLeaf.Compositions.Any(c => c.Id == deepId), Is.True,
                "precondition — the deep target Composition must be attached at depth 1030.");

            // Remove the shallow target — expected to succeed.
            device.RemoveComposition(shallowId);
            Assert.That(
                shallowLeaf.Compositions == null || !shallowLeaf.Compositions.Any(c => c.Id == shallowId),
                Is.True,
                "Device.RemoveComposition must reach depth 1000 (within MaxComponentWalkDepth = 1024) and drop the shallow target.");

            // Remove the deep target — expected to be a no-op because the depth
            // ceiling stops the walk before reaching depth 1030.
            device.RemoveComposition(deepId);
            Assert.That(
                deepLeaf.Compositions.Any(c => c.Id == deepId), Is.True,
                "Device.RemoveComposition must NOT reach past MaxComponentWalkDepth = 1024 — the belt-and-braces depth guard leaves depth-1030 Compositions intact so a pathological deeply-nested (or null-Id-cyclic) graph cannot exhaust the process stack.");
        }

        /// <summary>
        /// Sibling pin for <see cref="Device.RemoveDataItem(string)"/>. Same
        /// two-arm shape via DataItems attached to the shallow and deep leaves —
        /// the DataItem walk shares <c>MaxComponentWalkDepth = 1024</c> with the
        /// Composition walk.
        /// </summary>
        [Test]
        public void RemoveDataItem_depth_ceiling_removes_within_1024_leaves_past_1024_intact()
        {
            var device = BuildLinearDeepDevice(chainLength: 1030,
                shallowIndex: 999, deepIndex: 1029,
                out _, out _,
                out var shallowLeaf, out var deepLeaf);

            const string shallowDi = "di-shallow";
            const string deepDi = "di-deep";
            shallowLeaf.AddDataItem(new DataItem { Id = shallowDi, Name = shallowDi, Type = "Generic", Category = DataItemCategory.EVENT });
            deepLeaf.AddDataItem(new DataItem { Id = deepDi, Name = deepDi, Type = "Generic", Category = DataItemCategory.EVENT });

            device.RemoveDataItem(shallowDi);
            Assert.That(
                shallowLeaf.DataItems == null || !shallowLeaf.DataItems.Any(d => d.Id == shallowDi),
                Is.True,
                "Device.RemoveDataItem must reach depth 1000 (within MaxComponentWalkDepth = 1024) and drop the shallow DataItem.");

            device.RemoveDataItem(deepDi);
            Assert.That(
                deepLeaf.DataItems.Any(d => d.Id == deepDi), Is.True,
                "Device.RemoveDataItem must NOT reach past MaxComponentWalkDepth = 1024 — DataItems attached at depth 1030 must survive so the belt-and-braces depth ceiling is exercised.");
        }

        /// <summary>
        /// Sibling pin for <see cref="Component.RemoveComposition(string)"/> —
        /// the Component-side ceiling constant is defined in Component.cs
        /// independently of Device.cs, so a regression that bumps only the
        /// Device.cs constant while leaving Component.cs stale (or vice versa)
        /// fails on the sibling that still enforces 1024.
        /// </summary>
        [Test]
        public void Component_RemoveComposition_depth_ceiling_leaves_past_1024_intact()
        {
            // Rooted at a Component this time, not a Device.
            var root = new Component { Id = "root", Name = "root", Type = "Axes" };
            Component parent = root;
            Component shallowLeaf = null!;
            Component deepLeaf = null!;
            for (var i = 0; i < 1030; i++)
            {
                var c = new Component { Id = $"c-{i}", Name = $"c-{i}", Type = "Axes" };
                parent.AddComponent(c);
                parent = c;
                if (i == 999)
                {
                    c.AddComposition(new Composition { Id = "target-shallow", Name = "target-shallow", Type = "Generic" });
                    shallowLeaf = c;
                }
                if (i == 1029)
                {
                    c.AddComposition(new Composition { Id = "target-deep", Name = "target-deep", Type = "Generic" });
                    deepLeaf = c;
                }
            }

            root.RemoveComposition("target-shallow");
            Assert.That(
                shallowLeaf.Compositions == null || !shallowLeaf.Compositions.Any(c => c.Id == "target-shallow"),
                Is.True,
                "Component.RemoveComposition must reach depth 1000 (within MaxComponentWalkDepth = 1024) and drop the shallow target.");

            root.RemoveComposition("target-deep");
            Assert.That(
                deepLeaf.Compositions.Any(c => c.Id == "target-deep"), Is.True,
                "Component.RemoveComposition must NOT reach past MaxComponentWalkDepth = 1024 — the Component-side ceiling constant must stay in sync with the Device-side ceiling.");
        }

        // --------------------------------------------------------------
        // Trace-cap-hit diagnostic pin — dime L3-C2
        // --------------------------------------------------------------
        //
        // Cycle-2 L3-C2 added `Trace.TraceWarning` lines to the three
        // Device.Remove* private overloads that fire when the walk hits
        // depth > MaxComponentWalkDepth = 1024. The existing depth-ceiling
        // tests (RemoveComposition_depth_ceiling_removes_within_1024_leaves_past_1024_intact
        // and RemoveDataItem_depth_ceiling_removes_within_1024_leaves_past_1024_intact)
        // pin the null-effect contract (the deep target survives) but do NOT
        // capture the trace output — a regression that silently drops the
        // TraceWarning line still passes the intact-target assertions. This
        // block attaches a TraceListener and pins the diagnostic shape so
        // operators keep the actionable "walk depth 1024 exceeded" hint.

        private sealed class CapturingTraceListener : TraceListener
        {
            public List<string> Warnings { get; } = new List<string>();
            private readonly StringBuilder _lineBuffer = new StringBuilder();
            public override void Write(string? message) => _lineBuffer.Append(message);
            public override void WriteLine(string? message)
            {
                _lineBuffer.Append(message);
                _lineBuffer.Clear();
            }
            public override void TraceEvent(TraceEventCache? eventCache, string source, TraceEventType eventType, int id, string? message)
            {
                if (eventType == TraceEventType.Warning) Warnings.Add(message ?? string.Empty);
            }
            public override void TraceEvent(TraceEventCache? eventCache, string source, TraceEventType eventType, int id, string? format, params object?[]? args)
            {
                var message = args != null && args.Length > 0 && format != null ? string.Format(format, args) : format;
                TraceEvent(eventCache, source, eventType, id, message);
            }
        }

        /// <summary>
        /// Pins the L3-C2 trace-warning shape for
        /// <see cref="Device.RemoveComposition(string)"/>. A regression that
        /// drops the <c>Trace.TraceWarning</c> line inside the depth-guard
        /// early-return still passes the depth-ceiling behavioral test above
        /// because the deep-target-survives assertion only observes the
        /// null-effect, not the diagnostic. This fixture captures Trace output
        /// and asserts the warning fires with the exact shape operators grep on.
        /// </summary>
        [Test]
        public void RemoveComposition_depth_ceiling_hit_traces_warning()
        {
            var listener = new CapturingTraceListener();
            Trace.Listeners.Add(listener);
            try
            {
                var device = BuildLinearDeepDevice(chainLength: 1030,
                    shallowIndex: 999, deepIndex: 1029,
                    out _, out var deepId,
                    out _, out _);

                device.RemoveComposition(deepId);

                Assert.That(listener.Warnings.Any(w => w.Contains("Device.RemoveComposition") && w.Contains("walk depth 1024 exceeded")),
                    Is.True,
                    "the depth-cap-hit path must emit a Trace.TraceWarning naming Device.RemoveComposition and the exceeded ceiling — dime L3-C2 diagnostic contract.");
                Assert.That(listener.Warnings.Any(w => w.Contains("Device.RemoveComposition[d1]")),
                    Is.True,
                    "the depth-cap-hit trace warning must interpolate the device Id in [Id] brackets between the method name and the message body — dime F-IMP-C3-001 fleet-bisection contract; a regression that dropped the [d1] tag would silently revert the operator-diagnostic improvement.");
            }
            finally
            {
                Trace.Listeners.Remove(listener);
            }
        }

        /// <summary>
        /// Sibling L3-C2 pin for <see cref="Device.RemoveDataItem(string)"/>.
        /// The three Device.Remove* variants share the ceiling AND the trace shape;
        /// a regression that dropped the trace on just one variant would slip past
        /// the other two variants' pins.
        /// </summary>
        [Test]
        public void RemoveDataItem_depth_ceiling_hit_traces_warning()
        {
            var listener = new CapturingTraceListener();
            Trace.Listeners.Add(listener);
            try
            {
                var device = BuildLinearDeepDevice(chainLength: 1030,
                    shallowIndex: 999, deepIndex: 1029,
                    out _, out _,
                    out _, out var deepLeaf);
                const string deepDi = "di-deep-trace";
                deepLeaf.AddDataItem(new DataItem { Id = deepDi, Name = deepDi, Type = "Generic", Category = DataItemCategory.EVENT });

                device.RemoveDataItem(deepDi);

                Assert.That(listener.Warnings.Any(w => w.Contains("Device.RemoveDataItem") && w.Contains("walk depth 1024 exceeded")),
                    Is.True,
                    "the depth-cap-hit path must emit a Trace.TraceWarning naming Device.RemoveDataItem and the exceeded ceiling — dime L3-C2 diagnostic contract.");
                Assert.That(listener.Warnings.Any(w => w.Contains("Device.RemoveDataItem[d1]")),
                    Is.True,
                    "the depth-cap-hit trace warning must interpolate the device Id in [Id] brackets between the method name and the message body — dime F-IMP-C3-001 fleet-bisection contract; a regression that dropped the [d1] tag would silently revert the operator-diagnostic improvement.");
            }
            finally
            {
                Trace.Listeners.Remove(listener);
            }
        }

        /// <summary>
        /// Sibling L3-C2 pin for <see cref="Device.RemoveComponent(string)"/> —
        /// the H1-C2 fix added the same depth guard on this Remove* variant.
        /// A pathologically deep linear chain past depth 1024 hits the ceiling
        /// on the walk into the chain regardless of whether the componentId
        /// exists (RemoveComponent walks children and removes matching Ids;
        /// on a linear chain with no matching Id, the walk visits every frame
        /// until the ceiling fires).
        /// </summary>
        [Test]
        public void RemoveComponent_depth_ceiling_hit_traces_warning()
        {
            var listener = new CapturingTraceListener();
            Trace.Listeners.Add(listener);
            try
            {
                var device = BuildLinearDeepDevice(chainLength: 1030,
                    shallowIndex: 999, deepIndex: 1029,
                    out _, out _,
                    out _, out _);

                // Call with a Component Id that doesn't exist anywhere in the
                // chain — the walk visits every frame looking for it, hits the
                // ceiling at depth 1025, fires the trace warning.
                device.RemoveComponent("does-not-exist");

                Assert.That(listener.Warnings.Any(w => w.Contains("Device.RemoveComponent") && w.Contains("walk depth 1024 exceeded")),
                    Is.True,
                    "the depth-cap-hit path must emit a Trace.TraceWarning naming Device.RemoveComponent and the exceeded ceiling — dime L3-C2 diagnostic contract extended to the H1-C2 addition.");
                Assert.That(listener.Warnings.Any(w => w.Contains("Device.RemoveComponent[d1]")),
                    Is.True,
                    "the depth-cap-hit trace warning must interpolate the device Id in [Id] brackets between the method name and the message body — dime F-IMP-C3-001 fleet-bisection contract; a regression that dropped the [d1] tag would silently revert the operator-diagnostic improvement.");
            }
            finally
            {
                Trace.Listeners.Remove(listener);
            }
        }
    }
}
