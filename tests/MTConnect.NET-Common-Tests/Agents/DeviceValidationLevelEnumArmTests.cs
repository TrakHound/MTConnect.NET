// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Linq;
using System.Threading;
using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Devices.DataItems;
using NUnit.Framework;

namespace MTConnect.NET_Common_Tests.Agents
{
    /// <summary>
    /// Enum-arm coverage FLOOR (CONVENTIONS §1.0d-trigies-novodecies) for the
    /// <see cref="DeviceValidationLevel"/> enum introduced by PR #219 commit
    /// 90daffca. That commit added the enum, an <c>AgentConfiguration.DeviceValidationLevel</c>
    /// property, and swapped every <c>InputValidationLevel</c> reference in
    /// <c>MTConnectAgent.NormalizeDevice(IDevice)</c> to the new
    /// <c>DeviceValidationLevel</c> — but shipped ZERO tests for any of the four
    /// enum arms on any of the three validation sites.
    ///
    /// The three validation sites in <c>MTConnectAgent.NormalizeDevice</c>
    /// (MTConnectAgent.cs:1315–1363) branch on <c>DeviceValidationLevel</c>:
    ///
    ///   * generic Component  →  Raise + optionally Remove / Strict-null
    ///   * generic Composition →  Raise + optionally Remove / Strict-null
    ///   * generic DataItem   →  Raise + optionally Remove / Strict-null
    ///
    /// A "generic" child is one whose runtime type is exactly the base
    /// <see cref="Component"/> / <see cref="Composition"/> / <see cref="DataItem"/>
    /// class (i.e. not resolved to a concrete standard-defined type such as
    /// <see cref="AvailabilityDataItem"/>). The three sites use
    /// <c>o.GetType() == typeof(Component)</c> etc. to detect that shape.
    ///
    /// This fixture pins every (enum-arm × validation-site) combination — 4 × 3
    /// = 12 combinations plus the InvalidComponentAdded / InvalidCompositionAdded
    /// / InvalidDataItemAdded event-firing contract (Warning / Remove / Strict
    /// raise; Ignore does not). Under the FLOOR: every enum value in an enum
    /// used by the code under test has a test that exercises that arm; every
    /// early-return-on-invalid has a test that hits it.
    /// </summary>
    [TestFixture]
    [Category("DeviceValidationLevelEnumArm")]
    public class DeviceValidationLevelEnumArmTests
    {
        private const string DeviceUuid = "dev-devvalidation-1";
        private const string GenericComponentId = "generic-comp";
        private const string GenericCompositionId = "generic-comp-of";
        private const string GenericDataItemId = "generic-di";
        private const string GenericComponentType = "UnknownComponentType";
        private const string GenericCompositionType = "UnknownCompositionType";
        private const string GenericDataItemType = "UnknownDataItemType";

        // -----------------------------------------------------------------
        // enum-arm × site — the FLOOR grid. 4 arms × 3 sites = 12 tests.
        // -----------------------------------------------------------------

