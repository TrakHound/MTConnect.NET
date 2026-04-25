// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// Pins MTConnect Standard issue TrakHound/MTConnect.NET#134:
// `Organizers.Systems` must enumerate every member of the `System`
// substitution-group declared by the SysML model so that
// `Device.AddComponent()` auto-wraps every System peer into the
// `<Systems>` organizer at equal tree depth.
//
// Source (XMI): https://github.com/mtconnect/mtconnect_sysml_model
//   The `System` UML class is the parent of the substitution-group;
//   members are descendants whose summary description begins with
//   "System that ..." or "System composed of ..." (canonical SysML
//   phrasing used for `System` descendants on every published version
//   v1.1 through v2.5).
// Source (XSD): https://schemas.mtconnect.org/schemas/MTConnectDevices_*.xsd
//   Each `<xs:element>` whose `substitutionGroup="System"` is a member.
// Source (prose): https://docs.mtconnect.org/Part_3.0_DevicesInformationModel
//   §"Systems" organizer lists the member elements per published version.

using System;
using System.Collections.Generic;
using System.Linq;
using MTConnect.Devices;
using MTConnect.Devices.Components;
using NUnit.Framework;

namespace MTConnect.Tests.Common.SystemsOrganizer
{
    [TestFixture]
    [Category("OrganizersSystemsSubstitutionGroup")]
    public class OrganizersSystemsTests
    {
        // System substitution-group members per the SysML model materialized in
        // `libraries/MTConnect.NET-Common/Devices/Components/*.g.cs` on this
        // revision. Sorted alphabetically by `TypeId` for stability.
        private static readonly Type[] SystemMemberComponentTypes = new[]
        {
            typeof(AirHandlerComponent),
            typeof(ControllerComponent),
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

        public static IEnumerable<string> KnownSystemMembers =>
            SystemMemberComponentTypes.Select(t => GetTypeIdFromComponent(t));

        // Every System substitution-group member resolves through
        // `Organizers.GetOrganizerType()` to `Systems` so
        // `Device.AddComponent()` auto-wraps it; the only exception is
        // `Controller`, which `Device.AddComponent()` deliberately leaves
        // at the device root (see `Device.cs` near the
        // `organizerType != ControllersComponent.TypeId` guard).
        public static IEnumerable<Type> AutoWrappedSystemMemberTypes =>
            SystemMemberComponentTypes.Where(t => t != typeof(ControllerComponent));

        // Pairs the issue calls out plus a few representative peers; every
        // pair is expected to land at equal tree depth after
        // `Device.AddComponent()`.
        public static IEnumerable<(Type Left, Type Right)> EqualDepthPeerPairs =>
            new[]
            {
                (typeof(HeatingComponent),    typeof(ProtectiveComponent)),  // issue #134
                (typeof(HeatingComponent),    typeof(CoolingComponent)),
                (typeof(ElectricComponent),   typeof(HydraulicComponent)),
                (typeof(PressureComponent),   typeof(VacuumComponent)),
                (typeof(AirHandlerComponent), typeof(EnclosureComponent)),
            };

        [TestCaseSource(nameof(KnownSystemMembers))]
        public void System_substitution_group_member_listed_in_Organizers_Systems(string typeId)
        {
            Assert.That(Organizers.Systems, Has.Member(typeId),
                $"`{typeId}` is a SysML `System` substitution-group member; " +
                $"`Organizers.Systems` must enumerate it so `Device.AddComponent()` auto-wraps it under `<Systems>`.");
        }

        [TestCaseSource(nameof(KnownSystemMembers))]
        public void GetOrganizerType_for_system_member_returns_Systems(string typeId)
        {
            Assert.That(Organizers.GetOrganizerType(typeId),
                Is.EqualTo(SystemsComponent.TypeId),
                $"`Organizers.GetOrganizerType(\"{typeId}\")` must resolve to `Systems` so " +
                $"the auto-wrap path in `Device.AddComponent()` fires.");
        }

        [TestCaseSource(nameof(EqualDepthPeerPairs))]
        public void Peer_system_components_sit_at_equal_depth_after_AddComponent(
            (Type Left, Type Right) pair)
        {
            var device = new Device { Id = "d1", Name = "d1", Uuid = "D1" };
            device.AddComponent((IComponent)Activator.CreateInstance(pair.Left)!);
            device.AddComponent((IComponent)Activator.CreateInstance(pair.Right)!);

            var leftDepth = MeasureDepth(device, pair.Left);
            var rightDepth = MeasureDepth(device, pair.Right);

            Assert.That(leftDepth, Is.EqualTo(rightDepth),
                $"Peer System components `{pair.Left.Name}` and `{pair.Right.Name}` must sit at equal " +
                $"tree depth after `Device.AddComponent()` — both should auto-wrap under `<Systems>`. " +
                $"Got `{pair.Left.Name}` depth={leftDepth}, `{pair.Right.Name}` depth={rightDepth}.");
        }

        [TestCaseSource(nameof(AutoWrappedSystemMemberTypes))]
        public void Auto_wrapped_system_member_lands_under_Systems_organizer(Type componentType)
        {
            var device = new Device { Id = "d1", Name = "d1", Uuid = "D1" };
            device.AddComponent((IComponent)Activator.CreateInstance(componentType)!);

            var typeId = GetTypeIdFromComponent(componentType);
            var systemsOrganizer = device.Components?.FirstOrDefault(
                c => c.Type == SystemsComponent.TypeId);

            Assert.That(systemsOrganizer, Is.Not.Null,
                $"Adding a `{componentType.Name}` to a Device must produce a `<Systems>` organizer " +
                $"as a direct child of the Device.");
            Assert.That(systemsOrganizer!.Components, Is.Not.Null);
            Assert.That(systemsOrganizer.Components.Any(c => c.Type == typeId), Is.True,
                $"The `{typeId}` component must appear under the auto-created `<Systems>` organizer, " +
                $"not as a direct child of the Device.");
        }

        private static int MeasureDepth(Device device, Type targetComponentType)
        {
            var targetTypeId = GetTypeIdFromComponent(targetComponentType);
            return MeasureDepth(device.Components, targetTypeId, 1);
        }

        private static int MeasureDepth(IEnumerable<IComponent>? components, string targetTypeId, int currentDepth)
        {
            if (components == null) return -1;
            foreach (var c in components)
            {
                if (c.Type == targetTypeId) return currentDepth;
                var nested = MeasureDepth(c.Components, targetTypeId, currentDepth + 1);
                if (nested > 0) return nested;
            }
            return -1;
        }

        private static string GetTypeIdFromComponent(Type componentType)
        {
            var field = componentType.GetField("TypeId");
            return (string)field!.GetValue(null)!;
        }
    }
}
