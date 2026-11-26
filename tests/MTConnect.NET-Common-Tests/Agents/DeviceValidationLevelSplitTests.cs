// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Linq;
using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Devices.DataItems;
using NUnit.Framework;

namespace MTConnect.Tests.Common.Agents
{
    /// <summary>
    /// Pins the split between <see cref="IAgentConfiguration.DeviceValidationLevel"/>
    /// (governs <c>MTConnectAgent.NormalizeDevice</c>: reaction to unknown Components,
    /// Compositions, and DataItems on an added Device) and
    /// <see cref="IAgentConfiguration.InputValidationLevel"/> (governs the per-observation
    /// input path). Before this PR the two axes shared a single enum, so integrators could
    /// not run <c>InputValidationLevel = Strict</c> alongside a permissive device model. The
    /// fixture proves both invariants:
    /// <list type="bullet">
    ///   <item>Each <see cref="DeviceValidationLevel"/> arm (Ignore, Warning, Remove, Strict)
    ///     drives <c>NormalizeDevice</c>'s reaction to a generic (unknown-type) Component,
    ///     Composition, and DataItem exactly as documented.</item>
    ///   <item>The two axes are independent: an <c>InputValidationLevel = Strict</c>
    ///     configuration with <c>DeviceValidationLevel = Ignore</c> still accepts a Device
    ///     whose model contains generic entities, and vice versa a
    ///     <c>DeviceValidationLevel = Strict</c> with <c>InputValidationLevel = Ignore</c>
    ///     rejects the same Device.</item>
    ///   <item>Both configuration properties default to <see cref="DeviceValidationLevel.Warning"/>
    ///     / <see cref="InputValidationLevel.Warning"/>, matching the pre-split single-axis
    ///     default so existing integrators are unaffected.</item>
    /// </list>
    /// </summary>
    [TestFixture]
    [Category("DeviceValidationLevelSplit")]
    public class DeviceValidationLevelSplitTests
    {
        private const string DeviceUuid = "u-devvalid";
        private const string DeviceId = "d-devvalid";


        // -------------------------------------------------------------------- //
        // Configuration defaults + independence                                //
        // -------------------------------------------------------------------- //

        /// <summary>Configuration defaults preserve pre-split behaviour: both axes are Warning.</summary>
        [Test]
        public void AgentConfiguration_Defaults_Both_Axes_To_Warning()
        {
            var config = new AgentConfiguration();

            Assert.That(config.DeviceValidationLevel, Is.EqualTo(DeviceValidationLevel.Warning),
                "DeviceValidationLevel must default to Warning so existing single-axis integrators are unaffected");
            Assert.That(config.InputValidationLevel, Is.EqualTo(InputValidationLevel.Warning),
                "InputValidationLevel must default to Warning so existing single-axis integrators are unaffected");
        }

        /// <summary>The two validation axes can hold different values simultaneously - the whole point of the split.</summary>
        [Test]
        public void AgentConfiguration_Two_Axes_Can_Hold_Different_Values()
        {
            var config = new AgentConfiguration
            {
                DeviceValidationLevel = DeviceValidationLevel.Ignore,
                InputValidationLevel = InputValidationLevel.Strict,
            };

            Assert.That(config.DeviceValidationLevel, Is.EqualTo(DeviceValidationLevel.Ignore));
            Assert.That(config.InputValidationLevel, Is.EqualTo(InputValidationLevel.Strict));
        }


        // -------------------------------------------------------------------- //
        // DeviceValidationLevel arms x invalid Component                       //
        // -------------------------------------------------------------------- //

        /// <summary>DeviceValidationLevel = Ignore accepts a Device carrying a generic (unknown-type) Component unchanged.</summary>
        [Test]
        public void InvalidComponent_DeviceValidationLevel_Ignore_Accepts_Device_Untouched()
        {
            using var agent = NewAgent(DeviceValidationLevel.Ignore, InputValidationLevel.Ignore);
            var device = NewDeviceWithGenericComponent();

            var added = agent.AddDevice(device, initializeDataItems: false);

            Assert.That(added, Is.Not.Null, "Ignore must accept the Device");
            Assert.That(added.Components?.Any(c => c.Id == "generic-comp-1"), Is.True,
                "Ignore must leave the generic Component in place");
        }

        /// <summary>DeviceValidationLevel = Warning accepts the Device with the generic Component preserved.</summary>
        [Test]
        public void InvalidComponent_DeviceValidationLevel_Warning_Accepts_Device_Preserves_Component()
        {
            using var agent = NewAgent(DeviceValidationLevel.Warning, InputValidationLevel.Ignore);
            var device = NewDeviceWithGenericComponent();

            var added = agent.AddDevice(device, initializeDataItems: false);

            Assert.That(added, Is.Not.Null, "Warning must not reject the Device");
            Assert.That(added.Components?.Any(c => c.Id == "generic-comp-1"), Is.True,
                "Warning must not silently drop the generic Component");
        }