        /// <summary>
        /// Pins the generic-Component validation site: for each
        /// <see cref="DeviceValidationLevel"/> arm, adding a Device whose
        /// only invalid shape is a base-<see cref="Component"/> child takes
        /// the documented branch — Ignore silently retains, Warning raises
        /// once and retains, Remove raises once and drops the child, Strict
        /// raises once and returns a null device.
        /// </summary>
        [TestCase(DeviceValidationLevel.Ignore)]
        [TestCase(DeviceValidationLevel.Warning)]
        [TestCase(DeviceValidationLevel.Remove)]
        [TestCase(DeviceValidationLevel.Strict)]
        public void GenericComponent_under_each_level_takes_the_documented_branch(DeviceValidationLevel level)
        {
            using var agent = NewAgent(level);
            var device = BuildDeviceWithGenericComponent();

            var raised = 0;
            IComponent? raisedComponent = null;
            agent.InvalidComponentAdded += (_, comp, _) =>
            {
                Interlocked.Increment(ref raised);
                raisedComponent = comp;
            };

            var added = agent.AddDevice(device, initializeDataItems: false);

            switch (level)
            {
                case DeviceValidationLevel.Ignore:
                    // Ignore: no event, generic component survives, device landed.
                    Assert.That(raised, Is.EqualTo(0), "Ignore must not raise InvalidComponentAdded.");
                    Assert.That(added, Is.Not.Null, "Ignore must retain the device.");
                    Assert.That(ComponentIds(added!).Any(id => id == GenericComponentId), Is.True,
                        "Ignore must retain the generic Component.");
                    break;
                case DeviceValidationLevel.Warning:
                    Assert.That(raised, Is.EqualTo(1), "Warning must raise InvalidComponentAdded exactly once.");
                    Assert.That(raisedComponent!.Id, Is.EqualTo(GenericComponentId));
                    Assert.That(added, Is.Not.Null, "Warning must retain the device.");
                    Assert.That(ComponentIds(added!).Any(id => id == GenericComponentId), Is.True,
                        "Warning must retain the generic Component (event-only, no removal).");
                    break;
                case DeviceValidationLevel.Remove:
                    Assert.That(raised, Is.EqualTo(1), "Remove must raise InvalidComponentAdded exactly once.");
                    Assert.That(added, Is.Not.Null, "Remove must retain the device (only the component is dropped).");
                    Assert.That(ComponentIds(added!).Any(id => id == GenericComponentId), Is.False,
                        "Remove must drop the generic Component from the device tree.");
                    break;
                case DeviceValidationLevel.Strict:
                    Assert.That(raised, Is.EqualTo(1), "Strict must raise InvalidComponentAdded exactly once before nulling.");
                    Assert.That(added, Is.Null, "Strict must return null from NormalizeDevice on the first invalid Component.");
                    break;
            }
        }

        /// <summary>
        /// Pins the generic-Composition validation site: for each
        /// <see cref="DeviceValidationLevel"/> arm, adding a Device with a
        /// base-<see cref="Composition"/> nested under a child Component
        /// takes the documented branch — Ignore silently retains, Warning
        /// raises once and retains, Remove raises once and drops the
        /// nested Composition (F-TEST-BUG-1 fix), Strict raises once and
        /// returns a null device.
        /// </summary>
        [TestCase(DeviceValidationLevel.Ignore)]
        [TestCase(DeviceValidationLevel.Warning)]
        [TestCase(DeviceValidationLevel.Remove)]
        [TestCase(DeviceValidationLevel.Strict)]
        public void GenericComposition_under_each_level_takes_the_documented_branch(DeviceValidationLevel level)
        {
            using var agent = NewAgent(level);
            var device = BuildDeviceWithGenericComposition();

            var raised = 0;
            agent.InvalidCompositionAdded += (_, _, _) => Interlocked.Increment(ref raised);

            var added = agent.AddDevice(device, initializeDataItems: false);

            switch (level)
            {
                case DeviceValidationLevel.Ignore:
                    Assert.That(raised, Is.EqualTo(0));
                    Assert.That(added, Is.Not.Null);
                    Assert.That(FirstChildComponentCompositionIds(added!).Any(id => id == GenericCompositionId), Is.True);
                    break;
                case DeviceValidationLevel.Warning:
                    Assert.That(raised, Is.EqualTo(1));
                    Assert.That(added, Is.Not.Null);
                    Assert.That(FirstChildComponentCompositionIds(added!).Any(id => id == GenericCompositionId), Is.True,
                        "Warning must retain the generic Composition.");
                    break;
                case DeviceValidationLevel.Remove:
                    // F-TEST-BUG-1 fix (Device.cs:664): `Device.RemoveComposition(string)` now
                    // recurses into child Components' Compositions, mirroring the shape of the
                    // recursive `Device.RemoveComponent`. `NormalizeDevice` (MTConnectAgent.cs:1340)
                    // still locates the generic Composition via the recursive `GetCompositions()`;
                    // the Remove call now actually removes it.
                    Assert.That(raised, Is.EqualTo(1),
                        "Remove must raise InvalidCompositionAdded exactly once.");
                    Assert.That(added, Is.Not.Null,
                        "Remove must retain the device — only the generic Composition is dropped.");
                    Assert.That(FirstChildComponentCompositionIds(added!).Any(id => id == GenericCompositionId), Is.False,
                        "Remove must drop the nested generic Composition from the child Component's Compositions collection (F-TEST-BUG-1 fix — Device.RemoveComposition now recurses).");
                    break;
                case DeviceValidationLevel.Strict:
                    Assert.That(raised, Is.EqualTo(1));
                    Assert.That(added, Is.Null, "Strict must null the device on the first invalid Composition.");
                    break;
            }
        }

