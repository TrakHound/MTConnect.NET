// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Devices.Components;
using MTConnect.Devices.DataItems;
using NUnit.Framework;

namespace MTConnect.Tests.Common.Agents
{
    /// <summary>
    /// Enum-arm coverage FLOOR for <see cref="DeviceValidationLevel"/> as consumed
    /// by <c>MTConnectAgent.NormalizeDevice</c>. The 2026-06 PR that split Device
    /// validation off <see cref="InputValidationLevel"/> introduced 12 branch
    /// combinations that all executed against ONE test path (via
    /// <see cref="InputValidationLevel"/>) — see
    /// <see cref="AddObservationEmptyResultCoerceTests"/>. This fixture pins every
    /// arm of the four-value enum against each of the three generic-entity
    /// validation sites so a regression that widens, narrows, or reorders the
    /// enum is caught the moment the delta lands.
    ///
    /// Grid: 4 arms (Ignore / Warning / Remove / Strict) x 3 sites (generic
    /// Component / generic Composition / generic DataItem) = 12 base cases,
    /// plus the default-configuration invariant and the enum-arms-are-what-we-
    /// think-they-are guard.
    ///
    /// A "generic" entity is one whose runtime CLR type is the raw
    /// <see cref="Component"/> / <see cref="Composition"/> / <see cref="DataItem"/>
    /// base class rather than a concrete subclass — this is what the
    /// NormalizeDevice validation loop identifies via
    /// <c>o.GetType() == typeof(Component)</c> as "invalid type not found".
    /// Setting <c>Type</c> to a string the registry does not recognize causes
    /// the corresponding Create factory to fall back to the raw base class.
    /// </summary>
    [TestFixture]
    [Category("DeviceValidationLevel")]
    public class DeviceValidationLevelNormalizeDeviceTests
    {
        private const string DeviceUuid = "device-validation-level-device";
        private const string DeviceName = "DeviceValidationLevel";
        private const string DeviceId = "device-validation-level-device-id";

        // Unrecognized Type strings force each Create factory (Component /
        // Composition / DataItem) into its base-class fallback, which is
        // exactly the "generic" entity NormalizeDevice validates against.
        private const string UnknownComponentType = "ThisComponentTypeIsNotRegistered";
        private const string UnknownCompositionType = "ThisCompositionTypeIsNotRegistered";
        private const string UnknownDataItemType = "THIS_DATAITEM_TYPE_IS_NOT_REGISTERED";

        private const string ChildComponentId = "child-generic-component";
        private const string ChildCompositionId = "child-generic-composition";
        private const string ChildDataItemId = "child-generic-dataitem";

        // ---------------------------------------------------------------
        // Enum-shape guard: pin the arms + ordinals so a rename or reorder
        // fails loudly rather than silently shifting the arms consumed by
        // NormalizeDevice's `> Ignore`, `== Remove`, `== Strict` predicates.
        // ---------------------------------------------------------------

        /// <summary>Pins the four arms of <see cref="DeviceValidationLevel"/> at their exact ordinals: Ignore=0, Warning=1, Remove=2, Strict=3.</summary>
        [Test]
        public void DeviceValidationLevel_arms_and_ordinals_are_stable()
        {
            var arms = Enum.GetValues(typeof(DeviceValidationLevel))
                .Cast<DeviceValidationLevel>()
                .OrderBy(v => (int)v)
                .ToArray();

            Assert.That(arms, Is.EqualTo(new[]
            {
                DeviceValidationLevel.Ignore,
                DeviceValidationLevel.Warning,
                DeviceValidationLevel.Remove,
                DeviceValidationLevel.Strict,
            }));
            Assert.That((int)DeviceValidationLevel.Ignore, Is.EqualTo(0));
            Assert.That((int)DeviceValidationLevel.Warning, Is.EqualTo(1));
            Assert.That((int)DeviceValidationLevel.Remove, Is.EqualTo(2));
            Assert.That((int)DeviceValidationLevel.Strict, Is.EqualTo(3));
        }

        /// <summary>Pins the AgentConfiguration default: <see cref="AgentConfiguration.DeviceValidationLevel"/> is <see cref="DeviceValidationLevel.Warning"/>.</summary>
        [Test]
        public void AgentConfiguration_default_DeviceValidationLevel_is_Warning()
        {
            var config = new AgentConfiguration();

            Assert.That(config.DeviceValidationLevel, Is.EqualTo(DeviceValidationLevel.Warning),
                "Warning is the spec-safe default: the invalid entity survives but " +
                "a subscriber is notified. Any change to this default is a behavior break.");
        }