        /// <summary>DeviceValidationLevel = Remove accepts the Device but drops the generic Component.</summary>
        [Test]
        public void InvalidComponent_DeviceValidationLevel_Remove_Drops_Component_Keeps_Device()
        {
            using var agent = NewAgent(DeviceValidationLevel.Remove, InputValidationLevel.Ignore);
            var device = NewDeviceWithGenericComponent();

            var added = agent.AddDevice(device, initializeDataItems: false);

            Assert.That(added, Is.Not.Null, "Remove must accept the Device");
            Assert.That(added.Components?.Any(c => c.Id == "generic-comp-1"), Is.Not.True,
                "Remove must drop the generic Component from the accepted Device");
        }

        /// <summary>DeviceValidationLevel = Strict rejects the entire Device on the first generic Component.</summary>
        [Test]
        public void InvalidComponent_DeviceValidationLevel_Strict_Rejects_Entire_Device()
        {
            using var agent = NewAgent(DeviceValidationLevel.Strict, InputValidationLevel.Ignore);
            var device = NewDeviceWithGenericComponent();

            var added = agent.AddDevice(device, initializeDataItems: false);

            Assert.That(added, Is.Null,
                "Strict MUST reject the entire Device on the first generic Component");
        }


        // -------------------------------------------------------------------- //
        // DeviceValidationLevel arms x invalid Composition                     //
        // -------------------------------------------------------------------- //

        /// <summary>DeviceValidationLevel = Strict rejects a Device carrying a generic (unknown-type) Composition.</summary>
        [Test]
        public void InvalidComposition_DeviceValidationLevel_Strict_Rejects_Entire_Device()
        {
            using var agent = NewAgent(DeviceValidationLevel.Strict, InputValidationLevel.Ignore);
            var device = NewDeviceWithGenericComposition();

            var added = agent.AddDevice(device, initializeDataItems: false);

            Assert.That(added, Is.Null,
                "Strict MUST reject the entire Device on the first generic Composition");
        }

        /// <summary>DeviceValidationLevel = Remove drops the generic Composition but accepts the rest of the Device.</summary>
        [Test]
        public void InvalidComposition_DeviceValidationLevel_Remove_Drops_Composition_Keeps_Device()
        {
            using var agent = NewAgent(DeviceValidationLevel.Remove, InputValidationLevel.Ignore);
            var device = NewDeviceWithGenericComposition();

            var added = agent.AddDevice(device, initializeDataItems: false);

            Assert.That(added, Is.Not.Null, "Remove must accept the Device");
            Assert.That(added.Compositions?.Any(x => x.Id == "generic-composition-1"), Is.Not.True,
                "Remove must drop the generic Composition from the accepted Device");
        }


        // -------------------------------------------------------------------- //
        // DeviceValidationLevel arms x invalid DataItem                        //
        // -------------------------------------------------------------------- //

        /// <summary>DeviceValidationLevel = Strict rejects a Device carrying a generic (unknown-type) DataItem.</summary>
        [Test]
        public void InvalidDataItem_DeviceValidationLevel_Strict_Rejects_Entire_Device()
        {
            using var agent = NewAgent(DeviceValidationLevel.Strict, InputValidationLevel.Ignore);
            var device = NewDeviceWithGenericDataItem();

            var added = agent.AddDevice(device, initializeDataItems: false);

            Assert.That(added, Is.Null,
                "Strict MUST reject the entire Device on the first generic DataItem");
        }

        /// <summary>DeviceValidationLevel = Remove drops the generic DataItem but accepts the rest of the Device.</summary>
        [Test]
        public void InvalidDataItem_DeviceValidationLevel_Remove_Drops_DataItem_Keeps_Device()
        {
            using var agent = NewAgent(DeviceValidationLevel.Remove, InputValidationLevel.Ignore);
            var device = NewDeviceWithGenericDataItemOnChildComponent();

            var added = agent.AddDevice(device, initializeDataItems: false);

            Assert.That(added, Is.Not.Null, "Remove must accept the Device");
            var allDataItems = added.GetDataItems() ?? Enumerable.Empty<IDataItem>();
            Assert.That(allDataItems.Any(d => d.Id == "generic-di-1"), Is.False,
                "Remove must drop the generic DataItem from the accepted Device (including sub-Components)");
        }