        /// <summary>
        /// Pins the generic-DataItem validation site: for each
        /// <see cref="DeviceValidationLevel"/> arm, adding a Device with a
        /// base-<see cref="DataItem"/> attached directly to Device.DataItems
        /// takes the documented branch — Ignore silently retains, Warning
        /// raises once and retains, Remove raises once and drops the
        /// top-level DataItem (F-TEST-BUG-2 fix), Strict raises once and
        /// returns a null device.
        /// </summary>
        [TestCase(DeviceValidationLevel.Ignore)]
        [TestCase(DeviceValidationLevel.Warning)]
        [TestCase(DeviceValidationLevel.Remove)]
        [TestCase(DeviceValidationLevel.Strict)]
        public void GenericDataItem_under_each_level_takes_the_documented_branch(DeviceValidationLevel level)
        {
            using var agent = NewAgent(level);
            var device = BuildDeviceWithGenericDataItem();

            var raised = 0;
            agent.InvalidDataItemAdded += (_, _, _) => Interlocked.Increment(ref raised);

            var added = agent.AddDevice(device, initializeDataItems: false);

            switch (level)
            {
                case DeviceValidationLevel.Ignore:
                    Assert.That(raised, Is.EqualTo(0));
                    Assert.That(added, Is.Not.Null);
                    Assert.That(added!.DataItems!.Any(d => d.Id == GenericDataItemId), Is.True);
                    break;
                case DeviceValidationLevel.Warning:
                    Assert.That(raised, Is.EqualTo(1));
                    Assert.That(added, Is.Not.Null);
                    Assert.That(added!.DataItems!.Any(d => d.Id == GenericDataItemId), Is.True,
                        "Warning must retain the generic DataItem.");
                    break;
                case DeviceValidationLevel.Remove:
                    // F-TEST-BUG-2 fix (Device.cs:1017): `Device.RemoveDataItem(string)` now
                    // removes from `Device.DataItems` (the top-level collection) before
                    // descending into child Components — restoring the base
                    // `Component.RemoveDataItem` semantic the override previously lost.
                    Assert.That(raised, Is.EqualTo(1),
                        "Remove must raise InvalidDataItemAdded exactly once.");
                    Assert.That(added, Is.Not.Null,
                        "Remove must retain the device — only the generic DataItem is dropped.");
                    Assert.That(added!.DataItems!.Any(d => d.Id == GenericDataItemId), Is.False,
                        "Remove must drop the top-level generic DataItem from the Device (F-TEST-BUG-2 fix — Device.RemoveDataItem now covers the top-level collection).");
                    break;
                case DeviceValidationLevel.Strict:
                    Assert.That(raised, Is.EqualTo(1));
                    Assert.That(added, Is.Null, "Strict must null the device on the first invalid DataItem.");
                    break;
            }
        }

        // -----------------------------------------------------------------
        // Configuration contract — default value.
        // -----------------------------------------------------------------