        // ---------------------------------------------------------------
        // Generic Component site — 4 arms
        // ---------------------------------------------------------------

        /// <summary>Ignore: no InvalidComponentAdded event; the generic Component is retained on the device.</summary>
        [Test]
        public void NormalizeDevice_GenericComponent_Under_Ignore_Retains_Component_And_Suppresses_Event()
        {
            var raised = new List<string>();
            using var agent = NewAgent(DeviceValidationLevel.Ignore);
            agent.InvalidComponentAdded += (_, c, _) => raised.Add(c.Id);

            var device = DeviceWithGenericComponent();

            var added = agent.AddDevice(device);

            Assert.That(added, Is.Not.Null, "Ignore never invalidates the device");
            Assert.That(added.Components.Any(c => c.Id == ChildComponentId), Is.True,
                "Ignore never removes the generic Component");
            Assert.That(raised, Is.Empty,
                "Ignore short-circuits before the InvalidComponentAdded raise site");
        }

        /// <summary>Warning: InvalidComponentAdded fires; the generic Component is retained on the device.</summary>
        [Test]
        public void NormalizeDevice_GenericComponent_Under_Warning_Retains_Component_And_Raises_Event()
        {
            var raised = new List<string>();
            using var agent = NewAgent(DeviceValidationLevel.Warning);
            agent.InvalidComponentAdded += (_, c, _) => raised.Add(c.Id);

            var device = DeviceWithGenericComponent();

            var added = agent.AddDevice(device);

            Assert.That(added, Is.Not.Null);
            Assert.That(added.Components.Any(c => c.Id == ChildComponentId), Is.True,
                "Warning notifies but does not mutate the device");
            Assert.That(raised, Is.EqualTo(new[] { ChildComponentId }));
        }

        /// <summary>Remove: InvalidComponentAdded fires; the generic Component is removed from the device.</summary>
        [Test]
        public void NormalizeDevice_GenericComponent_Under_Remove_Drops_Component_And_Raises_Event()
        {
            var raised = new List<string>();
            using var agent = NewAgent(DeviceValidationLevel.Remove);
            agent.InvalidComponentAdded += (_, c, _) => raised.Add(c.Id);

            var device = DeviceWithGenericComponent();

            var added = agent.AddDevice(device);

            Assert.That(added, Is.Not.Null, "Remove keeps the device; only the invalid child is dropped");
            Assert.That(added.Components.Any(c => c.Id == ChildComponentId), Is.False,
                "Remove must drop the generic Component from the normalized device");
            Assert.That(raised, Is.EqualTo(new[] { ChildComponentId }));
        }

        /// <summary>Strict: InvalidComponentAdded fires; NormalizeDevice returns null so AddDevice rejects the entire device.</summary>
        [Test]
        public void NormalizeDevice_GenericComponent_Under_Strict_Rejects_Device_And_Raises_Event()
        {
            var raised = new List<string>();
            using var agent = NewAgent(DeviceValidationLevel.Strict);
            agent.InvalidComponentAdded += (_, c, _) => raised.Add(c.Id);

            var device = DeviceWithGenericComponent();

            var added = agent.AddDevice(device);

            Assert.That(added, Is.Null,
                "Strict must reject the entire device on the first invalid Component");
            Assert.That(raised, Is.EqualTo(new[] { ChildComponentId }),
                "Strict raises the event before returning null so subscribers can log");
        }

        // ---------------------------------------------------------------
        // Generic Composition site — 4 arms
        // ---------------------------------------------------------------

        /// <summary>Ignore: no InvalidCompositionAdded event; the generic Composition is retained on the device.</summary>
        [Test]
        public void NormalizeDevice_GenericComposition_Under_Ignore_Retains_Composition_And_Suppresses_Event()
        {
            var raised = new List<string>();
            using var agent = NewAgent(DeviceValidationLevel.Ignore);
            agent.InvalidCompositionAdded += (_, c, _) => raised.Add(c.Id);

            var device = DeviceWithGenericComposition();

            var added = agent.AddDevice(device);

            Assert.That(added, Is.Not.Null);
            Assert.That(added.Compositions.Any(c => c.Id == ChildCompositionId), Is.True);
            Assert.That(raised, Is.Empty);
        }

        /// <summary>Warning: InvalidCompositionAdded fires; the generic Composition is retained.</summary>
        [Test]
        public void NormalizeDevice_GenericComposition_Under_Warning_Retains_Composition_And_Raises_Event()
        {
            var raised = new List<string>();
            using var agent = NewAgent(DeviceValidationLevel.Warning);
            agent.InvalidCompositionAdded += (_, c, _) => raised.Add(c.Id);

            var device = DeviceWithGenericComposition();

            var added = agent.AddDevice(device);

            Assert.That(added, Is.Not.Null);
            Assert.That(added.Compositions.Any(c => c.Id == ChildCompositionId), Is.True);
            Assert.That(raised, Is.EqualTo(new[] { ChildCompositionId }));
        }