        // -------------------------------------------------------------------- //
        // Independence: DeviceValidationLevel does not depend on               //
        // InputValidationLevel and vice versa                                  //
        // -------------------------------------------------------------------- //

        /// <summary>
        /// InputValidationLevel = Strict does NOT trigger device rejection - the split
        /// isolates the observation-input axis from the device-shape axis.
        /// </summary>
        [Test]
        public void InputValidationLevel_Strict_Does_Not_Reject_Device_With_Generic_Component()
        {
            using var agent = NewAgent(DeviceValidationLevel.Ignore, InputValidationLevel.Strict);
            var device = NewDeviceWithGenericComponent();

            var added = agent.AddDevice(device, initializeDataItems: false);

            Assert.That(added, Is.Not.Null,
                "InputValidationLevel = Strict with DeviceValidationLevel = Ignore MUST NOT reject the Device");
            Assert.That(added.Components?.Any(c => c.Id == "generic-comp-1"), Is.True,
                "the split makes InputValidationLevel irrelevant to device-shape validation");
        }

        /// <summary>
        /// DeviceValidationLevel = Strict rejects regardless of a permissive InputValidationLevel -
        /// the two axes truly separate.
        /// </summary>
        [Test]
        public void DeviceValidationLevel_Strict_Rejects_Even_When_InputValidationLevel_Is_Ignore()
        {
            using var agent = NewAgent(DeviceValidationLevel.Strict, InputValidationLevel.Ignore);
            var device = NewDeviceWithGenericComponent();

            var added = agent.AddDevice(device, initializeDataItems: false);

            Assert.That(added, Is.Null,
                "DeviceValidationLevel = Strict must reject regardless of a permissive InputValidationLevel");
        }


        // -------------------------------------------------------------------- //
        // Helpers                                                              //
        // -------------------------------------------------------------------- //

        private static MTConnectAgentBroker NewAgent(
            DeviceValidationLevel deviceLevel,
            InputValidationLevel inputLevel)
        {
            var config = new AgentConfiguration
            {
                DeviceValidationLevel = deviceLevel,
                InputValidationLevel = inputLevel,
            };
            var agent = new MTConnectAgentBroker(config, initializeAgentDevice: false);
            agent.Start();
            return agent;
        }

        private static Device NewDeviceWithGenericComponent()
        {
            var device = new Device
            {
                Id = DeviceId,
                Name = DeviceId,
                Uuid = DeviceUuid,
            };

            // Generic Component: Type is unknown to the SDK, so it will be treated as an invalid component by NormalizeDevice.
            var genericComponent = new Component
            {
                Id = "generic-comp-1",
                Name = "generic-comp-1",
                Type = "UNKNOWN_COMPONENT_TYPE",
            };
            device.AddComponent(genericComponent);

            return device;
        }

        private static Device NewDeviceWithGenericComposition()
        {
            var device = new Device
            {
                Id = DeviceId,
                Name = DeviceId,
                Uuid = DeviceUuid,
            };

            // Attach the generic Composition directly to the Device so NormalizeDevice's
            // obj.RemoveComposition(id) call reaches it. Placing the Composition on a sub-Component
            // would exercise a different (non-recursive) removal path and would not exhibit the
            // "Remove strips it" contract.
            device.AddComposition(new Composition
            {
                Id = "generic-composition-1",
                Name = "generic-composition-1",
                Type = "UNKNOWN_COMPOSITION_TYPE",
            });

            return device;
        }

        private static Device NewDeviceWithGenericDataItem()
        {
            var device = new Device
            {
                Id = DeviceId,
                Name = DeviceId,
                Uuid = DeviceUuid,
            };

            device.AddDataItem(new DataItem
            {
                Id = "generic-di-1",
                Name = "generic-di-1",
                Category = DataItemCategory.EVENT,
                Type = "UNKNOWN_EVENT_TYPE",
                Representation = DataItemRepresentation.VALUE,
            });

            return device;
        }

        private static Device NewDeviceWithGenericDataItemOnChildComponent()
        {
            var device = new Device
            {
                Id = DeviceId,
                Name = DeviceId,
                Uuid = DeviceUuid,
            };

            // Attach the generic DataItem to a valid child Component, so the removal is
            // exercised through the Component traversal of obj.RemoveDataItem(id).
            var host = new MTConnect.Devices.Components.LinearComponent
            {
                Id = "host-linear",
                Name = "host-linear",
            };
            host.AddDataItem(new DataItem
            {
                Id = "generic-di-1",
                Name = "generic-di-1",
                Category = DataItemCategory.EVENT,
                Type = "UNKNOWN_EVENT_TYPE",
                Representation = DataItemRepresentation.VALUE,
            });
            device.AddComponent(host);

            return device;
        }
    }
}
