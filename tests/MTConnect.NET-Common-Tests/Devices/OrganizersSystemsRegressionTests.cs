// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// Regression guard for `Organizers.Systems` against the SysML model.
//
// Source (XMI): https://github.com/mtconnect/mtconnect_sysml_model
//   The `System` UML class is the parent of the substitution-group;
//   members are descendants whose summary description begins with
//   "System that ..." or "System composed of ..." (canonical SysML
//   phrasing applied to every `System` descendant).
// Source (prose): https://docs.mtconnect.org/Part_3.0_DevicesInformationModel
//   §"Systems" enumerates the members per published version.
//
// The generator that produces `*.g.cs` does not emit a substitution-group
// attribute, so this regression walks the assembly via reflection and
// matches the SysML descriptive convention. The intent is to catch a
// future regen that adds (or renames) a System member without updating
// `Organizers._systems`.
//
// `Controller` is a SysML `System` member but it has its own
// `Controllers` organizer (see `Device.AddComponent()`'s carve-out),
// so it is intentionally absent from `Organizers.Systems` and from the
// pinned baseline below. See `OrganizersControllerCarveOutTests` for
// the dedicated carve-out invariant.

using System.Linq;
using System.Reflection;
using MTConnect.Devices;
using MTConnect.Devices.Components;
using NUnit.Framework;

namespace MTConnect.Tests.Common.SystemsOrganizer
{
    [TestFixture]
    [Category("OrganizersSystemsRegressionGuard")]
    public class OrganizersSystemsRegressionTests
    {
        // The exact set of System substitution-group `TypeId` values pinned
        // here. Adding a new System member to the SysML model bumps this
        // list AND `Organizers._systems` together; either one moving without
        // the other fails this test.
        private static readonly string[] PinnedSystemMemberTypeIds = new[]
        {
            "AirHandler",
            // Controller intentionally absent — see file header.
            "Coolant",
            "Cooling",
            "Dielectric",
            "Electric",
            "Enclosure",
            "EndEffector",
            "Feeder",
            "Heating",
            "Hydraulic",
            "Lubrication",
            "PinTool",
            "Pneumatic",
            "Pressure",
            "ProcessPower",
            "Protective",
            "ToolHolder",
            "Vacuum",
            "WorkEnvelope",
        };

        [Test]
        public void Organizers_Systems_matches_pinned_System_substitution_group()
        {
            Assert.That(Organizers.Systems, Is.EquivalentTo(PinnedSystemMemberTypeIds),
                "`Organizers.Systems` has drifted from the pinned System " +
                "substitution-group list. If a regeneration added or renamed a " +
                "`System`-derived component, update both `Organizers._systems` " +
                "and `PinnedSystemMemberTypeIds` in this test.");
        }

        [Test]
        public void Every_System_described_Component_subclass_is_in_Organizers_Systems()
        {
            // Walks every concrete `IComponent` subclass in the assembly,
            // selects those whose `DescriptionText` matches the SysML
            // `System` substitution-group phrasing, and asserts each one
            // is enumerated by `Organizers.Systems`. Adding (or renaming)
            // a `System`-derived component via SysML regeneration without
            // updating `Organizers._systems` trips this guard.
            //
            // `Controller` is the sole structural exception: it is a SysML
            // `System` member but is routed through the dedicated
            // `Controllers` organizer, so the detector skips it (see
            // `DescribesSystemSubstitutionGroupMember`).
            var assembly = typeof(Organizers).Assembly;
            var componentTypes = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && typeof(IComponent).IsAssignableFrom(t))
                .ToArray();

            var detected = componentTypes
                .Where(t => DescribesSystemSubstitutionGroupMember(t))
                .Select(t => GetTypeIdConstant(t))
                .Where(id => id != null)
                .Cast<string>()
                .OrderBy(s => s)
                .ToArray();

            Assert.That(detected, Is.Not.Empty,
                "The reflection-based detector found zero `System` substitution-group " +
                "members. Either the assembly layout changed or the SysML descriptive " +
                "convention shifted — review the detector.");

            foreach (var typeId in detected)
            {
                Assert.That(Organizers.Systems, Has.Member(typeId),
                    $"Component class describing itself as a SysML `System` member " +
                    $"({typeId}) is missing from `Organizers.Systems`. " +
                    $"A future generator regeneration must keep `Organizers._systems` in sync.");
            }
        }

        private static bool DescribesSystemSubstitutionGroupMember(System.Type type)
        {
            // The SysML model materialized in `*.g.cs` files exposes the
            // human-readable description via `DescriptionText` const on each
            // `*Component` class. The canonical phrasing for a `System`
            // descendant is "System that ..." or "System composed of ...".
            // The abstract `SystemComponent` parent is filtered out by the
            // `!t.IsAbstract` guard in the caller — its description starts
            // with "Abstract Component ...".

            // `Controller` is a SysML `System` substitution-group member
            // (its `DescriptionText` begins with "System that ..."), but
            // this library routes it through the dedicated `Controllers`
            // organizer, so `Organizers.Systems` intentionally omits it.
            // Exclude the type from detection here so the membership assertion
            // does not flag the carve-out as a regression.
            if (type == typeof(ControllerComponent)) return false;

            var field = type.GetField("DescriptionText",
                BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            if (field == null) return false;
            var description = field.GetValue(null) as string;
            if (string.IsNullOrEmpty(description)) return false;
            return description.StartsWith("System that ", System.StringComparison.Ordinal)
                || description.StartsWith("System composed ", System.StringComparison.Ordinal);
        }

        private static string? GetTypeIdConstant(System.Type type)
        {
            var field = type.GetField("TypeId",
                BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
            return field?.GetValue(null) as string;
        }
    }
}