        /// <summary>Remove: InvalidCompositionAdded fires; the generic Composition is removed from the device.</summary>
        [Test]
        public void NormalizeDevice_GenericComposition_Under_Remove_Drops_Composition_And_Raises_Event()
        {
            var raised = new List<string>();
            using var agent = NewAgent(DeviceValidationLevel.Remove);
            agent.InvalidCompositionAdded += (_, c, _) => raised.Add(c.Id);

            var device = DeviceWithGenericComposition();

            var added = agent.AddDevice(device);

            Assert.That(added, Is.Not.Null);
            Assert.That(added.Compositions.Any(c => c.Id == ChildCompositionId), Is.False);
            Assert.That(raised, Is.EqualTo(new[] { ChildCompositionId }));
        }

        /// <summary>Strict: InvalidCompositionAdded fires; NormalizeDevice returns null.</summary>
        [Test]
        public void NormalizeDevice_GenericComposition_Under_Strict_Rejects_Device_And_Raises_Event()
        {
            var raised = new List<string>();
            using var agent = NewAgent(DeviceValidationLevel.Strict);
            agent.InvalidCompositionAdded += (_, c, _) => raised.Add(c.Id);

            var device = DeviceWithGenericComposition();

            var added = agent.AddDevice(device);

            Assert.That(added, Is.Null);
            Assert.That(raised, Is.EqualTo(new[] { ChildCompositionId }));
        }

        // ---------------------------------------------------------------
        // Generic DataItem site — 4 arms
        // ---------------------------------------------------------------

        /// <summary>Ignore: no InvalidDataItemAdded event; the generic DataItem is retained on the device.</summary>
        [Test]
        public void NormalizeDevice_GenericDataItem_Under_Ignore_Retains_DataItem_And_Suppresses_Event()
        {
            var raised = new List<string>();
            using var agent = NewAgent(DeviceValidationLevel.Ignore);
            agent.InvalidDataItemAdded += (_, d, _) => raised.Add(d.Id);

            var device = DeviceWithGenericDataItem();

            var added = agent.AddDevice(device);

            Assert.That(added, Is.Not.Null);
            Assert.That(added.GetDataItems().Any(d => d.Id == ChildDataItemId), Is.True);
            Assert.That(raised, Is.Empty);
        }

        /// <summary>Warning: InvalidDataItemAdded fires; the generic DataItem is retained.</summary>
        [Test]
        public void NormalizeDevice_GenericDataItem_Under_Warning_Retains_DataItem_And_Raises_Event()
        {
            var raised = new List<string>();
            using var agent = NewAgent(DeviceValidationLevel.Warning);
            agent.InvalidDataItemAdded += (_, d, _) => raised.Add(d.Id);

            var device = DeviceWithGenericDataItem();

            var added = agent.AddDevice(device);

            Assert.That(added, Is.Not.Null);
            Assert.That(added.GetDataItems().Any(d => d.Id == ChildDataItemId), Is.True);
            Assert.That(raised, Is.EqualTo(new[] { ChildDataItemId }));
        }

        /// <summary>Remove: InvalidDataItemAdded fires; the generic DataItem is removed from the device.</summary>
        [Test]
        public void NormalizeDevice_GenericDataItem_Under_Remove_Drops_DataItem_And_Raises_Event()
        {
            var raised = new List<string>();
            using var agent = NewAgent(DeviceValidationLevel.Remove);
            agent.InvalidDataItemAdded += (_, d, _) => raised.Add(d.Id);

            var device = DeviceWithGenericDataItem();

            var added = agent.AddDevice(device);

            Assert.That(added, Is.Not.Null);
            Assert.That(added.GetDataItems().Any(d => d.Id == ChildDataItemId), Is.False);
            Assert.That(raised, Is.EqualTo(new[] { ChildDataItemId }));
        }

        /// <summary>Strict: InvalidDataItemAdded fires; NormalizeDevice returns null.</summary>
        [Test]
        public void NormalizeDevice_GenericDataItem_Under_Strict_Rejects_Device_And_Raises_Event()
        {
            var raised = new List<string>();
            using var agent = NewAgent(DeviceValidationLevel.Strict);
            agent.InvalidDataItemAdded += (_, d, _) => raised.Add(d.Id);

            var device = DeviceWithGenericDataItem();

            var added = agent.AddDevice(device);

            Assert.That(added, Is.Null);
            Assert.That(raised, Is.EqualTo(new[] { ChildDataItemId }));
        }

