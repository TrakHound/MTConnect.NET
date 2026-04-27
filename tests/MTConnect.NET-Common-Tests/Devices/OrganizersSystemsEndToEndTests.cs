// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// End-to-end exercise of `Device.AddComponent()` against the full set of
// System substitution-group members. Verifies the auto-wrap behavior
// produces a symmetric tree shape — every auto-wrapped System peer lands
// under a single shared `<Systems>` organizer at equal depth, which is
// what `Probe` envelopes serialise out.
//
// Source (XSD): https://schemas.mtconnect.org/schemas/MTConnectDevices_*.xsd
//   `<xs:element name="Systems" ...>` is the organizer container; every
//   member of the `System` substitution-group is its child element on the
//   wire. The auto-wrap behavior in `Device.AddComponent()` exists
//   precisely to align programmatic device construction with the wire shape
//   declared by this XSD.
// Source (prose): https://docs.mtconnect.org/Part_3.0_DevicesInformationModel
//   §"Systems" — peer System members appear as siblings under `<Systems>`.

using System;
using System.Collections.Generic;
using System.Linq;
using MTConnect.Devices;
using MTConnect.Devices.Components;
using MTConnect.Tests.Common.TestHelpers;
using NUnit.Framework;

namespace MTConnect.Tests.Common.SystemsOrganizer
{
    [TestFixture]
    [Category("OrganizersSystemsEndToEnd")]
    public class OrganizersSystemsEndToEndTests
    {
        // Every auto-wrapped System member (Controller is excluded — it has its
        // own `Controllers` organizer per the Devices Information Model).
        private static readonly Type[] AutoWrappedSystemTypes = new[]
        {
            typeof(AirHandlerComponent),
            typeof(CoolantComponent),
            typeof(CoolingComponent),
            typeof(DielectricComponent),
            typeof(ElectricComponent),
            typeof(EnclosureComponent),
            typeof(EndEffectorComponent),
            typeof(FeederComponent),
            typeof(HeatingComponent),
            typeof(HydraulicComponent),
            typeof(LubricationComponent),
            typeof(PinToolComponent),
            typeof(PneumaticComponent),
            typeof(PressureComponent),
            typeof(ProcessPowerComponent),
            typeof(ProtectiveComponent),
            typeof(ToolHolderComponent),
            typeof(VacuumComponent),
            typeof(WorkEnvelopeComponent),
        };

        [Test]
        public void All_auto_wrapped_system_peers_land_under_single_Systems_organizer()
        {
            var device = new Device { Id = "d1", Name = "d1", Uuid = "D1" };

            foreach (var t in AutoWrappedSystemTypes)
            {
                device.AddComponent((IComponent)Activator.CreateInstance(t)!);
            }

            var systemsOrganizers = device.Components
                .Where(c => c.Type == SystemsComponent.TypeId)
                .ToArray();

            Assert.That(systemsOrganizers, Has.Length.EqualTo(1),
                "Every auto-wrapped System peer must share a single `<Systems>` organizer; " +
                $"got `{systemsOrganizers.Length}` organizers in the device tree.");

            var systemsOrganizer = systemsOrganizers.Single();
            var childTypeIds = systemsOrganizer.Components
                .Select(c => c.Type)
                .OrderBy(s => s, StringComparer.Ordinal)
                .ToArray();
            var expectedChildTypeIds = AutoWrappedSystemTypes
                .Select(t => (string)t.GetField("TypeId")!.GetValue(null)!)
                .OrderBy(s => s, StringComparer.Ordinal)
                .ToArray();

            Assert.That(childTypeIds, Is.EqualTo(expectedChildTypeIds),
                "The `<Systems>` organizer must enumerate every System peer as a direct child.");
        }

        [Test]
        public void Heating_and_Protective_share_Systems_organizer_after_separate_AddComponent_calls()
        {
            // Reproduces the exact symptom of TrakHound/MTConnect.NET#134:
            // two peer System components added one at a time must land
            // under the SAME `<Systems>` organizer, not at separate depths.
            var device = new Device { Id = "d1", Name = "d1", Uuid = "D1" };
            device.AddComponent(new HeatingComponent());
            device.AddComponent(new ProtectiveComponent());

            var directChildSystems = device.Components.Where(c => c.Type == SystemsComponent.TypeId).ToArray();
            Assert.That(directChildSystems, Has.Length.EqualTo(1),
                "Both `Heating` and `Protective` are System substitution-group members; " +
                "`Device.AddComponent()` must auto-wrap them under the same shared `<Systems>` organizer.");

            var children = directChildSystems.Single().Components
                .Select(c => c.Type)
                .OrderBy(s => s, StringComparer.Ordinal)
                .ToArray();
            Assert.That(children, Is.EqualTo(new[] { "Heating", "Protective" }));

            // Heating must NOT appear as a direct child of the Device — that would be the bug.
            Assert.That(device.Components.Any(c => c.Type == "Heating"), Is.False,
                "`Heating` must not appear as a direct child of the Device; " +
                "it is a System substitution-group member and must be auto-wrapped.");
        }

        [Test]
        public void Programmatic_device_assembly_produces_systems_at_consistent_depth()
        {
            var device = new Device { Id = "d1", Name = "d1", Uuid = "D1" };
            foreach (var t in AutoWrappedSystemTypes)
            {
                device.AddComponent((IComponent)Activator.CreateInstance(t)!);
            }

            var depths = AutoWrappedSystemTypes
                .Select(t => (string)t.GetField("TypeId")!.GetValue(null)!)
                .Select(typeId => ComponentDepthFinder.MeasureDepth(device.Components, typeId, 1))
                .Distinct()
                .ToArray();

            Assert.That(depths, Has.Length.EqualTo(1),
                "Every auto-wrapped System peer must sit at the same tree depth in the assembled device. " +
                $"Got distinct depths: [{string.Join(", ", depths)}].");
            Assert.That(depths[0], Is.EqualTo(2),
                "Auto-wrapped System peers must sit at depth 2 (Device → Systems → Member).");
        }
    }
}