        /// <summary>Pins the default value of <see cref="AgentConfiguration.DeviceValidationLevel"/>: freshly constructed AgentConfiguration must default to <see cref="DeviceValidationLevel.Warning"/> (AgentConfiguration.cs:164).</summary>
        [Test]
        public void AgentConfiguration_default_DeviceValidationLevel_is_Warning()
        {
            var config = new AgentConfiguration();

            Assert.That(config.DeviceValidationLevel, Is.EqualTo(DeviceValidationLevel.Warning),
                "The default must be Warning — Ignore would silently drop the invalid-device diagnostic that the "
                + "spec-conforming default warrants; Strict would reject devices from adapters that ship non-standard "
                + "type strings, breaking real-world onboarding.");
        }

        // -----------------------------------------------------------------
        // Enum-arm exhaustiveness — no arms added beyond the four covered.
        // -----------------------------------------------------------------

        /// <summary>Pins that <see cref="DeviceValidationLevel"/> has exactly four arms — Ignore, Warning, Remove, Strict — in that ordinal order. Adding a fifth arm without extending the switch grid above must fail this test and surface as a coverage-gap review item.</summary>
        [Test]
        public void DeviceValidationLevel_has_exactly_four_arms_in_documented_order()
        {
            var arms = Enum.GetValues(typeof(DeviceValidationLevel))
                .Cast<DeviceValidationLevel>()
                .ToArray();

            Assert.That(arms, Is.EqualTo(new[]
            {
                DeviceValidationLevel.Ignore,
                DeviceValidationLevel.Warning,
                DeviceValidationLevel.Remove,
                DeviceValidationLevel.Strict,
            }), "DeviceValidationLevel arms or ordinal order changed — extend the (arm × site) grid above to cover the new arm.");
        }

        // -----------------------------------------------------------------
        // Helpers.
        // -----------------------------------------------------------------

        private static MTConnectAgent NewAgent(DeviceValidationLevel level)
        {
            var config = new AgentConfiguration { DeviceValidationLevel = level };
            return new MTConnectAgent(config, uuid: "test-agent", initializeAgentDevice: false);
        }

        private static Device BuildDeviceWithGenericComponent()
        {
            var device = new Device
            {
                Id = "dev1",
                Uuid = DeviceUuid,
                Name = "dev1",
                Type = Device.TypeId,
            };
            var generic = new Component
            {
                Id = GenericComponentId,
                Name = GenericComponentId,
                Type = GenericComponentType,
            };
            device.AddComponent(generic);
            return device;
        }

        private static Device BuildDeviceWithGenericComposition()
        {
            var device = new Device
            {
                Id = "dev1",
                Uuid = DeviceUuid,
                Name = "dev1",
                Type = Device.TypeId,
            };
            var hostComponent = new Component
            {
                Id = "host",
                Name = "host",
                Type = "Axes",
            };
            hostComponent.AddComposition(new Composition
            {
                Id = GenericCompositionId,
                Name = GenericCompositionId,
                Type = GenericCompositionType,
            });
            device.AddComponent(hostComponent);
            return device;
        }

        private static Device BuildDeviceWithGenericDataItem()
        {
            var device = new Device
            {
                Id = "dev1",
                Uuid = DeviceUuid,
                Name = "dev1",
                Type = Device.TypeId,
            };
            device.AddDataItem(new DataItem
            {
                Id = GenericDataItemId,
                Type = GenericDataItemType,
                Category = DataItemCategory.EVENT,
            });
            return device;
        }

        private static System.Collections.Generic.IEnumerable<string> FirstChildComponentCompositionIds(IDevice device)
        {
            var components = device.GetComponents() ?? Array.Empty<IComponent>();
            var host = components.FirstOrDefault(c => c.Id == "host");
            if (host == null) return Array.Empty<string>();
            return host.Compositions?.Select(x => x.Id) ?? Array.Empty<string>();
        }

        private static System.Collections.Generic.IEnumerable<string> ComponentIds(IDevice device)
        {
            var components = device.GetComponents() ?? Array.Empty<IComponent>();
            return components.Select(c => c.Id);
        }
    }
}