        // ---------------------------------------------------------------
        // Subscriber-payload invariant (covers the Raise-site tuple)
        // ---------------------------------------------------------------

        /// <summary>Pins the subscriber tuple: (deviceUuid, IComponent, ValidationResult) with a non-empty message.</summary>
        [Test]
        public void InvalidComponentAdded_Subscriber_Receives_DeviceUuid_Entity_And_ValidationResult()
        {
            (string uuid, IComponent component, ValidationResult result)? captured = null;
            using var agent = NewAgent(DeviceValidationLevel.Warning);
            agent.InvalidComponentAdded += (u, c, r) => captured = (u, c, r);

            agent.AddDevice(DeviceWithGenericComponent());

            Assert.That(captured, Is.Not.Null);
            Assert.That(captured!.Value.uuid, Is.EqualTo(DeviceUuid));
            Assert.That(captured!.Value.component.Id, Is.EqualTo(ChildComponentId));
            Assert.That(captured!.Value.result.IsValid, Is.False);
            Assert.That(captured!.Value.result.Message, Does.Contain(UnknownComponentType),
                "Message must name the offending Component Type so subscribers can log it");
        }

        // ---------------------------------------------------------------
        // Cross-cutting: setting DeviceValidationLevel does NOT change
        // InputValidationLevel behavior and vice versa. The pre-PR
        // implementation collapsed the two on InputValidationLevel; this
        // assertion pins the split.
        // ---------------------------------------------------------------

        /// <summary>Pins the split: InputValidationLevel.Ignore + DeviceValidationLevel.Strict still rejects the device on a generic Component.</summary>
        [Test]
        public void DeviceValidationLevel_Is_Independent_Of_InputValidationLevel()
        {
            var config = new AgentConfiguration
            {
                InputValidationLevel = InputValidationLevel.Ignore,
                DeviceValidationLevel = DeviceValidationLevel.Strict,
            };
            using var agent = new MTConnectAgent(config, uuid: "split-test-agent", initializeAgentDevice: false);

            var added = agent.AddDevice(DeviceWithGenericComponent());

            Assert.That(added, Is.Null,
                "Post-split, DeviceValidationLevel.Strict rejects the device regardless of " +
                "InputValidationLevel — the two knobs no longer share state.");
        }

        // ---------------------------------------------------------------
        // Fixture harness
        // ---------------------------------------------------------------

        private static MTConnectAgent NewAgent(DeviceValidationLevel level)
        {
            var config = new AgentConfiguration
            {
                DeviceValidationLevel = level,
                // Keep InputValidationLevel at its default so nothing else in the
                // observation-side pipeline participates in this fixture's assertions.
                InputValidationLevel = InputValidationLevel.Warning,
            };
            return new MTConnectAgent(config, uuid: "device-validation-level-agent", initializeAgentDevice: false);
        }

        private static Device DeviceWithGenericComponent()
        {
            var device = NewDevice();
            device.AddDataItem(new AvailabilityDataItem(DeviceId));
            device.AddComponent(new Component
            {
                Id = ChildComponentId,
                Uuid = ChildComponentId,
                Name = ChildComponentId,
                Type = UnknownComponentType,
            });
            return device;
        }

        private static Device DeviceWithGenericComposition()
        {
            var device = NewDevice();
            device.AddDataItem(new AvailabilityDataItem(DeviceId));
            device.AddComposition(new Composition
            {
                Id = ChildCompositionId,
                Uuid = ChildCompositionId,
                Name = ChildCompositionId,
                Type = UnknownCompositionType,
            });
            return device;
        }

        private static Device DeviceWithGenericDataItem()
        {
            var device = NewDevice();
            device.AddDataItem(new AvailabilityDataItem(DeviceId));
            // NormalizeDevice's Remove branch invokes Device.RemoveDataItem,
            // which iterates Device.Components (not Device.DataItems). Attach
            // the generic DataItem to a known Axes container so the Remove
            // arm has an addressable removal target — pins the Remove arm on
            // the actually-working code path.
            var axes = new AxesComponent { Id = "axes-1" };
            axes.AddDataItem(new DataItem
            {
                Id = ChildDataItemId,
                Name = ChildDataItemId,
                Type = UnknownDataItemType,
                Category = DataItemCategory.EVENT,
            });
            device.AddComponent(axes);
            return device;
        }

        private static Device NewDevice()
        {
            return new Device
            {
                Id = DeviceId,
                Uuid = DeviceUuid,
                Name = DeviceName,
                Type = Device.TypeId,
            };
        }
    }
}
